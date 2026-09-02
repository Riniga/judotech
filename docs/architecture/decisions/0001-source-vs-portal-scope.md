# ADR-0001: `source/` is maintenance-only; `judotech-portal/` is the active target

- Status: proposed
- Date: 2026-09-02
- Deciders: project owner
- Related: MVP-001 (`docs/plans/MVP-001-workspace-foundation.plan.md`),
  `docs/architecture/overview.md` sections 1 and 11

## Context

The repository contains two generations of code with no recorded relationship
(`docs/architecture/overview.md` section 1):

- `source/` — a .NET 8 solution (`judotech.core` + `judotech.api`, an Azure
  Functions HTTP API over Cosmos DB), four Gulp/Pug static sites, three
  Windows-only video-streaming experiments, and Python data-import scripts. Parts
  of this are deployed today (the `judotech` function app and the `judotech.web`
  static site).
- `judotech-portal/` — a newer npm-workspaces monorepo (React 19 + Vite +
  Tailwind) that is currently a scaffold: one app (`athlete`) with a placeholder
  page, one partially-populated shared package (`ui`).

Every later decision in MVP-001 (test frameworks, CI layout, workspace
structure) depends on knowing which code is being actively developed and what
the portal builds on.

## Decision

- **`judotech-portal/` is the active development target.** New application and
  feature work happens there.
- **`source/` is maintenance-only.** Changes to `source/` are limited to
  security fixes, critical bug fixes, and the minimum needed to keep the deployed
  API and site running. No new features are added to `source/`.
- **The portal consumes the existing `source/judotech.api`** as its backend for
  now. The portal does not re-implement the API in this MVP.
- **Whether to rewrite, restructure, or replace `judotech.api`** (for example to
  address the issues in ADR-0007) is deferred to a future ADR, to be written
  before the portal exposes authenticated end-user functionality.
- The Gulp/Pug sites (`judotech.web*`) are frozen; they are not migrated into the
  portal as part of MVP-001.

Active vs frozen paths:

| Path | Status |
|------|--------|
| `judotech-portal/**` | Active |
| `source/judotech.core/**`, `source/judotech.api/**` | Maintenance-only |
| `source/judotech.web/**`, `source/judotech.web.calendar/**`, `source/judotech.web.club/**`, `source/judotech.web.referee/**` | Frozen (maintenance-only) |
| `source/judotech.VideoStream*/**` | Dormant; kept in place, not built (see ADR / technical-debt register) |
| `source/judotech.integrations.members/**`, `source/**/data/*.py` | Maintenance-only (manual tooling) |

## Consequences

- MVP-001 invests tooling effort mainly in `judotech-portal/` and in a minimal
  test/CI baseline for `source/` — not in restructuring `source/`.
- The portal's `packages/core` will contain an API client targeting
  `judotech.api`; its contract is whatever the Functions endpoints currently
  expose.
- The known `source/` architecture issues (Active Record, no DI, in-memory
  caches, auth weaknesses) are recorded in
  `docs/architecture/technical-debt.md` and are **not** fixed in MVP-001, except
  where they block the portal.
- If the owner later decides `source/judotech.api` must be replaced before the
  portal ships, parts of MVP-001 Phase 5–7 (the API test project, `ci-dotnet`)
  remain useful but the portal's backend strategy changes.
- Two toolchains (.NET and Node) remain in the repository and in CI.

## Alternatives considered

- **Treat both as active.** Rejected: doubles the standards, review and CI
  surface for a solo/small team while `source/` has no roadmap features left.
- **Delete or archive `source/` now.** Rejected: the API and `judotech.web` are
  in production and the portal has no backend of its own yet.
- **Rewrite the API inside the portal as part of MVP-001.** Rejected: that is
  feature-scale work, out of scope for a foundation MVP, and premature before the
  portal's needs are known.
