# Development Setup

> **Partial.** Prerequisites and toolchain pinning are in place (MVP-001
> Phase 4). The verified per-component "clone to running" walkthroughs are added
> in MVP-001 Phase 8. Until then, also see the repository root
> [`README.md`](../../README.md) and each component's own `README` / `readme.md`.

## Prerequisites

| Tool | Version | Pinned by | Notes |
|------|---------|-----------|-------|
| .NET SDK | 8.0.x (9.0.x also works) | `source/global.json` | `global.json` requests 8.0 and rolls forward to a newer major if 8.0 is not installed. CI uses 8.0.x. |
| Node.js | 24 (LTS) | `.nvmrc`, `judotech-portal/.nvmrc` | `judotech-portal/package.json` `engines` allows `>=22`. |
| npm | 11.x | `judotech-portal/package.json` `packageManager` | Ships with Node 24. |
| Azure Functions Core Tools (`func`) | v4 | — | Needed to run `source/judotech.api` locally. |
| Azure Cosmos DB | — | — | Optional locally: the emulator or a real endpoint, only needed for API integration tests and running the API against real data. |

With `nvm`: `nvm install` then `nvm use` in the repo root or in `judotech-portal/`.

## Editor

Open the workspace in VS Code and install the recommended extensions
(`.vscode/extensions.json`). Formatting on save and ESLint fixes on save are
configured in `.vscode/settings.json`; formatting rules come from
`.editorconfig`.

## Dev container

`devcontainer/dockerfile` provides an Ubuntu 22.04 base with common tools. It
does **not** yet include the .NET SDK or Node.js — install them inside the
container per the versions above, or use a local toolchain. Extending the
Dockerfile with pinned .NET 8 and Node 24 is tracked for a later change.

## Per-component setup

> Added in MVP-001 Phase 8, verified on a clean checkout:
>
> - `source/judotech.api` (active)
> - `judotech-portal` athlete app (active)
> - `source/judotech.web*` static sites (maintenance-only)
