# Coding Standard

This is the primary development guideline for coding conventions used across the workspace.

Correctness is mandatory. Clarity, structure, and maintainability take priority over speed and cleverness.
Unnecessary complexity is never acceptable.

## Language

- English for all code: variable names, function names, class names, comments, documentation, and commit messages.

## Naming

- Functions and variables: `snake_case`
- Classes: `PascalCase`
- Files: `snake_case.py`
- Constants: `UPPER_SNAKE_CASE`

## Core Principles

- Follow the **SOLID** principles whenever they improve maintainability:
  - **S** – Single Responsibility: one class, one module, one function, one purpose.
  - **O** – Open/Closed: extend existing code rather than modifying stable behavior.
  - **L** – Liskov Substitution: derived implementations must behave as their base contracts.
  - **I** – Interface Segregation: prefer small, focused interfaces over large, general ones.
  - **D** – Dependency Inversion: depend on abstractions, not concrete implementations.
- Prefer simple, readable and maintainable code over clever solutions.
- Separate logic that can be independently understood, tested or reused.
- Eliminate duplication only when it is real, recurring and improves maintainability.
- Write unit tests for reusable logic and non-trivial business rules.
- Do not introduce abstractions or complexity before they are needed.
- Refactor opportunistically: leave the code cleaner than you found it.
- Every public class and function must include a concise docstring.
- Write comments to explain **why**, never **what** the code does.
- Do not leave `TODO` comments without a corresponding MVP or implementation plan.

## Typing

- Type hints are required for all function signatures.
- Use `T | None` instead of `Optional[T]` where appropriate.
- Always declare a return type, including `-> None`.

## Imports

- Import order: standard library → third-party → local modules.
- Remove unused imports.

## Error Handling

- Never use `except: pass` or silently ignore exceptions.
- Log unexpected exceptions with sufficient context.
- Protect all division operations against zero denominators.
- Domain-specific errors should be implemented as custom exception classes.

## Project Structure

Each domain should follow a consistent structure where applicable:

```text
<domain>/
    service.py
    repository.py
    schemas.py
    exceptions.py
```

- Business logic belongs in `service.py`.
- Data access belongs in `repository.py`.

## Security

- Never log passwords, API keys, tokens, cookies, or secrets.
- Validate and authorize all external requests.
- Protect all state-changing web endpoints against CSRF where applicable.

## Dependencies

- Manage dependencies through `pyproject.toml`.
- Pin compatible version ranges.
- Document significant new dependencies in an ADR.

## Testing

- New functionality should include automated tests.
- Bug fixes should include a regression test whenever practical.

## General Principles

- Keep functions small and focused.
- Prefer composition over duplication.
- Favor readability over cleverness.
- Refactor before complexity grows.
- Leave the codebase cleaner than you found it.
