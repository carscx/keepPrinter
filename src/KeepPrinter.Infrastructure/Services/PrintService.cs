using KeepPrinter.Core.Contracts;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace KeepPrinter.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de impresión para Windows usando APIs nativas.
/// Renderiza PDFs con Windows.Data.Pdf y usa PrintDocument para impresión.
/// </summary>
[SupportedOSPlatform("windows10.0.17763.0")]
public class PrintService : IPrintService
{
    /// <summary>
    /// Obtiene la lista de impresoras instaladas en el sistema.
    /// </summary>
    public Task<List<PrinterInfo>> GetAvailablePrintersAsync()
    {
        return Task.Run(() =>
        {
            var printers = new List<PrinterInfo>();
            var defaultPrinter = new PrinterSettings().PrinterName;

            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    var settings = new PrinterSettings { PrinterName = printerName };

                    printers.Add(new PrinterInfo
                    {
                        Name = printerName,
                        IsDefault = printerName.Equals(defaultPrinter, StringComparison.OrdinalIgnoreCase),
                        IsOnline = settings.IsValid,
                        Description = printerName
                    });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolver lista vacía
                Console.WriteLine($"Error al obtener impresoras: {ex.Message}");
            }

            return printers;
        });
    }

    /// <summary>
    /// Obtiene el nombre de la impresora predeterminada del sistema.
    /// </summary>
    public Task<string?> GetDefaultPrinterAsync()
    {
        return Task.Run<string?>(() =>
        {
            try
            {
                var settings = new PrinterSettings();
                return settings.PrinterName;
            }
            catch
            {
                return null;
            }
        });
    }

    /// <summary>
    /// Imprime un archivo PDF en la impresora especificada usando impresión nativa de Windows.
    /// </summary>
    public async Task PrintPdfAsync(
        string pdfFilePath,
        string printerName,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(pdfFilePath))
        {
            throw new FileNotFoundException($"No se encontró el archivo PDF: {pdfFilePath}");
        }

        if (string.IsNullOrWhiteSpace(printerName))
        {
            throw new ArgumentException("El nombre de la impresora no puede estar vacío.", nameof(printerName));
        }

        // Validar que la impresora existe
        var printers = await GetAvailablePrintersAsync();
        if (!printers.Any(p => p.Name.Equals(printerName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"La impresora '{printerName}' no está disponible en el sistema.");
        }

        await PrintPdfNativeAsync(pdfFilePath, printerName, cancellationToken);
    }

    /// <summary>
    /// Imprime un PDF usando Windows.Data.Pdf para renderizado y PrintDocument para impresión.
    /// </summary>
    private async Task PrintPdfNativeAsync(
        string pdfFilePath,
        string printerName,
        CancellationToken cancellationToken)
    {
        // Cargar el PDF usando Windows.Data.Pdf
        var file = await StorageFile.GetFileFromPathAsync(pdfFilePath);
        var pdfDocument = await PdfDocument.LoadFromFileAsync(file);

        if (pdfDocument.PageCount == 0)
        {
            throw new InvalidOperationException("El PDF no tiene páginas para imprimir.");
        }

        // Crear PrintDocument para manejar la impresión
        var printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;

        // Variables para controlar la paginación
        var currentPageIndex = 0;
        var pagesToPrint = new List<System.Drawing.Image>();

        try
        {
            // Renderizar todas las páginas del PDF a imágenes
            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var pdfPage = pdfDocument.GetPage(i);
                var image = await RenderPdfPageToImageAsync(pdfPage);
                pagesToPrint.Add(image);
            }

            // Configurar el evento de impresión de página
            printDocument.PrintPage += (sender, e) =>
            {
                if (currentPageIndex < pagesToPrint.Count)
                {
                    var image = pagesToPrint[currentPageIndex];

                    // Calcular escala para ajustar al área imprimible
                    var bounds = e.MarginBounds;
                    var scaleX = (float)bounds.Width / image.Width;
                    var scaleY = (float)bounds.Height / image.Height;
                    var scale = Math.Min(scaleX, scaleY);

                    var scaledWidth = (int)(image.Width * scale);
                    var scaledHeight = (int)(image.Height * scale);

                    // Centrar la imagen en la página
                    var x = bounds.Left + (bounds.Width - scaledWidth) / 2;
                    var y = bounds.Top + (bounds.Height - scaledHeight) / 2;

                    e.Graphics!.DrawImage(image, x, y, scaledWidth, scaledHeight);

                    currentPageIndex++;
                    e.HasMorePages = currentPageIndex < pagesToPrint.Count;
                }
                else
                {
                    e.HasMorePages = false;
                }
            };

            // Imprimir el documento
            await Task.Run(() => printDocument.Print(), cancellationToken);
        }
        finally
        {
            // Limpiar recursos
            foreach (var image in pagesToPrint)
            {
                image?.Dispose();
            }
            printDocument.Dispose();
        }
    }

    /// <summary>
    /// Renderiza una página de PDF a una imagen System.Drawing.Image.
    /// </summary>
    private async Task<System.Drawing.Image> RenderPdfPageToImageAsync(PdfPage pdfPage)
    {
        // Crear un stream en memoria para la imagen
        using var stream = new InMemoryRandomAccessStream();

        // Renderizar la página del PDF a 300 DPI (calidad de impresión)
        var renderOptions = new PdfPageRenderOptions
        {
            DestinationWidth = (uint)(pdfPage.Size.Width * 300 / 72), // 72 DPI a 300 DPI
            DestinationHeight = (uint)(pdfPage.Size.Height * 300 / 72)
        };

        await pdfPage.RenderToStreamAsync(stream, renderOptions);

        // Crear decoder para la imagen
        var decoder = await BitmapDecoder.CreateAsync(stream);
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied);

        // Convertir SoftwareBitmap a System.Drawing.Bitmap
        return await ConvertSoftwareBitmapToBitmapAsync(softwareBitmap);
    }

    /// <summary>
    /// Convierte un SoftwareBitmap (WinRT) a System.Drawing.Bitmap.
    /// </summary>
    private async Task<System.Drawing.Bitmap> ConvertSoftwareBitmapToBitmapAsync(SoftwareBitmap softwareBitmap)
    {
        using var stream = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

        encoder.SetSoftwareBitmap(softwareBitmap);
        await encoder.FlushAsync();

        // Convertir a System.Drawing.Bitmap
        stream.Seek(0);
        var bitmap = new System.Drawing.Bitmap(stream.AsStream());

        return bitmap;
    }
}
