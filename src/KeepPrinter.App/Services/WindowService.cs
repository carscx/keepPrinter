using Microsoft.UI.Xaml;

namespace KeepPrinter.App.Services;

/// <summary>
/// Implementación del servicio de ventana para WinUI 3.
/// </summary>
public class WindowService : KeepPrinter.Core.Services.IWindowService
{
    private readonly Window _window;

    public WindowService(Window window)
    {
        _window = window;
    }

    /// <summary>
    /// Obtiene el handle nativo de la ventana principal.
    /// </summary>
    public nint GetMainWindowHandle()
    {
        return WinRT.Interop.WindowNative.GetWindowHandle(_window);
    }
}
