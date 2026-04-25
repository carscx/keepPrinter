namespace KeepPrinter.Core.Models;

/// <summary>
/// Etapa actual del flujo de trabajo de impresión de una tanda.
/// </summary>
public enum WorkflowStage
{
    /// <summary>
    /// La sesión se ha creado pero no se han generado las tandas.
    /// </summary>
    NotPrepared,

    /// <summary>
    /// Las tandas están generadas y listas para imprimir.
    /// </summary>
    Prepared,

    /// <summary>
    /// Pendiente de imprimir el frente de la tanda actual.
    /// </summary>
    PendingFront,

    /// <summary>
    /// El frente está impreso, pendiente de imprimir el dorso.
    /// </summary>
    PendingBack,

    /// <summary>
    /// La tanda actual está completa (frente y dorso confirmados).
    /// </summary>
    BatchComplete,

    /// <summary>
    /// Todas las tandas han sido completadas.
    /// </summary>
    Finished
}
