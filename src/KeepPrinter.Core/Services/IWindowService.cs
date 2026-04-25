namespace KeepPrinter.Core.Services;

/// <summary>
/// Servicio para interactuar con la ventana principal de la aplicación.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Obtiene el handle nativo de la ventana principal.
    /// </summary>
    nint GetMainWindowHandle();
}
