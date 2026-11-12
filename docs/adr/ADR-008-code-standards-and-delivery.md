# ADR-008 — Assessment code standards and delivery
Status: Accepted

## Decision
- All source code, identifiers and comments in English; user-facing response messages may be Spanish.
- Follow the assessment's exact whitespace rule: no blank lines between code lines, except one blank line before each `return` (where syntactically applicable). Avoid conflicts with generated code by limiting enforcement to authored source files.
- Descriptive method names; one responsibility per method, normally at most 25 lines; avoid deeply nested control flow.
- One class or enum per source file. No commented-out code. Prefer parameter objects to long primitive argument lists.
- Read configurable runtime values from environment variables; avoid committed secrets.
- Publish a public source repository including Dockerfile, Compose, migrations/init assets, Swagger and a detailed README for setup, running, testing and local debugging.
- Use a Git workflow that preserves the approved specs, ADRs and test results. No claimed validation without execution.

## Consequences
These rules are assessment-specific, even where stricter than ordinary .NET formatting conventions.
