namespace KeepPrinter.Core.Models;

/// <summary>
/// Representa una sesión de impresión completa, incluyendo el documento fuente,
/// parámetros de configuración, tandas generadas y progreso actual.
/// </summary>
public class PrintSession
{
    /// <summary>
    /// Identificador único de la sesión.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Ruta completa al archivo PDF de origen.
    /// </summary>
    public string SourcePdfPath { get; set; } = string.Empty;

    /// <summary>
    /// Carpeta de salida donde se generarán los PDFs de las tandas.
    /// </summary>
    public string OutputFolder { get; set; } = string.Empty;

    /// <summary>
    /// Número total de páginas del PDF original.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Página inicial desde donde comenzar la impresión (base 1).
    /// </summary>
    public int StartPage { get; set; } = 1;

    /// <summary>
    /// Cantidad de hojas físicas por tanda.
    /// </summary>
    public int SheetsPerBatch { get; set; } = 50;

    /// <summary>
    /// Nombre de la impresora seleccionada.
    /// </summary>
    public string? SelectedPrinter { get; set; }

    /// <summary>
    /// Lista de tandas generadas.
    /// </summary>
    public List<TandaInfo> Batches { get; set; } = new();

    /// <summary>
    /// Índice de la tanda actual en proceso (base 0).
    /// </summary>
    public int CurrentBatchIndex { get; set; } = 0;

    /// <summary>
    /// Etapa actual del flujo de trabajo.
    /// </summary>
    public WorkflowStage CurrentStage { get; set; } = WorkflowStage.NotPrepared;

    /// <summary>
    /// Fecha y hora de creación de la sesión.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de última modificación de la sesión.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Obtiene la tanda actual o null si no hay tandas.
    /// </summary>
    public TandaInfo? CurrentBatch =>
        CurrentBatchIndex >= 0 && CurrentBatchIndex < Batches.Count
            ? Batches[CurrentBatchIndex]
            : null;

    /// <summary>
    /// Indica si hay más tandas pendientes después de la actual.
    /// </summary>
    public bool HasMoreBatches => CurrentBatchIndex < Batches.Count - 1;

    /// <summary>
    /// Indica si la sesión está completamente finalizada.
    /// </summary>
    public bool IsFinished => CurrentStage == WorkflowStage.Finished;
}
