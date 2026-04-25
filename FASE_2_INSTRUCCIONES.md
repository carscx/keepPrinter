# 🚀 Cómo Continuar: Fase 2

## Objetivo de la Fase 2

Implementar los servicios de infraestructura para manejar PDFs e impresión.

---

## 1️⃣ Servicio de PDF (PdfService)

### Opción A: Usar PdfSharp (Recomendado - MIT License)

```bash
dotnet add src/KeepPrinter.Infrastructure package PdfSharp -v 6.2.0
```

**Ventajas:**
- ✅ Open source (MIT)
- ✅ Fácil de usar
- ✅ Buena documentación
- ✅ Comunidad activa

### Opción B: Usar iText7 (AGPL/Comercial)

```bash
dotnet add src/KeepPrinter.Infrastructure package itext7 -v 8.0.5
```

**Consideraciones:**
- ⚠️ Licencia AGPL (requiere código abierto) o comercial
- ✅ Más características
- ✅ Muy potente

### Implementación básica:

```csharp
// src/KeepPrinter.Infrastructure/Services/PdfService.cs
using KeepPrinter.Core.Contracts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace KeepPrinter.Infrastructure.Services;

public class PdfService : IPdfService
{
    public async Task<int> GetPageCountAsync(string pdfFilePath, CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            using var document = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Import);
            return document.PageCount;
        }, ct);
    }

    public async Task<bool> ValidatePdfFileAsync(string pdfFilePath, CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (!File.Exists(pdfFilePath)) return false;
                using var document = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Import);
                return document.PageCount > 0;
            }
            catch
            {
                return false;
            }
        }, ct);
    }

    public async Task GeneratePdfFromPagesAsync(
        string sourcePdfPath,
        string outputPdfPath,
        List<int> pageNumbers,
        CancellationToken ct = default)
    {
        await Task.Run(() =>
        {
            using var sourceDocument = PdfReader.Open(sourcePdfPath, PdfDocumentOpenMode.Import);
            using var outputDocument = new PdfDocument();

            foreach (var pageNum in pageNumbers)
            {
                // Las páginas en PdfSharp son base 0, las nuestras base 1
                var page = sourceDocument.Pages[pageNum - 1];
                outputDocument.AddPage(page);
            }

            outputDocument.Save(outputPdfPath);
        }, ct);
    }
}
```

---

## 2️⃣ Servicio de Impresión (PrintService)

### Estrategia de Implementación:

**Opción A: Win32 API (Nativo, más complejo)**

Usar P/Invoke para acceder a las APIs de impresión de Windows.

**Opción B: SumatraPDF CLI (Recomendado inicialmente)**

Usar SumatraPDF como fallback por su fiabilidad.

```bash
# Descargar SumatraPDF y colocarlo en la carpeta del proyecto
```

### Implementación básica:

```csharp
// src/KeepPrinter.Infrastructure/Services/PrintService.cs
using KeepPrinter.Core.Contracts;
using System.Diagnostics;
using System.Drawing.Printing;

namespace KeepPrinter.Infrastructure.Services;

public class PrintService : IPrintService
{
    public Task<List<string>> GetAvailablePrintersAsync(CancellationToken ct = default)
    {
        return Task.Run(() =>
        {
            var printers = new List<string>();
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printerName);
            }
            return printers;
        }, ct);
    }

    public Task<string?> GetDefaultPrinterAsync(CancellationToken ct = default)
    {
        return Task.Run(() =>
        {
            var settings = new PrinterSettings();
            return settings.PrinterName;
        }, ct);
    }

    public async Task<bool> PrintPdfAsync(
        string pdfFilePath,
        string printerName,
        CancellationToken ct = default)
    {
        try
        {
            // Opción: usar SumatraPDF
            var sumatraPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                "SumatraPDF.exe"
            );

            if (File.Exists(sumatraPath))
            {
                return await PrintWithSumatraAsync(sumatraPath, pdfFilePath, printerName, ct);
            }

            // Fallback: imprimir directamente (puede requerir más configuración)
            return await PrintDirectlyAsync(pdfFilePath, printerName, ct);
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> PrintWithSumatraAsync(
        string sumatraPath,
        string pdfPath,
        string printer,
        CancellationToken ct)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printer}\" \"{pdfPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null) return false;

        await process.WaitForExitAsync(ct);
        return process.ExitCode == 0;
    }

    private Task<bool> PrintDirectlyAsync(
        string pdfPath,
        string printer,
        CancellationToken ct)
    {
        // TODO: Implementar impresión directa con Win32 API o biblioteca de terceros
        throw new NotImplementedException("Impresión directa no implementada aún");
    }
}
```

### Dependencia adicional para impresión:

```bash
dotnet add src/KeepPrinter.Infrastructure package System.Drawing.Common
```

---

## 3️⃣ Registrar Servicios en DI

```csharp
// src/KeepPrinter.Infrastructure/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using KeepPrinter.Core.Contracts;
using KeepPrinter.Infrastructure.Services;

namespace KeepPrinter.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IPrintService, PrintService>();

        return services;
    }
}
```

Luego en la App:

```csharp
// src/KeepPrinter.App/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using KeepPrinter.Infrastructure;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        services.AddInfrastructureServices();
        // Agregar ViewModels y otros servicios aquí

        _serviceProvider = services.BuildServiceProvider();
    }

    // ...
}
```

---

## 4️⃣ Tests de Integración

Crear tests con PDFs de ejemplo:

```bash
# Crear carpeta de test fixtures
mkdir tests/KeepPrinter.Infrastructure.Tests/Fixtures
```

```csharp
// tests/KeepPrinter.Infrastructure.Tests/Services/PdfServiceTests.cs
using KeepPrinter.Infrastructure.Services;

public class PdfServiceTests
{
    private readonly PdfService _pdfService = new();
    private readonly string _testPdfPath = "Fixtures/test.pdf";

    [Fact]
    public async Task GetPageCount_ValidPdf_ReturnsCorrectCount()
    {
        // Arrange: Crear un PDF de prueba con 10 páginas

        // Act
        var pageCount = await _pdfService.GetPageCountAsync(_testPdfPath);

        // Assert
        Assert.Equal(10, pageCount);
    }

    [Fact]
    public async Task ValidatePdfFile_ValidPdf_ReturnsTrue()
    {
        var isValid = await _pdfService.ValidatePdfFileAsync(_testPdfPath);
        Assert.True(isValid);
    }

    [Fact]
    public async Task GeneratePdfFromPages_ExtractsCorrectPages()
    {
        // Arrange
        var outputPath = Path.GetTempFileName() + ".pdf";
        var pagesToExtract = new List<int> { 1, 3, 5 };

        // Act
        await _pdfService.GeneratePdfFromPagesAsync(_testPdfPath, outputPath, pagesToExtract);

        // Assert
        var resultCount = await _pdfService.GetPageCountAsync(outputPath);
        Assert.Equal(3, resultCount);

        // Cleanup
        File.Delete(outputPath);
    }
}
```

---

## 5️⃣ Workflow de Desarrollo Completo

### Paso a paso:

1. **Instalar PdfSharp**
   ```bash
   dotnet add src/KeepPrinter.Infrastructure package PdfSharp
   dotnet add src/KeepPrinter.Infrastructure package System.Drawing.Common
   ```

2. **Crear PdfService**
   - Implementar `IPdfService`
   - Tests básicos

3. **Crear PrintService**
   - Implementar `IPrintService`
   - Decidir estrategia (Win32 o Sumatra)

4. **Tests de integración**
   - Crear PDFs de prueba
   - Validar extracción de páginas

5. **Integrar con Core**
   - Probar flujo completo: cargar PDF → calcular tandas → generar PDFs

---

## 📚 Recursos Útiles

### PdfSharp:
- Documentación: http://www.pdfsharp.net/
- Ejemplos: https://github.com/empira/PDFsharp-samples

### Impresión Windows:
- PrinterSettings: https://learn.microsoft.com/en-us/dotnet/api/system.drawing.printing.printersettings
- SumatraPDF: https://www.sumatrapdfreader.org/docs/Command-line-arguments

### Testing:
- xUnit: https://xunit.net/
- Moq (si necesitas mocks): https://github.com/moq/moq4

---

## ❓ FAQ Fase 2

**P: ¿Qué biblioteca PDF usar?**  
R: PdfSharp es más simple y suficiente para extraer páginas. iText7 si necesitas más control.

**P: ¿Cómo probar la impresión sin impresora física?**  
R: Usa Microsoft Print to PDF como impresora de prueba.

**P: ¿Necesito SumatraPDF?**  
R: Es recomendado como fallback confiable, pero puedes implementar impresión nativa también.

**P: ¿Cómo manejo PDFs grandes?**  
R: Operaciones asíncronas + indicadores de progreso en UI (Fase 4).

---

**¡Listo para comenzar la Fase 2!** 🎉

Tienes todo lo necesario para implementar los servicios de infraestructura.
