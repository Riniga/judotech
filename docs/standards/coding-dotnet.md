# Coding Standard — C# / .NET

Applies to `source/judotech.core`, `source/judotech.api` and their test projects.
Read [`coding.md`](coding.md) first for the language-neutral principles.

`source/` is maintenance-only (ADR-0001). Apply this standard fully to new or
changed code; clean up surrounding code only when you are already editing it.

## Target and language

- Target framework: `net8.0`.
- `Nullable` enabled; `ImplicitUsings` enabled; `LangVersion` `latest`.
- Pin the SDK via `source/global.json`.
- Shared build properties live in `source/Directory.Build.props`.

## Naming

- Types, methods, properties, events, namespaces: `PascalCase`.
- Local variables and parameters: `camelCase`.
- Private fields: `_camelCase`.
- Constants: `PascalCase`.
- One top-level type per file; file name matches the type.
- File-scoped namespaces (`namespace judotech.api;`).

## Style

- Formatting is enforced by `.editorconfig`; run `dotnet format` before commit.
- `using` directives outside the namespace, sorted, unused ones removed.
- Prefer `var` when the type is obvious from the right-hand side.
- Expression-bodied members for one-liners; block bodies otherwise.
- Async methods return `Task` / `Task<T>` and end in `Async`. Do not block on
  async code with `.Result` or `.Wait()` in new code (existing uses are tracked
  as TD-002).

## Structure

- Keep domain logic separable from data access. New persistence code should sit
  behind an abstraction (as `DatabaseBase` already does) rather than being called
  directly from Functions.
- Azure Functions (`[Function]` methods) are thin: parse and validate the
  request, call into `judotech.core`, shape the response. No business rules in
  the Function body.
- Do not read configuration (`Environment.GetEnvironmentVariable`) deep in the
  data layer for new code; pass it in.

## Error handling and logging

- Use the injected `ILogger<T>` in Functions; use structured message templates,
  not string concatenation.
- Never log request bodies, passwords or tokens (see TD-034).
- Catch specific exceptions; never `catch (Exception) { }` with an empty body.

## Security

- Build Cosmos queries with `QueryDefinition` parameters
  (`.WithParameter("@token", token)`), never string concatenation (see TD-033).
- Never return password hashes from an API endpoint (see TD-038).
- Validate every query-string and body input before use.

## Tests

- xUnit, project `<name>.tests`, files `<Type>Tests.cs`. See
  [`testing.md`](testing.md).
- Tests that need Cosmos DB are `[Trait("Category", "Integration")]` and are not
  run in CI.

## Known deviations

Existing code does not fully follow this standard. The gaps are recorded in
[`../architecture/technical-debt.md`](../architecture/technical-debt.md)
(TD-002, TD-003, TD-032–TD-039). Do not expand them; reduce them when you touch
the surrounding code.
