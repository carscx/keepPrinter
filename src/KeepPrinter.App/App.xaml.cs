using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Services;
using KeepPrinter.Infrastructure.Services;
using KeepPrinter.ViewModels;
using KeepPrinter.App.Services;
using KeepPrinter.App.Views;

namespace KeepPrinter.App;

public partial class App : Application
{
    private Window? m_window;
    private IServiceProvider? _serviceProvider;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Crear ventana principal
        m_window = new MainWindow();

        // Configurar DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Crear MainPage y asignar ViewModel
        var mainPage = new MainPage();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainPage.DataContext = mainViewModel;

        // Asignar contenido a la ventana
        m_window.Content = mainPage;

        // Activar ventana
        m_window.Activate();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Servicios de infraestructura
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IPrintService, PrintService>();

        // Servicios de aplicación
        services.AddSingleton<IWindowService>(sp => new WindowService(m_window!));
        services.AddSingleton<IApplicationService, ApplicationService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SetupViewModel>();
        services.AddTransient<BatchProgressViewModel>();
        services.AddTransient<CompletionViewModel>();
    }
}
