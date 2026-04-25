using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Models;
using KeepPrinter.Core.Services;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace KeepPrinter.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de manipulación de PDFs usando PdfSharp.
/// </summary>
public class PdfService : IPdfService
{
    private readonly BatchCalculator _batchCalculator = new();

    /// <summary>
    /// Obtiene el número total de páginas de un archivo PDF.
    /// </summary>
    public async Task<int> GetPageCountAsync(string pdfPath)
    {
        return await Task.Run(() =>
        {
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException($"No se encontró el archivo PDF: {pdfPath}");
            }

            using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
            return document.PageCount;
        });
    }

    /// <summary>
    /// Genera los archivos PDF de frente y dorso para una tanda específica.
    /// </summary>
    public async Task<(string frontPath, string backPath)> GenerateBatchPdfsAsync(
        PrintSession session,
        TandaInfo batch,
        CancellationToken cancellationToken = default)
    {
        // Calcular páginas impares (frente) y pares (dorso)
        var oddPages = _batchCalculator.GetOddPages(batch.StartPage, batch.EndPage);
        var evenPages = _batchCalculator.GetEvenPages(batch.StartPage, batch.EndPage);

        // Nombres de archivos de salida
        var frontFileName = $"batch_{batch.Index:D3}_front.pdf";
        var backFileName = $"batch_{batch.Index:D3}_back.pdf";

        var frontPath = Path.Combine(session.OutputFolder, frontFileName);
        var backPath = Path.Combine(session.OutputFolder, backFileName);

        // Generar PDF de frente
        if (oddPages.Count > 0)
        {
            await ExtractPagesAsync(session.SourcePdfPath, frontPath, oddPages, cancellationToken);
        }

        // Generar PDF de dorso
        if (evenPages.Count > 0)
        {
            await ExtractPagesAsync(session.SourcePdfPath, backPath, evenPages, cancellationToken);
        }

        return (frontPath, backPath);
    }

    /// <summary>
    /// Extrae páginas específicas de un PDF y las guarda en un nuevo archivo.
    /// </summary>
    public async Task ExtractPagesAsync(
        string sourcePdfPath,
        string outputPdfPath,
        List<int> pageNumbers,
        CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            if (!File.Exists(sourcePdfPath))
            {
                throw new FileNotFoundException($"No se encontró el archivo PDF de origen: {sourcePdfPath}");
            }

            if (pageNumbers == null || pageNumbers.Count == 0)
            {
                throw new ArgumentException("La lista de páginas no puede estar vacía.", nameof(pageNumbers));
            }

            // Asegurar que el directorio de salida existe
            var outputDir = Path.GetDirectoryName(outputPdfPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using var sourceDocument = PdfReader.Open(sourcePdfPath, PdfDocumentOpenMode.Import);
            using var outputDocument = new PdfDocument();

            foreach (var pageNumber in pageNumbers)
            {
                // Validar que el número de página esté en rango
                // Recordar: pageNumbers están en base 1, PdfSharp usa base 0
                if (pageNumber < 1 || pageNumber > sourceDocument.PageCount)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(pageNumbers),
                        $"Número de página {pageNumber} fuera de rango (1-{sourceDocument.PageCount})");
                }

                var page = sourceDocument.Pages[pageNumber - 1];
                outputDocument.AddPage(page);
            }

            outputDocument.Save(outputPdfPath);
        }, cancellationToken);
    }
}
