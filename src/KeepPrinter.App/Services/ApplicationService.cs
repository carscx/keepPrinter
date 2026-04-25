using Microsoft.UI.Xaml;

namespace KeepPrinter.App.Services;

/// <summary>
/// Implementación del servicio de aplicación para WinUI 3.
/// </summary>
public class ApplicationService : KeepPrinter.Core.Services.IApplicationService
{
    /// <summary>
    /// Cierra la aplicación WinUI 3.
    /// </summary>
    public void Exit()
    {
        Application.Current.Exit();
    }
}
