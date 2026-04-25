using KeepPrinter.Core.Contracts;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace KeepPrinter.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de impresión para Windows.
/// Usa SumatraPDF como método principal de impresión silenciosa.
/// </summary>
[SupportedOSPlatform("windows")]
public class PrintService : IPrintService
{
    private const string SumatraPdfRelativePath = @"Tools\SumatraPDF.exe";

    /// <summary>
    /// Obtiene la lista de impresoras instaladas en el sistema.
    /// </summary>
    public Task<List<PrinterInfo>> GetAvailablePrintersAsync()
    {
        return Task.Run(() =>
        {
            var printers = new List<PrinterInfo>();
            var defaultPrinter = new PrinterSettings().PrinterName;

            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(new PrinterInfo
                    {
                        Name = printerName,
                        IsDefault = printerName.Equals(defaultPrinter, StringComparison.OrdinalIgnoreCase),
                        IsOnline = true, // Por defecto asumimos que está en línea
                        Description = printerName
                    });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolver lista vacía
                // TODO: Logging
                Console.WriteLine($"Error al obtener impresoras: {ex.Message}");
            }

            return printers;
        });
    }

    /// <summary>
    /// Obtiene el nombre de la impresora predeterminada del sistema.
    /// </summary>
    public Task<string?> GetDefaultPrinterAsync()
    {
        return Task.Run<string?>(() =>
        {
            try
            {
                var settings = new PrinterSettings();
                return settings.PrinterName;
            }
            catch
            {
                return null;
            }
        });
    }

    /// <summary>
    /// Imprime un archivo PDF en la impresora especificada usando SumatraPDF.
    /// </summary>
    public async Task PrintPdfAsync(
        string pdfFilePath,
        string printerName,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(pdfFilePath))
        {
            throw new FileNotFoundException($"No se encontró el archivo PDF: {pdfFilePath}");
        }

        if (string.IsNullOrWhiteSpace(printerName))
        {
            throw new ArgumentException("El nombre de la impresora no puede estar vacío.", nameof(printerName));
        }

        // Validar que la impresora existe
        var printers = await GetAvailablePrintersAsync();
        if (!printers.Any(p => p.Name.Equals(printerName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"La impresora '{printerName}' no está disponible en el sistema.");
        }

        // Intentar con SumatraPDF primero (más confiable)
        var sumatraPath = GetSumatraPdfPath();
        if (File.Exists(sumatraPath))
        {
            await PrintWithSumatraPdfAsync(pdfFilePath, printerName, sumatraPath, cancellationToken);
        }
        else
        {
            // Fallback: usar el verbo "print" del shell de Windows
            await PrintUsingShellAsync(pdfFilePath, printerName, cancellationToken);
        }
    }

    /// <summary>
    /// Imprime usando SumatraPDF en modo silencioso.
    /// </summary>
    private async Task PrintWithSumatraPdfAsync(
        string pdfFilePath,
        string printerName,
        string sumatraPath,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printerName}\" -silent \"{pdfFilePath}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new InvalidOperationException($"Error al imprimir con SumatraPDF (código {process.ExitCode}): {error}");
        }
    }

    /// <summary>
    /// Obtiene la ruta completa a SumatraPDF.exe
    /// </summary>
    private string GetSumatraPdfPath()
    {
        // Buscar relativo al directorio de la aplicación
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
        var sumatraPath = Path.Combine(appDir, SumatraPdfRelativePath);

        if (File.Exists(sumatraPath))
        {
            return sumatraPath;
        }

        // Buscar en el directorio raíz del proyecto (para desarrollo)
        var projectRoot = FindProjectRoot(appDir);
        if (projectRoot != null)
        {
            sumatraPath = Path.Combine(projectRoot, SumatraPdfRelativePath);
            if (File.Exists(sumatraPath))
            {
                return sumatraPath;
            }
        }

        // Retornar la ruta esperada (puede no existir)
        return Path.Combine(appDir, SumatraPdfRelativePath);
    }

    /// <summary>
    /// Encuentra la raíz del proyecto buscando hacia arriba el archivo .sln
    /// </summary>
    private string? FindProjectRoot(string startPath)
    {
        var currentDir = new DirectoryInfo(startPath);

        while (currentDir != null)
        {
            if (currentDir.GetFiles("*.sln").Length > 0)
            {
                return currentDir.FullName;
            }
            currentDir = currentDir.Parent;
        }

        return null;
    }

    /// <summary>
    /// Imprime usando el verbo "print" del shell de Windows (fallback).
    /// </summary>
    private async Task PrintUsingShellAsync(
        string pdfFilePath,
        string printerName,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = pdfFilePath,
            Verb = "print",
            Arguments = $"\"{printerName}\"",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("No se pudo iniciar el proceso de impresión.");
        }

        // Esperar un poco para que se envíe a la cola de impresión
        await Task.Delay(2000, cancellationToken);
    }
}
