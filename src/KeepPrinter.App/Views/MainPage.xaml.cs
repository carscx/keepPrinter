using Microsoft.UI.Xaml.Controls;
using KeepPrinter.ViewModels;

namespace KeepPrinter.App.Views;

/// <summary>
/// Página principal con navegación entre vistas.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();
    }
}
