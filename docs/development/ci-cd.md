# CI/CD

> **Stub.** This document is filled in by MVP-001, Phase 7
> ([`../plans/MVP-001-workspace-foundation.plan.md`](../plans/MVP-001-workspace-foundation.plan.md)).
> The current state of the workflows is described in
> [`../architecture/overview.md`](../architecture/overview.md) section 5.5.

Intended contents:

- One row per workflow in `.github/workflows/`: trigger, what it does, whether it
  deploys.
- The list of GitHub Actions secrets and what each is for.
- Required branch-protection settings for `main` (which checks must pass, no
  direct pushes).
- How a production deployment is triggered and gated.
