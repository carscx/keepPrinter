namespace KeepPrinter.Core.Exceptions;

/// <summary>
/// Excepción base para errores de validación en KeepPrinter.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
