# 🖨️ PrintService - Configuración de SumatraPDF

## Descripción

El `PrintService` de KeepPrinter utiliza **SumatraPDF** como método preferido para impresión silenciosa de archivos PDF en Windows. Si SumatraPDF no está disponible, el servicio usa un método fallback mediante el verbo "print" del shell de Windows.

---

## ¿Por qué SumatraPDF?

✅ **Ventajas:**
- Impresión silenciosa (`-silent`) sin mostrar ventanas
- Control exacto de la impresora destino (`-print-to`)
- Ligero y rápido (< 10 MB)
- Gratuito y open source (GPLv3)
- No requiere instalación (portable)

❌ **Alternativa (Shell "print"):**
- Abre el visor PDF predeterminado
- Puede mostrar ventanas al usuario
- Menos confiable en automatización

---

## 📥 Instalación

### Opción 1: Manual (Desarrollo)

1. **Descargar SumatraPDF:**
   - Ir a: https://www.sumatrapdfreader.org/download-free-pdf-viewer
   - Descargar la versión **portable** (64-bit o 32-bit según tu sistema)

2. **Extraer el ejecutable:**
   - Descomprimir el archivo ZIP descargado
   - Copiar `SumatraPDF.exe` a la carpeta del proyecto

3. **Colocar en la ruta correcta:**
   ```
   C:\Users\karsc\Proyectos\impresionTandas\keepPrinter\Tools\SumatraPDF.exe
   ```

4. **Verificar:**
   ```powershell
   Test-Path ".\Tools\SumatraPDF.exe"
   # Debe retornar: True
   ```

### Opción 2: Automática (PowerShell)

Ejecuta este comando desde la raíz del proyecto:

```powershell
# Crear carpeta Tools si no existe
New-Item -ItemType Directory -Force -Path ".\Tools"

# Descargar SumatraPDF portable (64-bit)
$url = "https://www.sumatrapdfreader.org/dl/rel/3.5.2/SumatraPDF-3.5.2-64.zip"
$zipPath = ".\Tools\SumatraPDF.zip"
Invoke-WebRequest -Uri $url -OutFile $zipPath

# Extraer solo el ejecutable
Expand-Archive -Path $zipPath -DestinationPath ".\Tools\Temp" -Force
Move-Item -Path ".\Tools\Temp\SumatraPDF.exe" -Destination ".\Tools\SumatraPDF.exe" -Force

# Limpiar archivos temporales
Remove-Item -Path ".\Tools\Temp" -Recurse -Force
Remove-Item -Path $zipPath -Force

Write-Host "✅ SumatraPDF instalado correctamente en .\Tools\SumatraPDF.exe" -ForegroundColor Green
```

---

## 🔧 Uso en Código

```csharp
using KeepPrinter.Infrastructure.Services;

var printService = new PrintService();

// Obtener impresoras disponibles
var printers = await printService.GetAvailablePrintersAsync();
foreach (var printer in printers)
{
    Console.WriteLine($"{printer.Name} (Default: {printer.IsDefault})");
}

// Imprimir un PDF
var pdfPath = @"C:\temp\documento.pdf";
var printerName = "HP LaserJet Pro M402";
await printService.PrintPdfAsync(pdfPath, printerName);
```

---

## 🧪 Tests

El proyecto incluye **12 tests** para `PrintService`:

- ✅ Enumeración de impresoras
- ✅ Detección de impresora predeterminada
- ✅ Validación de parámetros (archivo inexistente, impresora inválida, etc.)
- ✅ Manejo de CancellationToken

Los tests funcionan **sin SumatraPDF** (usan métodos de validación y enumeración que no requieren impresión real).

---

## 🚀 Producción (MSIX Packaging)

En la fase de empaquetado MSIX (Fase 5), se incluirá automáticamente:

```xml
<!-- KeepPrinter.App.csproj -->
<ItemGroup>
  <Content Include="..\Tools\SumatraPDF.exe">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

El ejecutable se copiará a la carpeta `Tools\` dentro del paquete de la aplicación.

---

## ❓ Preguntas Frecuentes

### ¿Qué pasa si no tengo SumatraPDF?

La aplicación usará el método fallback (verbo "print" del shell de Windows), que es menos confiable pero funcional.

### ¿Puedo usar otra impresora PDF?

Sí, cualquier impresora que acepte el verbo "print" de Windows funcionará con el método fallback.

### ¿SumatraPDF es seguro?

Sí, es open source (GPLv3) y ampliamente usado. Puedes verificar el código fuente en: https://github.com/sumatrapdfreader/sumatrapdf

### ¿Funciona en otros sistemas operativos?

No, `PrintService` usa APIs específicas de Windows (`System.Drawing.Printing`). Para Linux/macOS se requeriría una implementación diferente.

---

## 📚 Referencias

- **SumatraPDF:** https://www.sumatrapdfreader.org/
- **Documentación CLI:** https://www.sumatrapdfreader.org/docs/Command-line-arguments
- **Código fuente:** https://github.com/sumatrapdfreader/sumatrapdf
