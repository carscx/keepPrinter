namespace KeepPrinter.Core.Models;

/// <summary>
/// Información sobre una tanda de impresión (conjunto de páginas para frente/dorso).
/// </summary>
public class TandaInfo
{
    /// <summary>
    /// Índice de la tanda (base 0).
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Número de página inicial del PDF original (base 1).
    /// </summary>
    public int StartPage { get; set; }

    /// <summary>
    /// Número de página final del PDF original (base 1), inclusivo.
    /// </summary>
    public int EndPage { get; set; }

    /// <summary>
    /// Cantidad de hojas físicas en esta tanda (cada hoja tiene frente y dorso).
    /// </summary>
    public int SheetCount { get; set; }

    /// <summary>
    /// Ruta del archivo PDF generado para el frente (páginas impares).
    /// </summary>
    public string? FrontPdfPath { get; set; }

    /// <summary>
    /// Ruta del archivo PDF generado para el dorso (páginas pares).
    /// </summary>
    public string? BackPdfPath { get; set; }

    /// <summary>
    /// Indica si el frente de esta tanda ha sido impreso.
    /// </summary>
    public bool FrontPrinted { get; set; }

    /// <summary>
    /// Indica si el dorso de esta tanda ha sido impreso.
    /// </summary>
    public bool BackPrinted { get; set; }

    /// <summary>
    /// Indica si esta tanda ha sido confirmada como completa por el usuario.
    /// </summary>
    public bool IsComplete { get; set; }
}
