# Diseño de Interfaz - KeepPrinter

## 🎨 Visión General

Interfaz moderna y profesional para impresión por tandas, con enfoque en claridad del proceso y recuperabilidad.

**Nombre comercial mostrado:** Bunker Print Master  
**Tagline:** Impresión profesional por tandas

---

## 📐 Requisitos de Responsividad

- **Resolución mínima:** 1360x768px
- **Resolución máxima:** 4K (3840x2160px)
- **Escalado:** Adaptar controles y texto según DPI

---

## 🏗️ Estructura Principal

### Layout Base

```
┌─────────────────────────────────────────────────────────────────┐
│  [Stepper: Configurar → Preparar → Frente → Dorso → Confirmar] │ Header
├──────────┬─────────────────────────────────────────────────────┤
│          │                                                      │
│ Sidebar  │              Área Principal (3 columnas)            │
│ (220px)  │                                                      │
│          │                                                      │
├──────────┴─────────────────────────────────────────────────────┤
│                        Footer de Estado                         │ Footer
└─────────────────────────────────────────────────────────────────┘
```

---

## 1️⃣ Sidebar de Navegación (Izquierdo)

### Características:
- **Ancho fijo:** 220px
- **Color de fondo:** Navy oscuro (#1a2332 aprox.)
- **Posición:** Siempre visible (no colapsable en v1)

### Elementos:

#### Header del Sidebar
```
┌──────────────────────┐
│  🖨️  Bunker Print    │
│      Master          │
│  Impresión          │
│  profesional por    │
│  tandas             │
└──────────────────────┘
```

#### Menú de Navegación
```
┌──────────────────────┐
│  🏠  Inicio          │
│  📁  Explorador  [3] │ ← Badge con contador
│  👁️  Previsualización│
│  ⚙️  Proceso      [1]│ ← Item activo (fondo morado)
│  🕐  Historial       │
│  ⚙️  Configuración   │
└──────────────────────┘
```

**Estados visuales:**
- **Normal:** Texto blanco/gris claro
- **Hover:** Fondo semi-transparente
- **Activo:** Fondo morado (#6366f1), texto blanco, badge destacado

#### Footer del Sidebar
```
┌──────────────────────┐
│  🖨️  Impresora actual│
│      BROTHER_L2360D  │
│      🟢 Lista        │
│                      │
│  [Cambiar impresora] │
│                      │
│  Bunker Print Master │
│  v1.4.2          ℹ️   │
│                      │
│  🔵 Estado general   │
│     En proceso       │
└──────────────────────┘
```

---

## 2️⃣ Stepper Horizontal (Header Superior)

### Layout:
```
[✓ Configurar] ──── [✓ Preparar] ──── [③ Frente] ──── [④ Dorso] ──── [⑤ Confirmar]
 Impresora y        Generar tandas    Imprimiendo      Imprimir      Verificar y
 opciones                             frente           dorso         continuar
```

### Estados:
1. **Completado (✓):** Fondo verde, check blanco
2. **Activo (número en círculo azul):** Fondo azul, número blanco
3. **Pendiente (número en círculo gris):** Fondo gris claro, número gris oscuro

### Posición Controles (derecha del stepper):
```
⚙️ Configuración   ❓ Ayuda   [🛑 Detener proceso]
```
- Botón "Detener proceso": Rojo (#dc2626), hover más oscuro

---

## 3️⃣ Área Principal - Tres Columnas

### Distribución proporcional:
```
┌─────────────────────────────────────────────────────────┐
│  Col 1 (30%)  │  Col 2 (25%)  │  Col 3 (45%)           │
│  Explorador   │  Vista Previa │  Proceso por Tandas    │
└─────────────────────────────────────────────────────────┘
```

---

### Columna 1: Explorador de Documentos (30%)

#### A) Zona de Carga
```
┌──────────────────────────────┐
│          📤                  │
│  Arrastra y suelta PDFs aquí│
│                              │
│  o haz clic para seleccionar │
│                              │
│  [⬆️ Seleccionar PDF(s)]     │
└──────────────────────────────┘

3 documento(s) cargado(s)     Total: 469 páginas
```

#### B) Lista de Documentos
```
┌──────────────────────────────┐
│ 📄 Manual_Tecnico_2024.pdf   │
│    245 páginas · 12.4 MB     │  👁️  🗑️  ⌄
├──────────────────────────────┤
│ 📄 Catalogo_Productos.pdf    │
│    128 páginas · 8.7 MB      │  👁️  🗑️  ⌄
├──────────────────────────────┤
│ 📄 Instructivo_Instalacion   │
│    96 páginas · 5.1 MB       │  👁️  🗑️  ⌄
└──────────────────────────────┘

3 archivo(s)
```

**Acciones por documento:**
- 👁️ Ver/Previsualizar
- 🗑️ Eliminar
- ⌄ Expandir opciones

#### C) Configuración Inicial
```
┌──────────────────────────────┐
│ CONFIGURACIÓN INICIAL        │
│                              │
│ Página inicial               │
│ [1]                    ⬆️⬇️   │
│                              │
│ Hojas por tanda              │
│ [50]                   ⬆️⬇️   │
│                              │
│ ℹ️ Las tandas se generarán   │
│    en archivos separados     │
│    por frente y dorso cuando │
│    corresponda.              │
└──────────────────────────────┘
```

#### D) Estado de Impresora (Footer)
```
┌──────────────────────────────┐
│ 🖨️ Impresora                 │
│    BROTHER_L2360D_RAW        │
│                              │
│ [  Cambiar impresora  ]      │
└──────────────────────────────┘
```

---

### Columna 2: Vista Previa Rápida (25%)

```
┌──────────────────────────────────┐
│   VISTA PREVIA RÁPIDA            │
│                                  │
│  ┌────────────────────────┐      │
│  │                        │      │
│  │   [Thumbnail del PDF]  │      │
│  │   Manual Técnico       │      │
│  │   BUNKER 2024          │      │
│  │                        │      │
│  │   (Imagen de página)   │      │
│  │                        │      │
│  └────────────────────────┘      │
│                                  │
│  Manual_Tecnico_2024.pdf         │
│  Página 123 de 245               │
│                                  │
│  🔍 🔍+ ⛶                        │ Zoom in, out, fullscreen
│                                  │
│  Navegación                      │
│  [Primera] [Anterior]            │
│                                  │
│  123  / 245    [Siguiente]       │
│                                  │
│                 [Última]         │
└──────────────────────────────────┘
```

**Controles:**
- 🔍 Zoom out
- 🔍+ Zoom in  
- ⛶ Pantalla completa
- Navegación por páginas con input directo

---

### Columna 3: Proceso de Impresión por Tandas (45%)

#### A) Header de Tanda Actual
```
┌─────────────────────────────────────────────────────────────┐
│  PROCESO DE IMPRESIÓN POR TANDAS                            │
│                                                             │
│  Tanda actual          Páginas en tanda    Hojas  Documento│
│    3 / 10              101 - 150           50     Manual_...│
│    [FRENTE]            ← Badge morado                       │
└─────────────────────────────────────────────────────────────┘
```

#### B) Grid de Páginas con Estados
```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  ◀  101   102   103   ...   148   149   150   ▶            │
│      ✓     ✓     ✓           ✓     ✓     ✓                │
│     🟢    🟢    🟢          🟢    🟢    🟢               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Estados visuales:**
- 🟢 **Impreso:** Círculo verde con checkmark
- ⚪ **Pendiente:** Círculo gris vacío
- 🔵 **En proceso:** Círculo azul pulsante
- ❌ **Error:** Círculo rojo con X

#### C) Progreso General
```
┌─────────────────────────────────────────────────────────────┐
│  Progreso general                     Tiempo transcurrido   │
│  ████████████░░░░░░░░░  30%          ⏱️ 00:12:45          │
└─────────────────────────────────────────────────────────────┘
```

#### D) Estado Actual
```
┌─────────────────────────────────────────────────────────────┐
│  Estado actual                                              │
│  ⏳ Imprimiendo frente... Página 123 de 150                 │
│                                                             │
│  [⏸️ Pausar]           [🛑 Detener y revisar]              │
└─────────────────────────────────────────────────────────────┘
```

#### E) Pasos del Proceso (Cards)
```
┌──────────────┬──────────────┬──────────────┬──────────────┐
│ ① Preparar  │ 🖨️ Imprimir  │ 🖨️ Imprimir  │ ✓ Confirmar  │
│    tandas   │    frente    │    dorso     │    tanda     │
├──────────────┼──────────────┼──────────────┼──────────────┤
│ Genera los  │ Imprime todas│ Imprime todas│ Verifica que │
│ archivos de │ las tandas   │ las tandas   │ la tanda se  │
│ frente y    │ de frente.   │ de dorso.    │ haya impreso │
│ dorso por   │              │              │ correctamente│
│ tanda.      │              │              │              │
├──────────────┼──────────────┼──────────────┼──────────────┤
│ ✅ Completado│ ⚙️ En progreso│ ⏳ Pendiente │ ⏳ Pendiente │
└──────────────┴──────────────┴──────────────┴──────────────┘

┌──────────────┐
│ 🔄 Continuar │
│              │
│ Pasa a la    │
│ siguiente    │
│ tanda y      │
│ repite el    │
│ proceso.     │
│              │
│ ⏳ Pendiente │
└──────────────┘
```

---

## 4️⃣ Footer de Estado (Inferior)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ 🔵 Estado general │ 🖨️ Impresora    │ 📄 Documento actual │ 🗂️ Tanda actual│
│    En proceso     │   BROTHER_L2360 │   Manual_Tecnico... │   3 de 10      │
│                   │                 │                     │   (frentes)    │
├───────────────────┴─────────────────┴─────────────────────┴────────────────┤
│ 📄 Páginas impresas │ ⏱️ Tiempo estimado                                   │
│    122 de 469       │    00:28:15 restante                                 │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Paleta de Colores

### Colores Principales
```css
--color-primary: #6366f1;        /* Índigo - Activo */
--color-success: #10b981;        /* Verde - Completado */
--color-warning: #f59e0b;        /* Ámbar - Advertencia */
--color-error: #dc2626;          /* Rojo - Error/Detener */
--color-info: #3b82f6;           /* Azul - Información */
```

### Colores de Fondo
```css
--bg-sidebar: #1a2332;           /* Navy oscuro */
--bg-sidebar-active: #6366f1;    /* Morado para item activo */
--bg-main: #f9fafb;              /* Gris muy claro */
--bg-card: #ffffff;              /* Blanco */
--bg-hover: rgba(99, 102, 241, 0.1); /* Hover sutil */
```

### Colores de Texto
```css
--text-primary: #111827;         /* Casi negro */
--text-secondary: #6b7280;       /* Gris medio */
--text-sidebar: #e5e7eb;         /* Gris claro para sidebar */
--text-sidebar-active: #ffffff;  /* Blanco para item activo */
```

### Estados de Páginas
```css
--page-printed: #10b981;         /* Verde - Impreso ✓ */
--page-pending: #d1d5db;         /* Gris - Pendiente */
--page-current: #3b82f6;         /* Azul - En proceso */
--page-error: #dc2626;           /* Rojo - Error */
```

---

## 🖼️ Componentes Reutilizables

### 1. Badge de Notificación
```
[ 3 ]  ← Círculo pequeño con número
```
- Fondo: Color de notificación (#f59e0b o #dc2626)
- Texto: Blanco
- Posición: Superior derecha del item de menú

### 2. Botón Primario
```css
background: #6366f1;
color: white;
border-radius: 6px;
padding: 8px 16px;
hover: background #4f46e5;
```

### 3. Botón Secundario
```css
background: white;
border: 1px solid #d1d5db;
color: #374151;
border-radius: 6px;
padding: 8px 16px;
hover: background #f9fafb;
```

### 4. Botón Peligro
```css
background: #dc2626;
color: white;
border-radius: 6px;
padding: 8px 16px;
hover: background #b91c1c;
```

### 5. Card de Paso del Proceso
```css
background: white;
border: 1px solid #e5e7eb;
border-radius: 8px;
padding: 16px;
box-shadow: 0 1px 3px rgba(0,0,0,0.1);
```

**Estados:**
- **Completado:** Border verde, icono ✅
- **En progreso:** Border azul, icono ⚙️ animado
- **Pendiente:** Border gris, icono ⏳

### 6. Círculo de Estado de Página
```css
width: 36px;
height: 36px;
border-radius: 50%;
display: flex;
align-items: center;
justify-content: center;
```

**Variantes:**
- Impreso: `background: #10b981; content: '✓'`
- Pendiente: `background: #d1d5db`
- Actual: `background: #3b82f6; animation: pulse`
- Error: `background: #dc2626; content: '✕'`

---

## 📱 Comportamiento Responsive

### Resolución 1360x768px (mínimo)
- Sidebar: 200px fijo
- Grid de páginas: 2 filas x 5 columnas
- Vista previa: Más pequeña pero visible
- Footer: Layout horizontal compacto

### Resolución 1920x1080px (estándar)
- Sidebar: 220px
- Grid de páginas: 2 filas x 7 columnas
- Vista previa: Tamaño medio
- Footer: Layout horizontal normal

### Resolución 4K (3840x2160px)
- Sidebar: 240px con iconos más grandes
- Grid de páginas: 3 filas x 10 columnas
- Vista previa: Grande con más detalle
- Footer: Layout horizontal con más espacio
- Fuentes: Escalado 1.5x para legibilidad

### Breakpoints sugeridos
```css
/* Mínimo */
@media (min-width: 1360px) { ... }

/* Estándar */
@media (min-width: 1920px) { ... }

/* 2K */
@media (min-width: 2560px) { ... }

/* 4K */
@media (min-width: 3840px) { ... }
```

---

## 🔄 Flujos de Interacción

### Flujo Principal (Usuario)

1. **Inicio / Explorador**
   - Arrastrar PDF(s) o seleccionar
   - Configurar página inicial y hojas por tanda
   - Ver lista de documentos cargados

2. **Proceso → Preparar tandas**
   - Click en "Preparar tandas"
   - Sistema genera PDFs de frente/dorso
   - Muestra progreso de generación
   - Card "Preparar tandas" → ✅ Completado

3. **Proceso → Imprimir frente**
   - Click en "Imprimir frente"
   - Sistema envía tanda actual (frente) a impresora
   - Grid muestra páginas marcándose como impresas ✓
   - Progreso general se actualiza
   - Card "Imprimir frente" → ⚙️ En progreso

4. **Proceso → Imprimir dorso**
   - Una vez completado frente, click "Imprimir dorso"
   - Usuario voltea hojas manualmente
   - Sistema envía tanda actual (dorso) a impresora
   - Card "Imprimir dorso" → ⚙️ En progreso

5. **Proceso → Confirmar tanda**
   - Click en "Confirmar tanda"
   - Modal pregunta: "¿La tanda se imprimió correctamente?"
   - Si SÍ: avanza a siguiente tanda, repite desde paso 3
   - Si NO: permite rehacer frente o dorso
   - Card "Confirmar tanda" → ✅ Completado

6. **Proceso → Continuar (siguiente tanda)**
   - Click en "Continuar"
   - Avanza CurrentBatchIndex++
   - Vuelve a paso 3 hasta terminar todas las tandas

7. **Finalización**
   - Stepper muestra "Confirmar" como activo
   - Modal de éxito: "¡Impresión completada!"
   - Opción de ver historial o iniciar nuevo trabajo

### Flujo de Pausa/Reanudación

1. **Pausar**
   - Click en "Pausar"
   - Sistema guarda estado actual (SessionStore)
   - Botón cambia a "▶️ Reanudar"

2. **Detener y revisar**
   - Click en "Detener y revisar"
   - Modal de confirmación
   - Sistema guarda estado y vuelve a vista de configuración
   - Permite ajustar parámetros si es necesario

3. **Reanudar (al abrir app)**
   - Si hay sesión pendiente, modal: "¿Continuar último trabajo?"
   - SI: Carga sesión y va a la tanda/cara pendiente
   - NO: Inicia nuevo trabajo

### Flujo de Errores

1. **Error de impresión**
   - Página en grid se marca con ❌ rojo
   - Notificación: "Error al imprimir página X"
   - Botones: "Reintentar página" | "Continuar sin esta página"

2. **Atasco/problema físico**
   - Usuario hace click "Pausar"
   - Corrige problema físico
   - Click "Reanudar" (sistema retoma desde última página confirmada)

---

## 🧩 Componentes WinUI 3 Sugeridos

### Navegación
- **NavigationView** (para sidebar con menú)
- **NavigationViewItem** (items del menú)
- **Breadcrumb** (opcional para navegación)

### Layout
- **Grid** (layout principal de 3 columnas)
- **StackPanel** (listas verticales)
- **WrapGrid** (grid de páginas)

### Controles
- **Button** (acciones principales)
- **ProgressBar** (progreso general)
- **ProgressRing** (carga de PDF, generación de tandas)
- **NumberBox** (página inicial, hojas por tanda)
- **ComboBox** (selector de impresora)
- **InfoBadge** (contador en menú: "3", "1")

### Presentación
- **Card** (contenedores de pasos del proceso)
- **Expander** (detalles de documentos)
- **TeachingTip** (tooltips informativos)
- **ContentDialog** (modales de confirmación)

### Visualización
- **ListView** (lista de documentos)
- **ItemsRepeater** (grid de páginas)
- **Image** (thumbnail de PDF)
- **ScrollViewer** (scroll en listas largas)

### Feedback
- **InfoBar** (notificaciones de estado: éxito, advertencia, error)
- **ToolTip** (ayuda contextual)

---

## 🎭 Animaciones y Transiciones

### Micro-interacciones
- **Hover en botones:** Cambio de color + elevación ligera
- **Click en botón:** Escala 0.98 momentánea
- **Página impresa:** Fade in del checkmark ✓
- **Progreso:** Barra animada con transición suave

### Transiciones de estado
- **Cambio de tanda:** Fade out → fade in del contenido
- **Cambio de etapa en stepper:** Transición de color + checkmark
- **Carga de PDF:** Skeleton loader o shimmer effect

### Estados de carga
- **Generando tandas:** ProgressRing animado
- **Imprimiendo:** ProgressBar con pulso
- **Procesando PDF:** Shimmer en thumbnail

---

## ♿ Accesibilidad

### Navegación por teclado
- **Tab:** Navegar entre controles
- **Enter:** Activar botón seleccionado
- **Flechas:** Navegar en menú y grid de páginas
- **Espacio:** Seleccionar/activar

### Lectores de pantalla
- Labels descriptivos en todos los controles
- Roles ARIA apropiados
- Anuncios de cambio de estado

### Alto contraste
- Soporte para tema de alto contraste de Windows
- Iconos visibles en todos los modos
- Borders visibles en estados de focus

---

## 📚 Referencias de Implementación

### Fase 4 (UI y ViewModels)
Este diseño se implementará principalmente en la **Fase 4** con:

1. **MainWindow.xaml:**
   - NavigationView con sidebar
   - Grid de 3 columnas
   - Footer de estado

2. **Views/ProcessView.xaml:**
   - Stepper horizontal
   - Grid de páginas con binding
   - Cards de pasos del proceso

3. **ViewModels/ProcessViewModel.cs:**
   - Propiedades observables para estados
   - Comandos para acciones (imprimir, pausar, continuar)
   - Lógica de transición de estados

4. **Styles/:**
   - Colores en ResourceDictionary
   - Estilos de botones
   - Templates de cards

---

## 🎯 Prioridades de Implementación

### MVP (Versión 1.0)
- ✅ Layout base con 3 columnas
- ✅ Stepper horizontal funcional
- ✅ Grid de páginas con estados visuales
- ✅ Controles principales (imprimir, pausar, continuar)
- ✅ Footer de estado

### Mejoras Futuras (v1.1+)
- 🔄 Drag & drop de PDFs
- 🔄 Vista previa avanzada del PDF
- 🔄 Historial de trabajos
- 🔄 Configuración avanzada (orden de impresión, márgenes)
- 🔄 Temas claro/oscuro switcheable

---

**Documento de referencia para implementación en Fases 4 y 5**
