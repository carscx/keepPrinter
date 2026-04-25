using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeepPrinter.Core.Models;

namespace KeepPrinter.ViewModels;

/// <summary>
/// ViewModel para la pantalla de finalización de la sesión.
/// </summary>
public partial class CompletionViewModel : ObservableObject
{
    private readonly PrintSession? _session;
    private readonly MainViewModel _mainViewModel;

    public CompletionViewModel(
        PrintSession? session,
        MainViewModel mainViewModel)
    {
        _session = session;
        _mainViewModel = mainViewModel;
    }

    /// <summary>
    /// Información de la sesión completada
    /// </summary>
    public string SourceFileName => _session != null 
        ? Path.GetFileName(_session.SourcePdfPath) 
        : "N/A";

    public int TotalPages => _session?.TotalPages ?? 0;
    public int TotalBatches => _session?.Batches.Count ?? 0;
    public int CompletedBatches => _session?.Batches.Count(b => b.IsComplete) ?? 0;
    public string OutputFolder => _session?.OutputFolder ?? "N/A";
    public DateTime StartTime => _session?.CreatedAt ?? DateTime.Now;
    public DateTime EndTime => _session?.LastModifiedAt ?? DateTime.Now;
    public TimeSpan Duration => EndTime - StartTime;

    public string DurationFormatted => Duration.TotalMinutes < 1
        ? $"{Duration.Seconds} segundos"
        : $"{Duration.Minutes} min {Duration.Seconds} seg";

    /// <summary>
    /// Abre la carpeta con los PDFs generados.
    /// </summary>
    [RelayCommand]
    private void OpenOutputFolder()
    {
        try
        {
            if (!string.IsNullOrEmpty(_session?.OutputFolder) && Directory.Exists(_session.OutputFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", _session.OutputFolder);
            }
        }
        catch
        {
            // Ignorar errores al abrir carpeta
        }
    }

    /// <summary>
    /// Inicia una nueva sesión.
    /// </summary>
    [RelayCommand]
    private void StartNewSession()
    {
        _mainViewModel.Restart();
    }

    /// <summary>
    /// Cierra la aplicación.
    /// </summary>
    [RelayCommand]
    private void ExitApplication()
    {
        App.Current.Exit();
    }
}
