# Filosofía de KeepPrinter

## Problema que resolvemos

En muchos entornos (oficina, taller, imprenta ligera) la impresora **no imprime automáticamente las dos caras**. El operario:

1. Imprime un bloque de **frentes**.
2. Retira el taco, lo invierte y recoloca según la impresora.
3. Imprime el bloque de **dorsos** alineado con esos frentes.

Si el documento tiene cientos de páginas, se trabaja por **tandas** (por ejemplo 50 hojas lógicas por bloque). Si en la tanda 3 falla una hoja, lo razonable es **reanudar desde ahí** (o desde la cara que falta), no reimprimir todo el PDF.

## Principios de diseño

### 1. Reanudación explícita

- La aplicación debe recordar (en disco o en sesión clara) al menos:
  - documento de trabajo (ruta o identificador),
  - parámetros de tanda (página inicial, hojas por tanda),
  - **índice de tanda actual** y **fase** (frente impreso / pendiente dorso / tanda confirmada).
- Tras un error, el usuario debe poder decir: *“sigo en la tanda 3, me falta el dorso”* sin reprocesar todo.

### 2. Tandas como unidad de trabajo

- Cada tanda corresponde a un rango de páginas del PDF original y genera uno o dos PDFs auxiliares (frente / dorso) según la lógica de paginación.
- El usuario confirma explícitamente cuando una tanda **salió bien** antes de avanzar; eso evita desalineación silenciosa frente/dorso.

### 3. Mínima sorpresa en la impresora

- Orden de páginas dentro de cada PDF auxiliar debe ser **predecible** y documentado (coherente con lo que ya validaste en flujos manuales existentes).
- Listado de impresoras = el del sistema; nada de nombres inventados salvo fallback de emergencia.

### 4. Honestidad ante el fallo

- Si no hay duplex, el software **no** puede garantizar el reverso solo: el usuario es parte del proceso. La UI debe guiar, no ocultar pasos.

## Qué no pretende ser KeepPrinter

- Un sustituto genérico del cuadro de impresión de Windows para todo tipo de trabajos.
- Un visor PDF completo (salvo lo mínimo para comprobar rangos, si en el futuro se añade).
- Una solución cloud obligatoria: el foco es **máquina local** y control del operario.

## Relación con otros proyectos

KeepPrinter nace con la misma **intención práctica** que flujos previos de impresión por tandas en otros repos, pero con **nombre propio**, **repo independiente** y **reanudación** como requisito de primera clase, no como añadido tardío.
