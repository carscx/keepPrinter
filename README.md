# KeepPrinter

Aplicación de escritorio para **imprimir documentos grandes en impresoras que no tienen duplex automático**, trabajando por **tandas** (frente y dorso por separado) y permitiendo **reanudar** si una hoja falla o se interrumpe el trabajo.

## Para quién es

- Impresoras **solo simplex** (una cara por pasada): hay que imprimir primero un conjunto de frentes, volver las hojas, y luego los dorsos.
- Documentos **largos**: conviene trocear el PDF en tandas manejables.
- Situaciones donde **cualquier hoja puede fallar**: atasco, toner, corte de luz — no queremos reimprimir desde la página 1.

## Filosofía

1. **El progreso es sagrado.** El usuario debe poder cerrar la app, corregir un atasco y **seguir desde la última tanda o cara coherente**, no adivinar por dónde iba.
2. **Un flujo claro:** preparar tandas → imprimir frente de la tanda actual → imprimir dorso → confirmar → siguiente. Sin magia oculta.
3. **Transparencia:** siempre visible qué rango de páginas corresponde a la tanda actual y qué archivos se van a enviar a la impresora.
4. **Nativo en Windows:** listado de impresoras del sistema e integración con el flujo de impresión que ya conocen los usuarios.

## Objetivo del producto

Ofrecer una herramienta **fiable y recuperable** para el ciclo manual frente/dorso en impresoras no duplex, reduciendo estrés y papel basura cuando algo sale mal a mitad de un documento.

## Estado del proyecto

Repositorio recién inicializado. El plan técnico (WinUI 3, .NET) y el alcance detallado están en [`docs/PLAN.md`](docs/PLAN.md). La filosofía ampliada en [`docs/PHILOSOPHY.md`](docs/PHILOSOPHY.md).

## Licencia

Por definir (añadir `LICENSE` cuando lo decidas).
