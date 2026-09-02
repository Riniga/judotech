# Development Process

How work moves from idea to merged change in this workspace. Keeps changes
small, reviewable and safe to merge.

## Branching

- Never work directly on `main`. `main` is the latest stable state.
- Branch from an up-to-date `main`:
  - `feature/<short-name>` — new capability
  - `fix/<short-name>` — bug fix
  - `docs/<short-name>` — documentation only
  - `refactor/<short-name>` — behaviour-preserving change
  - `chore/<short-name>` — tooling, dependencies, config
- One logical change per branch. Prefer several small branches over one large
  mixed one.
- Full rules: [`../standards/git.md`](../standards/git.md).

## While working

- Follow the active plan in [`../plans/`](../plans/) when there is one; implement
  one plan item at a time.
- Commit after each completed and verified step, with a clear English imperative
  message.
- Run the checks for the stack you changed before each commit:
  - .NET: `dotnet test source/judotech.sln`
  - portal: `cd judotech-portal && npm run lint && npm run typecheck && npm test`
- Update documentation in the **same** change, not later (see below).

## When documentation is required

Update docs in the same pull request when the change affects:

- workspace or architecture structure → `../architecture/overview.md`, and an ADR
  if it is a decision;
- development setup, tools or dependencies → `../development/`,
  `../dependencies/`;
- coding, testing or Git conventions → `../standards/`;
- behaviour that is not self-evident from the code.

Do not document implementation detail that the code already makes clear.

## When tests are required

- New feature → unit tests, plus integration tests where a boundary is crossed.
- Bug fix → a regression test that fails before the fix.
- Refactor → existing tests stay green.
- See [`../standards/testing.md`](../standards/testing.md).

## Pull requests

- Open against `main` using the PR template.
- Describe the purpose and link the MVP, plan, ADR or issue.
- All CI checks pass before merge (see [`ci-cd.md`](ci-cd.md)).
- No local merges into `main` unless explicitly agreed.
- Do not push or merge without approval when an AI assistant is doing the work.

## Definition of done

A change is done when:

- it does what the plan item / issue described, and nothing unrelated;
- checks and tests pass locally and in CI;
- documentation is updated in the same PR;
- an ADR exists for any significant decision;
- no secrets, local settings or generated artefacts are committed;
- the plan checklist (if any) is updated.

## Working with AI assistants

When Claude Code, Copilot or similar is used:

- it follows the active plan and implements one item at a time;
- it shows the intended change before a broad refactor;
- it updates the plan as work progresses;
- **it does not commit, push, merge or delete branches** — the human reviews the
  diff and commits (see [`../../.claude/settings.json`](../../.claude/settings.json)
  and [`../standards/git.md`](../standards/git.md)).

## Related

- [`workflow.md`](workflow.md) — the MVP → plan → implement → complete loop
- [`setup.md`](setup.md) — environment and how to run each component
- [`ci-cd.md`](ci-cd.md) — pipelines, secrets, branch protection
