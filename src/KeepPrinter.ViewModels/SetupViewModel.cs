using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Models;
using KeepPrinter.Core.Services;
using KeepPrinter.Core.Validation;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace KeepPrinter.ViewModels;

/// <summary>
/// ViewModel para la configuración inicial de la sesión de impresión.
/// </summary>
public partial class SetupViewModel : ObservableObject
{
    private readonly IPdfService _pdfService;
    private readonly IWindowService _windowService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private string? _selectedPdfPath;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private int _startPage = 1;

    [ObservableProperty]
    private int _sheetsPerBatch = 25;

    [ObservableProperty]
    private string? _outputFolder;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _statusMessage;

    public SetupViewModel(
        IPdfService pdfService,
        IWindowService windowService,
        MainViewModel mainViewModel)
    {
        _pdfService = pdfService;
        _windowService = windowService;
        _mainViewModel = mainViewModel;
    }

    /// <summary>
    /// Abre el diálogo para seleccionar un archivo PDF.
    /// </summary>
    [RelayCommand]
    private async Task SelectPdfAsync()
    {
        try
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add(".pdf");

            // Obtener la ventana actual para el picker
            var hwnd = _windowService.GetMainWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                SelectedPdfPath = file.Path;
                await LoadPdfInfoAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al seleccionar archivo: {ex.Message}";
        }
    }

    /// <summary>
    /// Carga la información del PDF seleccionado.
    /// </summary>
    private async Task LoadPdfInfoAsync()
    {
        if (string.IsNullOrEmpty(SelectedPdfPath))
        {
            return;
        }

        try
        {
            IsProcessing = true;
            ErrorMessage = null;
            StatusMessage = "Analizando PDF...";

            TotalPages = await _pdfService.GetPageCountAsync(SelectedPdfPath);

            // Configurar carpeta de salida por defecto
            if (string.IsNullOrEmpty(OutputFolder))
            {
                var pdfFolder = Path.GetDirectoryName(SelectedPdfPath);
                OutputFolder = Path.Combine(pdfFolder!, "KeepPrinter_Output");
            }

            StatusMessage = $"PDF cargado: {TotalPages} páginas";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar PDF: {ex.Message}";
            TotalPages = 0;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Selecciona la carpeta de salida para los PDFs generados.
    /// </summary>
    [RelayCommand]
    private async Task SelectOutputFolderAsync()
    {
        try
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add("*");

            var hwnd = _windowService.GetMainWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                OutputFolder = folder.Path;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al seleccionar carpeta: {ex.Message}";
        }
    }

    /// <summary>
    /// Inicia la sesión de impresión.
    /// </summary>
    [RelayCommand]
    private async Task StartSessionAsync()
    {
        ErrorMessage = null;

        // Validar configuración
        if (string.IsNullOrEmpty(SelectedPdfPath))
        {
            ErrorMessage = "Debes seleccionar un archivo PDF.";
            return;
        }

        if (string.IsNullOrEmpty(OutputFolder))
        {
            ErrorMessage = "Debes seleccionar una carpeta de salida.";
            return;
        }

        try
        {
            IsProcessing = true;
            StatusMessage = "Preparando sesión...";

            // Crear sesión
            var session = new PrintSession
            {
                Id = Guid.NewGuid(),
                SourcePdfPath = SelectedPdfPath,
                OutputFolder = OutputFolder,
                StartPage = StartPage,
                TotalPages = TotalPages,
                SheetsPerBatch = SheetsPerBatch,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now
            };

            // Validar parámetros
            PrintSessionValidator.ValidateInitialParameters(session);

            // Calcular tandas
            StatusMessage = "Calculando tandas...";
            var calculator = new BatchCalculator();
            session.Batches = calculator.CalculateBatches(session);

            // Generar PDFs de tandas
            StatusMessage = $"Generando {session.Batches.Count} tandas...";

            foreach (var batch in session.Batches)
            {
                await _pdfService.GenerateBatchPdfsAsync(session, batch);
            }

            session.CurrentStage = WorkflowStage.Prepared;

            StatusMessage = "¡Sesión lista para imprimir!";

            // Iniciar sesión en MainViewModel
            _mainViewModel.StartSession(session);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al preparar sesión: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Indica si se puede iniciar la sesión.
    /// </summary>
    public bool CanStartSession => 
        !string.IsNullOrEmpty(SelectedPdfPath) && 
        !string.IsNullOrEmpty(OutputFolder) &&
        TotalPages > 0 &&
        !IsProcessing;
}
