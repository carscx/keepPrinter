using KeepPrinter.Core.Models;

namespace KeepPrinter.Core.Contracts;

/// <summary>
/// Contrato para operaciones con archivos PDF.
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Obtiene el número total de páginas de un archivo PDF.
    /// </summary>
    /// <param name="pdfPath">Ruta al archivo PDF.</param>
    /// <returns>Número total de páginas.</returns>
    Task<int> GetPageCountAsync(string pdfPath);

    /// <summary>
    /// Genera los archivos PDF de frente y dorso para una tanda específica.
    /// </summary>
    /// <param name="session">Sesión actual.</param>
    /// <param name="batch">Tanda para la cual generar los PDFs.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Tupla con las rutas de los archivos generados (frente, dorso).</returns>
    Task<(string frontPath, string backPath)> GenerateBatchPdfsAsync(
        PrintSession session,
        TandaInfo batch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extrae páginas específicas de un PDF y las guarda en un nuevo archivo.
    /// </summary>
    /// <param name="sourcePdfPath">PDF de origen.</param>
    /// <param name="outputPdfPath">PDF de salida.</param>
    /// <param name="pageNumbers">Números de página a extraer (base 1).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task ExtractPagesAsync(
        string sourcePdfPath,
        string outputPdfPath,
        List<int> pageNumbers,
        CancellationToken cancellationToken = default);
}
