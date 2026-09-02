# MVP Workflow

The workspace is built in small, scoped increments called MVPs. Each MVP goes
through the same loop. The prompt templates that drive it (for AI-assisted work)
are in [`../claude-prompts/`](../claude-prompts/).

## The loop

1. **Identify an MVP.** Decide the next increment that moves the workspace or a
   product forward. Write it up as `docs/mvp/MVP-NNN-<slug>.md` with: Goal,
   Scope (in and out), Expected value, Acceptance criteria. No implementation
   detail.
2. **Plan it.** Write `docs/plans/MVP-NNN-<slug>.plan.md` with: Goal,
   Assumptions, Proposed file changes, step-by-step TODOs grouped into phases
   (one phase = one commit), Risks / open questions.
3. **Decide as needed.** Any significant decision made while planning or
   implementing gets an ADR in
   [`../architecture/decisions/`](../architecture/decisions/).
4. **Implement.** Work the plan one phase at a time on a branch. Check off plan
   items as they land. Deferred or discovered issues go into
   [`../architecture/technical-debt.md`](../architecture/technical-debt.md),
   not into scope creep.
5. **Complete.** Verify every acceptance criterion, update the MVP status to
   `done` with a pointer to where each criterion was met, update the overview and
   any affected docs, and open a pull request against `main`.

## Where things live

| Artefact | Location |
|----------|----------|
| MVP definition | `docs/mvp/MVP-NNN-<slug>.md` |
| Implementation plan | `docs/plans/MVP-NNN-<slug>.plan.md` |
| Decisions | `docs/architecture/decisions/NNNN-<slug>.md` |
| Deferred / accepted issues | `docs/architecture/technical-debt.md` |

## MVP status values

`proposed` → `in progress` → `done`. A `done` MVP is not edited further; a
follow-up is a new MVP.

## Phase-to-commit mapping

Each phase in a plan is one commit with the commit message given in the plan.
The human reviews the diff and commits; the assistant does not commit.
