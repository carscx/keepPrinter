using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Core.Models;

namespace KeepPrinter.ViewModels;

/// <summary>
/// ViewModel para el progreso de impresión de tandas.
/// </summary>
public partial class BatchProgressViewModel : ObservableObject
{
    private readonly PrintSession _session;
    private readonly IPdfService _pdfService;
    private readonly IPrintService _printService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private TandaInfo? _currentBatch;

    [ObservableProperty]
    private int _currentBatchNumber;

    [ObservableProperty]
    private int _totalBatches;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _selectedPrinter;

    [ObservableProperty]
    private List<PrinterInfo> _availablePrinters = new();

    public BatchProgressViewModel(
        PrintSession session,
        IPdfService pdfService,
        IPrintService printService,
        MainViewModel mainViewModel)
    {
        _session = session;
        _pdfService = pdfService;
        _printService = printService;
        _mainViewModel = mainViewModel;

        TotalBatches = _session.Batches.Count;
        CurrentBatchNumber = _session.CurrentBatchIndex + 1;
        CurrentBatch = _session.CurrentBatch;

        _ = LoadPrintersAsync();
    }

    /// <summary>
    /// Carga las impresoras disponibles.
    /// </summary>
    private async Task LoadPrintersAsync()
    {
        try
        {
            AvailablePrinters = await _printService.GetAvailablePrintersAsync();

            // Seleccionar impresora predeterminada o la guardada en sesión
            if (!string.IsNullOrEmpty(_session.SelectedPrinter))
            {
                SelectedPrinter = _session.SelectedPrinter;
            }
            else
            {
                var defaultPrinter = await _printService.GetDefaultPrinterAsync();
                SelectedPrinter = defaultPrinter ?? AvailablePrinters.FirstOrDefault()?.Name;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar impresoras: {ex.Message}";
        }
    }

    /// <summary>
    /// Imprime el frente de la tanda actual.
    /// </summary>
    [RelayCommand]
    private async Task PrintFrontAsync()
    {
        if (CurrentBatch == null || string.IsNullOrEmpty(SelectedPrinter))
        {
            ErrorMessage = "Selecciona una impresora.";
            return;
        }

        try
        {
            IsProcessing = true;
            ErrorMessage = null;
            StatusMessage = $"Imprimiendo frente de tanda {CurrentBatchNumber}...";

            await _printService.PrintPdfAsync(CurrentBatch.FrontPdfPath!, SelectedPrinter);

            CurrentBatch.FrontPrinted = true;
            _session.SelectedPrinter = SelectedPrinter;
            _session.LastModifiedAt = DateTime.Now;

            // Actualizar etapa
            if (_session.CurrentStage == WorkflowStage.Prepared)
            {
                _session.CurrentStage = WorkflowStage.PendingBack;
            }

            StatusMessage = "✅ Frente impreso correctamente";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al imprimir frente: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Imprime el dorso de la tanda actual.
    /// </summary>
    [RelayCommand]
    private async Task PrintBackAsync()
    {
        if (CurrentBatch == null || string.IsNullOrEmpty(SelectedPrinter))
        {
            ErrorMessage = "Selecciona una impresora.";
            return;
        }

        if (!CurrentBatch.FrontPrinted)
        {
            ErrorMessage = "Debes imprimir el frente primero.";
            return;
        }

        try
        {
            IsProcessing = true;
            ErrorMessage = null;
            StatusMessage = $"Imprimiendo dorso de tanda {CurrentBatchNumber}...";

            await _printService.PrintPdfAsync(CurrentBatch.BackPdfPath!, SelectedPrinter);

            CurrentBatch.BackPrinted = true;
            _session.LastModifiedAt = DateTime.Now;

            StatusMessage = "✅ Dorso impreso correctamente";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al imprimir dorso: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Confirma que la tanda actual está completa y avanza a la siguiente.
    /// </summary>
    [RelayCommand]
    private void ConfirmBatchComplete()
    {
        if (CurrentBatch == null)
        {
            return;
        }

        if (!CurrentBatch.FrontPrinted || !CurrentBatch.BackPrinted)
        {
            ErrorMessage = "Debes imprimir frente y dorso antes de continuar.";
            return;
        }

        CurrentBatch.IsComplete = true;
        _session.CurrentStage = WorkflowStage.BatchComplete;
        _session.LastModifiedAt = DateTime.Now;

        // Avanzar a siguiente tanda
        if (_session.HasMoreBatches)
        {
            _session.CurrentBatchIndex++;
            _session.CurrentStage = WorkflowStage.Prepared;

            CurrentBatch = _session.CurrentBatch;
            CurrentBatchNumber = _session.CurrentBatchIndex + 1;

            StatusMessage = $"Tanda {CurrentBatchNumber} de {TotalBatches}";
            ErrorMessage = null;
        }
        else
        {
            // Sesión completa
            _session.CurrentStage = WorkflowStage.Finished;
            _mainViewModel.NavigateToCompletion();
        }
    }

    /// <summary>
    /// Abre la carpeta con los PDFs generados.
    /// </summary>
    [RelayCommand]
    private void OpenOutputFolder()
    {
        try
        {
            if (!string.IsNullOrEmpty(_session.OutputFolder) && Directory.Exists(_session.OutputFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", _session.OutputFolder);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al abrir carpeta: {ex.Message}";
        }
    }

    /// <summary>
    /// Cancela la sesión actual.
    /// </summary>
    [RelayCommand]
    private void CancelSession()
    {
        _mainViewModel.Restart();
    }

    /// <summary>
    /// Propiedades computadas
    /// </summary>
    public int CompletedBatches => _session.Batches.Count(b => b.IsComplete);
    public double ProgressPercentage => (double)CompletedBatches / TotalBatches * 100;
    public bool CanPrintFront => !IsProcessing && CurrentBatch != null;
    public bool CanPrintBack => !IsProcessing && CurrentBatch?.FrontPrinted == true;
    public bool CanConfirm => CurrentBatch?.FrontPrinted == true && CurrentBatch?.BackPrinted == true;
}
