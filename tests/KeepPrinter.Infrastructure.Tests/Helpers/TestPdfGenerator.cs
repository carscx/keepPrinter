using PdfSharp.Pdf;

namespace KeepPrinter.Infrastructure.Tests.Helpers;

/// <summary>
/// Generador de PDFs de prueba para tests.
/// </summary>
public static class TestPdfGenerator
{
    /// <summary>
    /// Genera un PDF de prueba con un número específico de páginas.
    /// Nota: Las páginas son vacías para evitar problemas de fuentes en PdfSharp.
    /// </summary>
    /// <param name="outputPath">Ruta donde guardar el PDF.</param>
    /// <param name="pageCount">Número de páginas a generar.</param>
    public static void GenerateTestPdf(string outputPath, int pageCount)
    {
        using var document = new PdfDocument();
        document.Info.Title = "Test PDF";
        document.Info.Author = "KeepPrinter Tests";
        document.Info.Subject = $"PDF de prueba con {pageCount} páginas";

        // Agregar páginas vacías
        for (int i = 1; i <= pageCount; i++)
        {
            var page = document.AddPage();
            // Página vacía intencionalmente (sin gráficos ni texto)
            // para evitar problemas de FontResolver en tests
        }

        // Asegurar que el directorio existe
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        document.Save(outputPath);
    }

    /// <summary>
    /// Crea un PDF de prueba temporal que se eliminará automáticamente.
    /// </summary>
    /// <param name="pageCount">Número de páginas.</param>
    /// <returns>Ruta al archivo temporal.</returns>
    public static string CreateTemporaryTestPdf(int pageCount)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_pdf_{Guid.NewGuid()}.pdf");
        GenerateTestPdf(tempPath, pageCount);
        return tempPath;
    }
}
