using Microsoft.UI.Xaml.Controls;
using KeepPrinter.ViewModels;

namespace KeepPrinter.App.Views;

/// <summary>
/// Página de configuración inicial de la sesión de impresión.
/// </summary>
public sealed partial class SetupPage : Page
{
    public SetupViewModel ViewModel => (SetupViewModel)DataContext;

    public SetupPage()
    {
        this.InitializeComponent();
    }
}
