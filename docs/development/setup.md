# Development Setup

## Prerequisites

| Tool | Version | Pinned by | Needed for |
|------|---------|-----------|------------|
| .NET SDK | 8.0.x (9.0.x also works) | `source/global.json` | `source/` (API + tests) |
| Node.js | 24 (LTS) | `.nvmrc`, `judotech-portal/.nvmrc` | `judotech-portal/`, static sites |
| npm | 11.x | `judotech-portal/package.json` `packageManager` | — |
| Azure Functions Core Tools (`func`) | v4 | — | running `source/judotech.api` locally |
| Azure Cosmos DB | — | — | optional: emulator or a real endpoint, only for API data / integration tests |

`global.json` requests .NET 8 and rolls forward to a newer major if 8.x is
absent. With `nvm`: `nvm install && nvm use` in the repo root or in
`judotech-portal/`.

## Editor

Open the repo in VS Code and install the recommended extensions
(`.vscode/extensions.json`). Format-on-save and ESLint fix-on-save are set in
`.vscode/settings.json`; rules come from `.editorconfig`.

## Clone to running

### `judotech-portal` — athlete app (active)

```bash
cd judotech-portal
npm install
npm run dev:athlete      # Vite dev server on http://localhost:5173
```

Other commands (from `judotech-portal/`): `npm run build`, `npm run lint`,
`npm run typecheck`, `npm test` — each fans out across the workspace via
Turborepo.

### `source/judotech.api` — Functions API (active)

```bash
dotnet build source/judotech.sln

cp source/judotech.api/local.settings_sample.json source/judotech.api/local.settings.json
# edit local.settings.json: set EndpointUrl / PrimaryKey / DatabaseId
#   - Cosmos DB emulator: EndpointUrl https://localhost:8081 and its well-known key
#   - or a real Cosmos DB account

cd source/judotech.api
func start               # host on http://localhost:7071
```

`GET http://localhost:7071/api/HashPassword?password=abc` works without Cosmos;
the user/competition endpoints need a reachable Cosmos DB. `local.settings.json`
is gitignored.

### `source/judotech.web*` — static sites (maintenance-only)

```bash
cd source/judotech.web.club          # or .calendar / .referee
npm install
npx gulp --environment development    # output in ./public
npx live-server public                # optional local preview
```

`source/judotech.web` currently fails to build (TD-046); `calendar`, `club` and
`referee` build.

## Running the tests

| Stack | From | Command |
|-------|------|---------|
| .NET | repo root | `dotnet test source/judotech.sln` |
| .NET (CI subset) | repo root | `dotnet test source/judotech.sln --filter "Category!=Integration"` |
| Portal (all workspaces) | `judotech-portal/` | `npm test` |
| One portal workspace | `judotech-portal/` | `npm test --workspace @judotech/ui` |

The `judotech.api.tests` integration test is `Skip`-marked; remove the `Skip`
and provide Cosmos settings to run it. See
[`../standards/testing.md`](../standards/testing.md).

## Dev container

`devcontainer/dockerfile` is an Ubuntu 22.04 base with common tools. It does
**not** yet bundle the .NET SDK or Node.js — install them inside the container
per the versions above, or use a local toolchain.

## Verified

`dotnet build` + `dotnet test` and the full `judotech-portal` flow
(`npm ci` → `lint` → `typecheck` → `test` → `build`) were run clean on
Windows 11 during MVP-001. `func start` hosts the API and answers
`HashPassword`.
