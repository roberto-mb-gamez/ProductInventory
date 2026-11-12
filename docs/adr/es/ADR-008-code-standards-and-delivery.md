# ADR-008 — Estándares de código y entrega de la prueba
**Estado:** Aceptado

## Decisión
- Todo el código fuente, identificadores y comentarios deben estar en inglés; los mensajes mostrados al usuario pueden estar en español.
- Respetar la regla exacta de espacios en blanco de la prueba: no dejar líneas vacías entre líneas de código, excepto una línea antes de cada `return` cuando sea sintácticamente aplicable. Limitar esta exigencia al código fuente escrito para evitar conflictos con código generado.
- Usar nombres de métodos descriptivos; una responsabilidad por método, normalmente no más de 25 líneas; evitar anidamiento profundo de flujo de control.
- Una clase o enumeración por archivo de código fuente. No mantener código comentado. Preferir objetos de parámetros frente a listas extensas de argumentos primitivos.
- Leer la configuración de ejecución desde variables de entorno y no publicar secretos.
- Entregar un repositorio público que incluya Dockerfile, Compose, migraciones o archivos de inicialización, Swagger y un README detallado para configurar, ejecutar, probar y depurar localmente.
- Utilizar un flujo Git que conserve las especificaciones, ADR y resultados de pruebas aprobados. No afirmar que una validación se ejecutó si no ocurrió.

## Consecuencias
Estas reglas son específicas de la prueba, incluso cuando son más estrictas que las convenciones habituales de formato en .NET.
