# Revisar resultados de una etapa

## Prompt para pegar en Codex

Realiza una auditoría de solo lectura de la etapa activa. Contrasta tareas marcadas, archivos reales, git diff, pruebas y evidencias. Indica criterios PASSED, FAILED, BLOCKED o NOT_RUN. No cambies la etapa a APPROVED ni avances. Dame un resumen de riesgos, problemas y recomendación de aprobación.

## Formato de respuesta requerido
1. Etapa y estado verificado.
2. Tareas realizadas y archivos cambiados.
3. Pruebas/comandos realmente ejecutados y resultados.
4. Bloqueos o decisiones que necesitan autorización.
5. Archivos de seguimiento actualizados y siguiente paso.

**Instrucción permanente:** El código, los identificadores y los comentarios del proyecto deben permanecer en inglés. Puedes explicarme los resultados en español.
