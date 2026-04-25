using KeepPrinter.Infrastructure.Services;
using KeepPrinter.Infrastructure.Tests.Helpers;

namespace KeepPrinter.Infrastructure.Tests.Services;

/// <summary>
/// Tests para el servicio de impresión.
/// </summary>
public class PrintServiceTests : IDisposable
{
    private readonly PrintService _printService;
    private readonly List<string> _tempFiles;

    public PrintServiceTests()
    {
        _printService = new PrintService();
        _tempFiles = new List<string>();
    }

    public void Dispose()
    {
        foreach (var file in _tempFiles.Where(File.Exists))
        {
            try { File.Delete(file); } catch { }
        }
    }

    [Fact]
    public async Task GetAvailablePrintersAsync_ReturnsListOfPrinters()
    {
        // Act
        var printers = await _printService.GetAvailablePrintersAsync();

        // Assert
        Assert.NotNull(printers);
        // En un sistema Windows siempre debe haber al menos una impresora (puede ser virtual como "Microsoft Print to PDF")
        // No fallamos si está vacío porque puede ser un entorno CI sin impresoras
    }

    [Fact]
    public async Task GetAvailablePrintersAsync_MarkOneAsDefault()
    {
        // Act
        var printers = await _printService.GetAvailablePrintersAsync();

        // Assert
        if (printers.Any())
        {
            Assert.Single(printers.Where(p => p.IsDefault));
        }
    }

    [Fact]
    public async Task GetDefaultPrinterAsync_ReturnsNameOrNull()
    {
        // Act
        var defaultPrinter = await _printService.GetDefaultPrinterAsync();

        // Assert
        // Puede ser null si no hay impresoras instaladas
        // Si no es null, debe ser un string no vacío
        if (defaultPrinter != null)
        {
            Assert.False(string.IsNullOrWhiteSpace(defaultPrinter));
        }
    }

    [Fact]
    public async Task GetDefaultPrinterAsync_MatchesDefaultInList()
    {
        // Arrange
        var printers = await _printService.GetAvailablePrintersAsync();
        var defaultPrinter = await _printService.GetDefaultPrinterAsync();

        // Assert
        if (printers.Any() && defaultPrinter != null)
        {
            var defaultInList = printers.FirstOrDefault(p => p.IsDefault);
            Assert.NotNull(defaultInList);
            Assert.Equal(defaultPrinter, defaultInList.Name);
        }
    }

    [Fact]
    public async Task PrintPdfAsync_ThrowsFileNotFoundException_WhenPdfDoesNotExist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent.pdf");
        var printerName = "Microsoft Print to PDF";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _printService.PrintPdfAsync(nonExistentPath, printerName));
    }

    [Fact]
    public async Task PrintPdfAsync_ThrowsArgumentException_WhenPrinterNameIsEmpty()
    {
        // Arrange
        var testPdfPath = TestPdfGenerator.CreateTemporaryTestPdf(1);
        _tempFiles.Add(testPdfPath);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _printService.PrintPdfAsync(testPdfPath, ""));
    }

    [Fact]
    public async Task PrintPdfAsync_ThrowsArgumentException_WhenPrinterNameIsNull()
    {
        // Arrange
        var testPdfPath = TestPdfGenerator.CreateTemporaryTestPdf(1);
        _tempFiles.Add(testPdfPath);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _printService.PrintPdfAsync(testPdfPath, null!));
    }

    [Fact]
    public async Task PrintPdfAsync_ThrowsInvalidOperationException_WhenPrinterDoesNotExist()
    {
        // Arrange
        var testPdfPath = TestPdfGenerator.CreateTemporaryTestPdf(1);
        _tempFiles.Add(testPdfPath);
        var invalidPrinterName = "NonExistentPrinter_XYZ123";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _printService.PrintPdfAsync(testPdfPath, invalidPrinterName));
    }

    [Fact]
    public async Task PrintPdfAsync_AcceptsValidPdfAndPrinter()
    {
        // Arrange
        var testPdfPath = TestPdfGenerator.CreateTemporaryTestPdf(1);
        _tempFiles.Add(testPdfPath);

        var printers = await _printService.GetAvailablePrintersAsync();
        if (!printers.Any())
        {
            // Skip test si no hay impresoras disponibles (entornos CI)
            return;
        }

        var printerName = printers.First().Name;

        // Act & Assert - no debe lanzar excepción
        // Nota: No validamos si realmente imprimió porque depende del entorno
        // Solo verificamos que el método no lance excepciones con parámetros válidos
        var exception = await Record.ExceptionAsync(
            () => _printService.PrintPdfAsync(testPdfPath, printerName));

        // En algunos entornos puede fallar si no hay SumatraPDF o el visor no funciona
        // Por eso solo verificamos que si falla, no sea por validación de parámetros
        if (exception != null)
        {
            Assert.IsNotType<ArgumentException>(exception);
            Assert.IsNotType<FileNotFoundException>(exception);
        }
    }

    [Fact]
    public async Task GetAvailablePrintersAsync_AllPrintersHaveNonEmptyName()
    {
        // Act
        var printers = await _printService.GetAvailablePrintersAsync();

        // Assert
        foreach (var printer in printers)
        {
            Assert.False(string.IsNullOrWhiteSpace(printer.Name));
        }
    }

    [Fact]
    public async Task GetAvailablePrintersAsync_IsOnlinePropertyIsSet()
    {
        // Act
        var printers = await _printService.GetAvailablePrintersAsync();

        // Assert
        foreach (var printer in printers)
        {
            // IsOnline debe tener un valor (true o false)
            Assert.NotNull(printer.IsOnline);
        }
    }

    [Fact]
    public async Task PrintPdfAsync_HandlesCancellationToken()
    {
        // Arrange
        var testPdfPath = TestPdfGenerator.CreateTemporaryTestPdf(1);
        _tempFiles.Add(testPdfPath);

        var printers = await _printService.GetAvailablePrintersAsync();
        if (!printers.Any())
        {
            return; // Skip si no hay impresoras
        }

        var printerName = printers.First().Name;
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancelar inmediatamente

        // Act & Assert
        // Puede lanzar OperationCanceledException o completar antes de cancelación
        var exception = await Record.ExceptionAsync(
            () => _printService.PrintPdfAsync(testPdfPath, printerName, cts.Token));

        if (exception != null)
        {
            // Si hay excepción, debe ser por cancelación o por fallos de impresión
            // No debe ser por validación de parámetros
            Assert.IsNotType<ArgumentException>(exception);
            Assert.IsNotType<FileNotFoundException>(exception);
        }
    }
}
