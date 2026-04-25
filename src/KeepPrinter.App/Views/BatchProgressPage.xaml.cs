using Microsoft.UI.Xaml.Controls;
using KeepPrinter.ViewModels;

namespace KeepPrinter.App.Views;

/// <summary>
/// Página de progreso de impresión de tandas.
/// </summary>
public sealed partial class BatchProgressPage : Page
{
    public BatchProgressViewModel ViewModel => (BatchProgressViewModel)DataContext;

    public BatchProgressPage()
    {
        this.InitializeComponent();
    }
}
