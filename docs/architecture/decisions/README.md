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
| _none yet_ | ADRs 0001–0007 are added in MVP-001, Phase 2 | — |
