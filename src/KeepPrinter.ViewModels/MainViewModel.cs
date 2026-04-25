using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeepPrinter.Core.Models;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Services;

namespace KeepPrinter.ViewModels;

/// <summary>
/// ViewModel principal que gestiona la navegación y el estado de la sesión.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IPdfService _pdfService;
    private readonly IPrintService _printService;
    private readonly IWindowService _windowService;
    private readonly IApplicationService _applicationService;

    [ObservableProperty]
    private PrintSession? _currentSession;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _currentViewName = "Setup";

    public MainViewModel(
        IPdfService pdfService,
        IPrintService printService,
        IWindowService windowService,
        IApplicationService applicationService)
    {
        _pdfService = pdfService;
        _printService = printService;
        _windowService = windowService;
        _applicationService = applicationService;

        // Iniciar en vista de configuración
        NavigateToSetup();
    }

    /// <summary>
    /// Navega a la vista de configuración inicial.
    /// </summary>
    [RelayCommand]
    private void NavigateToSetup()
    {
        CurrentViewName = "Setup";
        CurrentView = new SetupViewModel(_pdfService, _windowService, this);
    }

    /// <summary>
    /// Navega a la vista de progreso de tandas.
    /// </summary>
    [RelayCommand]
    private void NavigateToBatchProgress()
    {
        if (CurrentSession == null)
        {
            return;
        }

        CurrentViewName = "BatchProgress";
        CurrentView = new BatchProgressViewModel(CurrentSession, _pdfService, _printService, this);
    }

    /// <summary>
    /// Navega a la vista de finalización.
    /// </summary>
    [RelayCommand]
    public void NavigateToCompletion()
    {
        CurrentViewName = "Completion";
        CurrentView = new CompletionViewModel(CurrentSession, this, _applicationService);
    }

    /// <summary>
    /// Crea una nueva sesión y navega a progreso de tandas.
    /// </summary>
    public void StartSession(PrintSession session)
    {
        CurrentSession = session;
        NavigateToBatchProgress();
    }

    /// <summary>
    /// Reinicia la aplicación (nueva sesión).
    /// </summary>
    [RelayCommand]
    public void Restart()
    {
        CurrentSession = null;
        NavigateToSetup();
    }
}
