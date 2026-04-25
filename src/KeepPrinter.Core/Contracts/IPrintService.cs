namespace KeepPrinter.Core.Contracts;

/// <summary>
/// Información sobre una impresora disponible en el sistema.
/// </summary>
public class PrinterInfo
{
    /// <summary>
    /// Nombre de la impresora.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indica si esta es la impresora predeterminada del sistema.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Indica si la impresora está disponible/en línea.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Descripción o puerto de la impresora.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Contrato para operaciones de impresión.
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Obtiene la lista de impresoras disponibles en el sistema.
    /// </summary>
    /// <returns>Lista de impresoras.</returns>
    Task<List<PrinterInfo>> GetAvailablePrintersAsync();

    /// <summary>
    /// Imprime un archivo PDF en la impresora especificada.
    /// </summary>
    /// <param name="pdfPath">Ruta al archivo PDF a imprimir.</param>
    /// <param name="printerName">Nombre de la impresora.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task PrintPdfAsync(string pdfPath, string printerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el nombre de la impresora predeterminada del sistema.
    /// </summary>
    /// <returns>Nombre de la impresora predeterminada o null si no hay.</returns>
    Task<string?> GetDefaultPrinterAsync();
}
