using KeepPrinter.Core.Exceptions;
using KeepPrinter.Core.Models;

namespace KeepPrinter.Core.Validation;

/// <summary>
/// Valida las propiedades y estado de una PrintSession.
/// </summary>
public static class PrintSessionValidator
{
    /// <summary>
    /// Valida los parámetros iniciales de una sesión antes de preparar tandas.
    /// </summary>
    /// <exception cref="ValidationException">Si alguna validación falla.</exception>
    public static void ValidateInitialParameters(PrintSession session)
    {
        if (string.IsNullOrWhiteSpace(session.SourcePdfPath))
        {
            throw new ValidationException("La ruta del archivo PDF de origen no puede estar vacía.");
        }

        if (session.TotalPages <= 0)
        {
            throw new ValidationException("El número total de páginas debe ser mayor que cero.");
        }

        if (session.StartPage < 1)
        {
            throw new ValidationException("La página inicial debe ser mayor o igual a 1.");
        }

        if (session.StartPage > session.TotalPages)
        {
            throw new ValidationException(
                $"La página inicial ({session.StartPage}) no puede ser mayor que el total de páginas ({session.TotalPages}).");
        }

        if (session.SheetsPerBatch <= 0)
        {
            throw new ValidationException("El número de hojas por tanda debe ser mayor que cero.");
        }

        if (session.SheetsPerBatch > 1000)
        {
            throw new ValidationException("El número de hojas por tanda no puede exceder 1000.");
        }

        if (string.IsNullOrWhiteSpace(session.OutputFolder))
        {
            throw new ValidationException("La carpeta de salida no puede estar vacía.");
        }
    }

    /// <summary>
    /// Valida que una tanda esté lista para imprimir el frente.
    /// </summary>
    public static void ValidateReadyForFrontPrint(TandaInfo tanda)
    {
        if (string.IsNullOrWhiteSpace(tanda.FrontPdfPath))
        {
            throw new ValidationException("La tanda no tiene un archivo PDF de frente generado.");
        }

        if (tanda.FrontPrinted)
        {
            throw new ValidationException("El frente de esta tanda ya ha sido impreso.");
        }
    }

    /// <summary>
    /// Valida que una tanda esté lista para imprimir el dorso.
    /// </summary>
    public static void ValidateReadyForBackPrint(TandaInfo tanda)
    {
        if (string.IsNullOrWhiteSpace(tanda.BackPdfPath))
        {
            throw new ValidationException("La tanda no tiene un archivo PDF de dorso generado.");
        }

        if (!tanda.FrontPrinted)
        {
            throw new ValidationException("Debe imprimir el frente antes de imprimir el dorso.");
        }

        if (tanda.BackPrinted)
        {
            throw new ValidationException("El dorso de esta tanda ya ha sido impreso.");
        }
    }

    /// <summary>
    /// Valida que se pueda confirmar una tanda como completa.
    /// </summary>
    public static void ValidateCanConfirmBatch(TandaInfo tanda)
    {
        if (!tanda.FrontPrinted)
        {
            throw new ValidationException("No se puede confirmar una tanda sin haber impreso el frente.");
        }

        if (!tanda.BackPrinted)
        {
            throw new ValidationException("No se puede confirmar una tanda sin haber impreso el dorso.");
        }

        if (tanda.IsComplete)
        {
            throw new ValidationException("Esta tanda ya ha sido confirmada como completa.");
        }
    }

    /// <summary>
    /// Valida que se pueda avanzar a la siguiente tanda.
    /// </summary>
    public static void ValidateCanAdvanceBatch(PrintSession session)
    {
        if (session.CurrentBatchIndex >= session.Batches.Count - 1)
        {
            throw new ValidationException("No hay más tandas disponibles.");
        }

        var currentBatch = session.CurrentBatch;
        if (currentBatch != null && !currentBatch.IsComplete)
        {
            throw new ValidationException("Debe confirmar la tanda actual antes de avanzar.");
        }
    }
}
