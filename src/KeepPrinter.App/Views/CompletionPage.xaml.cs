using Microsoft.UI.Xaml.Controls;
using KeepPrinter.ViewModels;

namespace KeepPrinter.App.Views;

/// <summary>
/// Página de finalización de la sesión de impresión.
/// </summary>
public sealed partial class CompletionPage : Page
{
    public CompletionViewModel ViewModel => (CompletionViewModel)DataContext;

    public CompletionPage()
    {
        this.InitializeComponent();
    }
}
