# ADR-0006: One production environment; deployments are separate and gated

- Status: proposed
- Date: 2026-09-02
- Deciders: project owner
- Related: MVP-001 Phase 7, `docs/architecture/overview.md` section 5.5

## Context

`.github/workflows/ci_api.yml` builds **and deploys** `source/judotech.api` to
the production Azure Function app on every push to `main` and on a weekly cron;
`ci_web.yml` similarly uploads `source/judotech.web` to production blob storage.
CI and CD are the same workflow. `deploy_function.yml` and `deploy_web.yml` are
manual duplicates. The roadmap mentions Test / UAT / Production tiers, but only
Production exists, and there is no gate before a deploy.

## Decision

- **One environment for now: `production`.** Test and UAT tiers are deferred
  until an application actually needs them.
- **CI never deploys.** Continuous-integration workflows build, type-check, lint
  and test only.
- **Deployment is a separate, explicitly triggered workflow** (`workflow_dispatch`)
  bound to a protected GitHub `production` environment (required reviewer /
  approval). No deploy happens as a side effect of a push or a schedule.
- The combined `ci_api.yml` / `ci_web.yml` workflows are replaced by
  build-and-test CI workflows plus the existing manual deploy workflows, which
  gain the `production` environment binding.

## Consequences

- MVP-001 Phase 7 splits the workflows: `ci-dotnet.yml`, `ci-portal.yml`,
  `ci-web-legacy.yml` (all no-deploy) and keeps `deploy_function.yml` /
  `deploy_web.yml` as gated manual deploys with `environment: production`.
- Merging to `main` no longer ships to production; someone runs the deploy
  workflow and approves it.
- A broken `main` no longer auto-deploys.
- Adding a Test/UAT environment later means adding an environment and a
  workflow, not restructuring.
- The GitHub `production` environment and its protection rule must be configured
  in repository settings — documented in `docs/development/ci-cd.md`.

## Alternatives considered

- **Keep deploy-on-push to production.** Fast, but unsafe with no tests gating
  and no staging; a bad merge is live immediately.
- **Add Test + UAT + Production now.** Matches the roadmap wording but there is
  no application to exercise them and no infrastructure for them; premature.
- **Deploy on tags / releases.** Reasonable future option; deferred until the
  release process is defined.
