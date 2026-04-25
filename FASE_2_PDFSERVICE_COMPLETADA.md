# ✅ Fase 2 (Parcial) - PdfService Completada

## 🎯 Objetivo Cumplido

Implementación completa del servicio de manipulación de PDFs usando **PdfSharp 6.2.4**.

---

## 📦 Lo que se implementó

### **1. PdfService** (`KeepPrinter.Infrastructure/Services/PdfService.cs`)

#### Métodos implementados:

##### `GetPageCountAsync(string pdfPath)`
- Obtiene el número total de páginas de un PDF
- Manejo de errores: `FileNotFoundException` si el archivo no existe
- Operación asíncrona

##### `GenerateBatchPdfsAsync(PrintSession session, TandaInfo batch, CancellationToken ct)`
- Genera dos PDFs por tanda:
  - **Frente**: `batch_000_front.pdf` con páginas impares (1, 3, 5, ...)
  - **Dorso**: `batch_000_back.pdf` con páginas pares (2, 4, 6, ...)
- Naming convention: `batch_{index:D3}_{front|back}.pdf` (ej: `batch_000_front.pdf`)
- Crea automáticamente la carpeta de salida si no existe
- Maneja correctamente PDFs con número impar de páginas

##### `ExtractPagesAsync(string sourcePdf, string outputPdf, List<int> pages, CancellationToken ct)`
- Extrae páginas específicas de un PDF (base 1)
- Valida que las páginas estén en rango
- Crea el directorio de salida si no existe
- Manejo robusto de errores

---

## 🧪 Tests Implementados (14 tests pasando)

### **TestPdfGenerator** (Helper)
Utilidad para generar PDFs de prueba:
- Crea PDFs con N páginas vacías
- Evita problemas de FontResolver en PdfSharp

### **PdfServiceTests**

#### GetPageCountAsync (3 tests)
✅ `GetPageCountAsync_ValidPdf_ReturnsCorrectCount` - PDF de 10 páginas retorna 10  
✅ `GetPageCountAsync_SinglePagePdf_ReturnsOne` - PDF de 1 página retorna 1  
✅ `GetPageCountAsync_NonExistentFile_ThrowsFileNotFoundException` - Valida error

#### ExtractPagesAsync (6 tests)
✅ `ExtractPagesAsync_ExtractSpecificPages_CreatesCorrectPdf` - Extrae páginas 1,3,5,7,9  
✅ `ExtractPagesAsync_ExtractSinglePage_CreatesOnePagePdf` - Extrae página 5  
✅ `ExtractPagesAsync_ExtractAllPages_CreatesIdenticalPdf` - Extrae todas (1-5)  
✅ `ExtractPagesAsync_EmptyPageList_ThrowsArgumentException` - Lista vacía lanza error  
✅ `ExtractPagesAsync_PageOutOfRange_ThrowsArgumentOutOfRangeException` - Página 15 en PDF de 10  
✅ `ExtractPagesAsync_NonExistentSourceFile_ThrowsFileNotFoundException` - Valida error

#### GenerateBatchPdfsAsync (5 tests)
✅ `GenerateBatchPdfsAsync_FirstBatch_GeneratesCorrectFrontAndBackPdfs` - Tanda 1: 50 frentes + 50 dorsos  
✅ `GenerateBatchPdfsAsync_SecondBatch_GeneratesCorrectlyNumberedFiles` - Verifica `batch_001_*`  
✅ `GenerateBatchPdfsAsync_OddNumberOfPages_GeneratesCorrectPdfs` - 99 páginas: 50 frentes, 49 dorsos  
✅ `GenerateBatchPdfsAsync_SmallBatch_GeneratesCorrectPdfs` - 4 hojas (8 páginas): 4 frentes, 4 dorsos  
✅ `GenerateBatchPdfsAsync_CreatesOutputFolderIfNotExists` - Crea carpeta automáticamente

---

## 📊 Resultados de Tests

```
Resumen de pruebas: total: 50; con errores: 0; correcto: 50; omitido: 0
  - KeepPrinter.Core.Tests: 36 tests ✅
  - KeepPrinter.Infrastructure.Tests: 14 tests ✅
```

---

## 🔧 Dependencias Instaladas

### PdfSharp 6.2.4
```bash
dotnet add src/KeepPrinter.Infrastructure package PDFsharp -v 6.2.4
dotnet add tests/KeepPrinter.Infrastructure.Tests package PDFsharp -v 6.2.4
```

**Características utilizadas:**
- `PdfReader.Open()` - Leer PDFs existentes
- `PdfDocument.AddPage()` - Agregar páginas a nuevo PDF
- `PdfDocument.Save()` - Guardar PDF a disco

---

## 🏗️ Arquitectura del Servicio

```
PdfService
│
├── GetPageCountAsync
│   └── Retorna: int (total de páginas)
│
├── GenerateBatchPdfsAsync
│   ├── Calcula páginas impares (frente)
│   ├── Calcula páginas pares (dorso)
│   ├── Genera batch_XXX_front.pdf
│   ├── Genera batch_XXX_back.pdf
│   └── Retorna: (string frontPath, string backPath)
│
└── ExtractPagesAsync
    ├── Valida páginas en rango
    ├── Crea carpeta de salida
    ├── Extrae páginas especificadas
    └── Guarda PDF resultante
```

---

## 💡 Decisiones de Diseño

### Páginas en Base 1
```csharp
// Usuario dice "página 5", internamente:
var page = sourceDocument.Pages[pageNumber - 1]; // Índice 4
```

### Naming Convention Consistente
```csharp
batch_000_front.pdf  // Primera tanda, frente
batch_000_back.pdf   // Primera tanda, dorso
batch_001_front.pdf  // Segunda tanda, frente
...
```

### Creación Automática de Carpetas
```csharp
var outputDir = Path.GetDirectoryName(outputPdfPath);
if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}
```

### Manejo de PDFs con Páginas Impares
```csharp
// PDF de 99 páginas:
// Frente: 1, 3, 5, ..., 99 = 50 páginas
// Dorso:  2, 4, 6, ..., 98 = 49 páginas (no hay página 100)
```

---

## 🎓 Lecciones Aprendidas

### PdfSharp FontResolver
**Problema:** PdfSharp 6.x requiere FontResolver para dibujar texto.  
**Solución:** Tests usan PDFs vacíos (sin texto) para simplicidad.  
**Futuro:** Si se necesita texto en tests, implementar FontResolver o usar páginas de PDFs reales.

### Cleanup de Archivos Temporales
**Implementación:** Clase `PdfServiceTests` implementa `IDisposable` para limpiar PDFs generados.
```csharp
public void Dispose()
{
    foreach (var file in _tempFiles)
    {
        if (File.Exists(file))
            try { File.Delete(file); } catch { }
    }
}
```

---

## ✅ Checklist de Completitud

- [x] Implementar `IPdfService`
- [x] Métodos asíncronos con `CancellationToken`
- [x] Manejo robusto de errores
- [x] Tests unitarios y de integración
- [x] Helper para generar PDFs de prueba
- [x] Validación de páginas fuera de rango
- [x] Creación automática de carpetas
- [x] Naming convention consistente
- [x] Documentación en código (XML comments)
- [x] Registro en DI (`ServiceCollectionExtensions`)
- [x] 100% de tests pasando

---

## 🚀 Próximos Pasos

### Completar Fase 2:

#### **PrintService** (Pendiente)
```csharp
public class PrintService : IPrintService
{
    Task<List<string>> GetAvailablePrintersAsync();
    Task<string?> GetDefaultPrinterAsync();
    Task<bool> PrintPdfAsync(string pdfPath, string printerName);
}
```

**Tareas:**
1. Enumerar impresoras del sistema usando `PrinterSettings`
2. Implementar impresión nativa (Win32 API)
3. Implementar fallback con SumatraPDF
4. Tests con impresora virtual (Microsoft Print to PDF)

#### **SessionStore** (Pendiente)
```csharp
public class SessionStore : ISessionStore
{
    Task SaveSessionAsync(PrintSession session);
    Task<PrintSession?> LoadLastSessionAsync();
    // ...
}
```

**Tareas:**
1. Serialización JSON de `PrintSession`
2. Guardar en `ApplicationData.LocalFolder`
3. Tests de guardado/carga
4. Manejo de sesiones corruptas

---

## 📚 Recursos Utilizados

### Documentación:
- [PdfSharp Documentation](http://www.pdfsharp.net/)
- [xUnit Documentation](https://xunit.net/)
- [.NET Async/Await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)

### NuGet Packages:
- `PDFsharp 6.2.4` - Manipulación de PDFs
- `xunit 2.x` - Framework de testing
- `Microsoft.Extensions.DependencyInjection.Abstractions 10.0.7` - DI

---

**Fecha de completitud:** 25 de abril de 2026  
**Autor:** Carlos (con asistencia de GitHub Copilot)  
**Estado:** ✅ Completado y testeado

---

## 🎉 Resumen Ejecutivo

**Fase 2 (PdfService):**
- ✅ 1 servicio implementado
- ✅ 3 métodos públicos
- ✅ 14 tests pasando
- ✅ 100% cobertura de funcionalidad crítica
- ✅ Integrado en DI
- ✅ Documentado

**Próximo hito:** Implementar `PrintService` para completar toda la Fase 2.
