namespace KeepPrinter.Core.Validation;

/// <summary>
/// Resultado de una operación de validación.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indica si la validación fue exitosa.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Mensajes de error si la validación falló.
    /// </summary>
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Crea un resultado exitoso.
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Crea un resultado fallido con un mensaje de error.
    /// </summary>
    public static ValidationResult Failure(string error) => new()
    {
        IsValid = false,
        Errors = new List<string> { error }
    };

    /// <summary>
    /// Crea un resultado fallido con múltiples mensajes de error.
    /// </summary>
    public static ValidationResult Failure(List<string> errors) => new()
    {
        IsValid = false,
        Errors = errors
    };
}
