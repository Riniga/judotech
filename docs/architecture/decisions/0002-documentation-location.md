# ADR-0002: All project documentation lives under `docs/`

- Status: accepted
- Date: 2026-09-02
- Deciders: project owner
- Related: MVP-001 Phase 1, `docs/standards/documentation.md`

## Context

Documentation had drifted between two trees. Tracked files lived under
`documentation/`, while `docs/standards/documentation.md` and the workflow
prompt templates in `docs/claude-prompts/` assumed `docs/`. A partial rename left
an empty `documentation/` directory and stale `documentation/...` path
references in the architecture overview.

## Decision

- All project documentation lives under `docs/`.
- The `documentation/` directory is removed and must not be recreated.
- `docs/standards/documentation.md` remains the authority on structure, naming
  (`kebab-case`) and language.

## Consequences

- MVP-001 Phase 1 deleted `documentation/`, fixed the overview's path
  references, and added `docs/README.md` as the entry point.
- Any external links or bookmarks pointing at `documentation/...` break; there
  are no known external consumers.
- Contributors have one place to look and one place to add documentation.

## Alternatives considered

- **Keep `documentation/`.** Rejected: it contradicts the standards and the
  prompt templates, and `docs/` is the more common convention.
- **Support both.** Rejected: guarantees future drift.
