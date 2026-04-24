# Plan técnico — KeepPrinter (WinUI 3)

## Visión

Cliente Windows **.NET 8 + WinUI 3**, MVVM, capas separadas. Prioridad: **modelo de sesión con puntos de reanudación** además del pipeline PDF frente/dorso ya conocido en flujos por tandas.

## Alcance funcional v1

| Área | Descripción |
|------|-------------|
| Entrada | Un PDF por trabajo; validar ruta y total de páginas. |
| Parámetros | Página inicial, hojas por tanda; validación frente a total. |
| Tandas | Generación de PDFs por tanda (frente impar / dorso par, orden coherente con reglas validadas en prototipos previos). |
| Impresoras | Enumeración Windows (local + conexiones); impresión vía estrategia nativa + fallback documentado (p. ej. SumatraPDF) si aplica. |
| Flujo UI | Estados: sin preparar → preparado → frente tanda N → dorso tanda N → confirmar → siguiente / fin. |
| **Reanudación** | Persistir estado de sesión (archivo local en `ApplicationData` o JSON junto a la carpeta de salida): documento, carpeta de salida, lista de tandas, **tanda actual**, **etapa** (`PendienteFrente` / `PendienteDorso` / `TandaCompleta`). Al abrir la app, ofrecer *Continuar último trabajo* o *Nuevo trabajo*. |

## Arquitectura de proyectos

```text
keepPrinter/
  src/
    KeepPrinter.App/           # WinUI 3
    KeepPrinter.Core/          # Dominio, estados, validación, contratos
    KeepPrinter.Infrastructure/# PDF, disco, Win32 impresión, persistencia sesión
    KeepPrinter.ViewModels/
  tests/
    KeepPrinter.Core.Tests/
  docs/
```

## Fases

1. **F0** — Solución vacía WinUI + class libraries + DI + `.editorconfig`.
2. **F1** — Modelos `PrintSession`, `TandaInfo`, `WorkflowStage`; validación pura; tests.
3. **F2** — Servicio PDF (generar tandas en carpeta `FIX_*` o convención KeepPrinter); tests con PDF pequeño.
4. **F3** — `ISessionStore`: guardar/cargar sesión; pruebas manuales de “cerrar app y volver”.
5. **F4** — UI: flujo principal + indicador de progreso + “Continuar”.
6. **F5** — Empaquetado, icono, README usuario final.

## Riesgos

- **Sincronización frente/dorso:** la confirmación explícita por tanda mitiga desalineación; no avanzar de tanda sin confirmación del usuario.
- **Drivers RAW vs PDF:** documentar impresoras “RAW” y rutas de fallback.

## Próximo paso en máquina de desarrollo

Crear la solución desde Visual Studio 2022 (**Aplicación WinUI 3 en blanco**) o plantillas `dotnet` si están instaladas; añadir proyectos `Core`, `Infrastructure`, `ViewModels` y referencias como en el árbol anterior.
