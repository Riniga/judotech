# `source/` — legacy implementation (maintenance-only)

Per [ADR-0001](../docs/architecture/decisions/0001-source-vs-portal-scope.md),
this tree is **maintenance-only**. New application and feature work happens in
[`../judotech-portal/`](../judotech-portal/).

Changes here are limited to security fixes, critical bug fixes, and the minimum
needed to keep the deployed API and site running.

| Path | Status |
|------|--------|
| `judotech.core/`, `judotech.api/` (+ `*.tests/`) | maintenance-only — the .NET Functions API the portal consumes |
| `judotech.web/`, `judotech.web.calendar/`, `judotech.web.club/`, `judotech.web.referee/` | frozen Gulp/Pug static sites (`judotech.web` does not currently build — TD-046) |
| `judotech.VideoStream*/` | dormant .NET Framework 4.8 experiments, not in `judotech.sln` |
| `judotech.integrations.members/`, `**/data/*.py` | manual member-import tooling |

See [`../docs/architecture/overview.md`](../docs/architecture/overview.md) for the
full picture and [`../docs/development/setup.md`](../docs/development/setup.md) to
build and run.
