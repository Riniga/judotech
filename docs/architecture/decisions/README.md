# Architecture Decision Records

An Architecture Decision Record (ADR) captures one significant decision: the
context that forced it, the choice made, and the consequences that follow.

Write an ADR when a decision:

- affects the workspace structure, build, or how applications are added,
- is hard or expensive to reverse,
- picks one option where a reasonable person would have picked another, or
- future contributors will otherwise ask "why is it done this way?".

Do not write an ADR for reversible, low-impact, or self-evident choices.

## Format

- One file per decision: `NNNN-short-title.md`, four-digit zero-padded number,
  `kebab-case` title.
- Copy [`adr-template.md`](adr-template.md) to start.
- Numbers are never reused. A superseded ADR keeps its file and is marked
  `superseded by ADR-XXXX`; the new ADR links back with `supersedes ADR-YYYY`.

## Statuses

| Status | Meaning |
|--------|---------|
| `proposed` | Written, not yet ratified by the owner |
| `accepted` | Ratified; in effect |
| `superseded` | Replaced by a later ADR (link to it) |
| `deprecated` | No longer relevant, not replaced |

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [0001](0001-source-vs-portal-scope.md) | `source/` is maintenance-only; `judotech-portal/` is the active target | accepted |
| [0002](0002-documentation-location.md) | All project documentation lives under `docs/` | accepted |
| [0003](0003-language-policy.md) | English for code and docs; Swedish only in domain data and UI copy | accepted |
| [0004](0004-frontend-workspace.md) | Front-end workspace — npm workspaces + Turborepo, `@judotech/*` packages | accepted |
| [0005](0005-test-frameworks.md) | Test frameworks — xUnit for .NET, Vitest + Testing Library for the portal | accepted |
| [0006](0006-environments-and-deployment.md) | One production environment; deployments are separate and gated | accepted |
| [0007](0007-source-auth-review.md) | `source/` authentication scheme — review outcome | accepted |

ADR-0001…0007 were ratified by the owner at MVP-001 close-out (2026-09-02).
