# Coding Standard

The primary development guideline for the workspace. Language-neutral principles
live here; concrete rules for each stack live in the per-language standards
linked at the bottom.

Correctness is mandatory. Clarity, structure and maintainability take priority
over speed and cleverness. Unnecessary complexity is never acceptable.

## Language

- English for all code: identifiers, comments, developer-facing log and error
  messages, commit messages, and documentation.
- See [`../architecture/decisions/0003-language-policy.md`](../architecture/decisions/0003-language-policy.md)
  for what may stay in another language (domain data, user-facing UI copy).

## Core principles

- Follow **SOLID** where it improves maintainability — not as a goal in itself:
  - **S** – one class / module / function, one purpose.
  - **O** – extend rather than modify stable behaviour.
  - **L** – a subtype must honour its base contract.
  - **I** – prefer small, focused interfaces.
  - **D** – depend on abstractions, not concrete implementations.
- Prefer simple, readable code over clever code.
- Separate logic that can be understood, tested or reused independently.
- Remove duplication only when it is real, recurring, and removing it improves
  maintainability.
- Do not add abstraction or configurability before it is needed.
- Keep functions small and focused; prefer composition over duplication.
- Refactor opportunistically — leave the code cleaner than you found it, but in
  a separate commit from behavioural changes where practical.

## Comments and documentation

- Every public type and function has a short doc comment (XML doc for C#, TSDoc
  for TypeScript) saying what it is for.
- Comments explain **why**, not **what**.
- Do not leave a `TODO` without a corresponding entry in
  [`../architecture/technical-debt.md`](../architecture/technical-debt.md), an
  MVP, or an implementation plan.

## Error handling

- Never swallow an exception silently (`catch {}`, `except: pass`, empty
  `.catch()`).
- Log unexpected exceptions with enough context to diagnose them.
- Model domain errors as their own types, not bare strings or generic
  exceptions.
- Guard against predictable failure inputs (null, empty, zero divisor,
  out-of-range).

## Security

- Never log passwords, API keys, tokens, cookies, connection strings or other
  secrets.
- Validate and authorise every external request.
- Never build a query or command by concatenating untrusted input — use
  parameters.
- Protect state-changing web endpoints against CSRF where applicable.

## Dependencies

- Follow [`../dependencies/README.md`](../dependencies/README.md) for how
  versions are pinned and reviewed.
- A significant new dependency needs a note in the dependency policy or an ADR.
- Remove unused dependencies and imports.

## Testing

- New functionality includes automated tests.
- A bug fix includes a regression test whenever practical.
- Details and frameworks: [`testing.md`](testing.md).

## Language-specific standards

- [`coding-dotnet.md`](coding-dotnet.md) — C# / .NET (`source/`)
- [`coding-typescript-react.md`](coding-typescript-react.md) — TypeScript / React
  (`judotech-portal/`)

The Python data-import scripts in `source/` are maintenance-only tooling
(ADR-0001) and are not covered by a formal standard; keep them readable and
PEP 8-ish.
