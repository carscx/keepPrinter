# 📋 ViewModels - Pendientes de Corrección

## ✅ Creado en esta Sesión

### **ViewModels implementados:**
1. ✅ `MainViewModel.cs` - Navegación y estado de sesión
2. ✅ `SetupViewModel.cs` - Configuración inicial (selección PDF, parámetros)
3. ✅ `BatchProgressViewModel.cs` - Progreso de impresión de tandas
4. ✅ `CompletionViewModel.cs` - Pantalla de finalización

### **Paquetes agregados:**
- ✅ `CommunityToolkit.Mvvm 8.4.0` (ViewModels + App)
- ✅ Referencia a Infrastructure en ViewModels

### **TargetFramework actualizado:**
- ✅ `KeepPrinter.ViewModels`: `net8.0-windows10.0.19041.0`
- ✅ `KeepPrinter.Infrastructure.Tests`: `net8.0-windows10.0.19041.0`
- ✅ Ambos con `<UseWinRT>true</UseWinRT>`

---

## ⚠️ Errores de Compilación a Corregir (15-20 min)

### **1. SetupViewModel.cs**

**Línea 181:** Cambiar `SessionId` → `Id`
```csharp
// ❌ Actual:
SessionId = Guid.NewGuid(),

// ✅ Correcto:
Id = Guid.NewGuid(),
```

**Línea 192:** Cambiar método de validación
```csharp
// ❌ Actual:
var validation = PrintSessionValidator.ValidateSessionParameters(
    session.SourcePdfPath,
    session.StartPage,
    session.TotalPages,
    session.SheetsPerBatch);

// ✅ Correcto:
var validation = PrintSessionValidator.ValidateBatchParameters(
    session.SourcePdfPath,
    session.StartPage,
    session.TotalPages,
    session.SheetsPerBatch);
```

**Línea 217:** Cambiar `WorkflowStage.Ready` → `WorkflowStage.Prepared`
```csharp
// ❌ Actual:
session.CurrentStage = WorkflowStage.Ready;

// ✅ Correcto:
session.CurrentStage = WorkflowStage.Prepared;
```

**Líneas 68 y 137:** Resolver referencia a `App.MainWindow`
```csharp
// ❌ Actual:
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

// ✅ Solución: Pasar ventana como parámetro al ViewModel
// O usar Application.Current.MainWindow (requiere cast)
```

---

### **2. BatchProgressViewModel.cs**

**Línea 111:** Cambiar `WorkflowStage.Ready` → `WorkflowStage.Prepared`
```csharp
// ❌ Actual:
if (_session.CurrentStage == WorkflowStage.Ready)

// ✅ Correcto:
if (_session.CurrentStage == WorkflowStage.Prepared)
```

**Línea 187:** Cambiar `WorkflowStage.BatchCompleted` → `WorkflowStage.Completed`
```csharp
// ❌ Actual:
_session.CurrentStage = WorkflowStage.BatchCompleted;

// ✅ Correcto:
_session.CurrentStage = WorkflowStage.Completed;
```

**Línea 194:** Cambiar `WorkflowStage.Ready` → `WorkflowStage.Prepared`
```csharp
// ❌ Actual:
_session.CurrentStage = WorkflowStage.Ready;

// ✅ Correcto:
_session.CurrentStage = WorkflowStage.Prepared;
```

**Líneas 206 y 235:** Hacer métodos públicos en MainViewModel
```csharp
// En MainViewModel.cs, cambiar estos métodos de private a public:

// ❌ Actual:
[RelayCommand]
private void NavigateToCompletion()

[RelayCommand]
private void Restart()

// ✅ Correcto:
[RelayCommand]
public void NavigateToCompletion()

[RelayCommand]
public void Restart()
```

---

### **3. CompletionViewModel.cs**

**Línea 67:** Hacer método público en MainViewModel
```csharp
// Ya cubierto en punto anterior (MainViewModel.Restart)
```

**Línea 76:** Resolver referencia a `App.Current`
```csharp
// ❌ Actual:
App.Current.Exit();

// ✅ Correcto:
Application.Current.Exit();
// (Agregar using Microsoft.UI.Xaml;)
```

---

### **4. MainViewModel.cs**

**Hacer públicos estos métodos** (para que BatchProgressViewModel y CompletionViewModel puedan llamarlos):
```csharp
// Cambiar de private a public:
[RelayCommand]
public void NavigateToCompletion() // Ya es private, cambiar

[RelayCommand]
public void Restart() // Ya es private, cambiar
```

---

## 📝 Tareas Pendientes para Próxima Sesión

### **Paso 1: Corregir Errores de Compilación** (15-20 min)
1. ✅ Aplicar todas las correcciones listadas arriba
2. ✅ Verificar que `dotnet build` sea exitoso
3. ✅ Ejecutar tests (deben seguir 62/62 pasando)

### **Paso 2: Crear Páginas XAML** (2-3 horas)

#### **SetupPage.xaml**
- FilePicker para seleccionar PDF
- TextBox para mostrar info del PDF (páginas)
- NumberBox para configurar hojas por tanda
- FolderPicker para carpeta de salida
- Botón "Iniciar Sesión"
- ProgressRing mientras procesa

#### **BatchProgressPage.xaml**
- ComboBox con impresoras disponibles
- Información de tanda actual (N de M)
- Grid con estado de páginas
- Botones:
  - "Imprimir Frente"
  - "Imprimir Dorso"
  - "Confirmar Tanda Completa"
  - "Abrir Carpeta"
  - "Cancelar"
- ProgressBar de progreso general
- StatusBar con mensajes

#### **CompletionPage.xaml**
- Resumen de sesión:
  - Archivo procesado
  - Total de páginas
  - Tandas completadas
  - Duración
- Botones:
  - "Abrir Carpeta de Salida"
  - "Nueva Sesión"
  - "Salir"

### **Paso 3: Configurar DI en App.xaml.cs** (30 min)
```csharp
// App.xaml.cs
public partial class App : Application
{
    public static Window? MainWindow { get; private set; }
    private IServiceProvider? _serviceProvider;

    public App()
    {
        InitializeComponent();
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Registrar servicios de Infrastructure
        services.AddInfrastructureServices();

        // Registrar ViewModels
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var mainViewModel = _serviceProvider!.GetRequiredService<MainViewModel>();

        MainWindow = new MainWindow
        {
            Content = new MainPage { DataContext = mainViewModel }
        };

        MainWindow.Activate();
    }
}
```

### **Paso 4: Crear MainPage con Navegación** (1 hora)
```xaml
<!-- MainPage.xaml -->
<Page>
    <Grid>
        <ContentControl Content="{Binding CurrentView}" />
    </Grid>
</Page>
```

Con DataTemplates para cada ViewModel → View.

---

## 🎯 Resumen Ejecutivo

### **Estado Actual:**
- ✅ Infraestructura completa (PdfService + PrintService nativo)
- ✅ 62 tests pasando
- ✅ ViewModels creados (con errores de compilación menores)
- ❌ UI pendiente de implementar

### **Próxima Sesión (Estimado: 4-5 horas):**
1. Corregir errores (15-20 min)
2. Crear 3 páginas XAML (2-3 h)
3. Configurar DI (30 min)
4. Probar flujo end-to-end (1 h)

### **Resultado Final:**
🎉 **Aplicación funcional completa** que permite:
- Seleccionar PDF
- Configurar tandas
- Generar PDFs de frente/dorso
- Imprimir con 300 DPI nativo
- Ver progreso en tiempo real
- Todo in-memory (sin persistencia por ahora)

---

## 📚 Referencias

- **Diseño UI:** `docs/UI_DESIGN.md` (responsive, colores, componentes)
- **Filosofía:** `docs/PHILOSOPHY.md` (KISS, progreso sagrado)
- **Plan:** `docs/PLAN.md` (roadmap completo)
- **Persistencia:** `docs/PERSISTENCIA_ESTRATEGIA.md` (análisis de opciones)

---

## 🔧 Comandos Útiles

```powershell
# Compilar
dotnet build KeepPrinter.sln

# Ejecutar tests
dotnet test KeepPrinter.sln

# Ejecutar app (desde Visual Studio F5 en KeepPrinter.App)
```

---

**Nota:** Este documento se convertirá en `FASE_3_VIEWMODELS_PENDIENTES.md` después del commit.
