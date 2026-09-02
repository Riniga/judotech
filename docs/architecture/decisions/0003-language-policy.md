# ADR-0003: English for code and documentation; Swedish only in domain data and UI copy

- Status: proposed
- Date: 2026-09-02
- Deciders: project owner
- Related: `docs/standards/coding.md`, `docs/architecture/overview.md` section 6

## Context

`docs/standards/coding.md` already requires English for all code and commits, but
practice is mixed: Swedish appears in code comments and some identifiers, in
several `readme.md` files, in the requirements documents
(`functional-requirements.md`, `non-functional-requirements.md`, `features.md`),
and in business data. The domain itself (Swedish judo clubs, SportAdmin exports,
referee-fee forms) is Swedish, and end users are Swedish-speaking.

## Decision

English is used for:

- all code — identifiers, comments, log messages, error messages intended for
  developers;
- commit messages, pull request text, branch names;
- all documentation under `docs/` and all `README` / `readme` files.

Swedish (or other languages) is allowed only for:

- business and domain **data** (member registers, club names, source
  spreadsheets, exported JSON) kept in its original form;
- user-facing UI copy — and that should go through an internationalisation layer
  once one exists, not be hard-coded.

## Consequences

- New code and docs are English from now on; reviewers may reject non-English
  contributions.
- Existing Swedish content is migrated opportunistically, not urgently:
  - the three Swedish requirements documents are translated — tracked as a
    backlog item in `docs/architecture/technical-debt.md`, not a blocker;
  - Swedish comments/identifiers in `source/` are left as-is (that code is
    maintenance-only per ADR-0001) and cleaned only when a file is touched for
    another reason.
- `docs/standards/coding.md` is updated in MVP-001 Phase 3 to reference this ADR.

## Alternatives considered

- **Swedish for everything.** Rejected: narrows the contributor pool and breaks
  from common open-source practice; the repo is already GPL-3.0 and public.
- **Bilingual documentation.** Rejected: doubles maintenance for a small team.
- **Leave the policy implicit.** Rejected: the current standard is already
  ignored; an explicit, scoped rule is enforceable.
