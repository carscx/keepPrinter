using Microsoft.UI.Xaml;
using System;
using System.IO;

namespace KeepPrinter.App;

public partial class App : Application
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
        "KeepPrinter_Log.txt");

    public App()
    {
        WriteLog("=== App Constructor Started ===");
        try
        {
            WriteLog("Calling InitializeComponent...");
            InitializeComponent();
            WriteLog("InitializeComponent completed successfully");
        }
        catch (Exception ex)
        {
            WriteLog($"ERROR in Constructor: {ex.Message}");
            WriteLog($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        WriteLog("=== OnLaunched Started ===");
        try
        {
            WriteLog("Creating MainWindow...");
            m_window = new MainWindow();
            WriteLog("MainWindow created successfully");

            WriteLog("Activating window...");
            m_window.Activate();
            WriteLog("Window activated successfully");
        }
        catch (Exception ex)
        {
            WriteLog($"ERROR in OnLaunched: {ex.Message}");
            WriteLog($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    private Window? m_window;

    private static void WriteLog(string message)
    {
        try
        {
            File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
        }
        catch
        {
            // Si no se puede escribir el log, ignorar silenciosamente
        }
    }
}
