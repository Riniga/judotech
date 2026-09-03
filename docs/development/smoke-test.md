# Startup & Smoke Test

How to start each part of JudoTech and confirm it is alive. This is **not** full
UAT — it verifies that things build, start, and answer. Use it:

- on a fresh checkout, to confirm the environment is set up;
- after each phase of an implementation plan, as a regression gate;
- as the first pass before deeper acceptance testing.

Setup and versions: [`setup.md`](setup.md). Test frameworks:
[`../standards/testing.md`](../standards/testing.md).

## What runs where

| Component | Start from | Command | URL |
|-----------|-----------|---------|-----|
| API (Azure Functions) | `source/judotech.api` | `func start` | <http://localhost:7071> |
| Cosmos DB emulator | anywhere | see TC-05 | <https://localhost:8081/_explorer/index.html> |
| Portal — athlete app | `judotech-portal` | `npm run dev:athlete` | <http://localhost:5173> |
| Static site (any of 4) | `source/judotech.web[.*]` | `npx gulp --environment development` then `npx live-server public` | <http://localhost:8080> |

The API and the portal are the two things under active development. The Cosmos
emulator is only needed for the data-backed API endpoints. The static sites
build and serve independently.

## Start everything (happy path)

Four terminals:

```bash
# 1 — Cosmos emulator (Docker)
docker run --rm -p 8081:8081 -p 10250-10255:10250-10255 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator

# 2 — API
cd source/judotech.api
cp -n local.settings_sample.json local.settings.json   # first time
# then set PrimaryKey in local.settings.json (see Prerequisites below) — it is gitignored
func start

# 3 — Portal
cd judotech-portal
npm install        # first time
npm run dev:athlete

# 4 — (optional) a static site
cd source/judotech.web
npm install        # first time
npx gulp --environment development && npx live-server public
```

On Windows without Docker you can install the native
[Azure Cosmos DB emulator](https://learn.microsoft.com/azure/cosmos-db/local-emulator)
instead of terminal 1.

## Prerequisites for the "Needs Cosmos" cases

Configuration is **setup, not a test**. Before TC-06 onward:

1. The emulator is running (TC-05 passes).
2. `source/judotech.api/local.settings.json` exists
   (`cp local.settings_sample.json local.settings.json`) and its `PrimaryKey` is
   set. This file is **gitignored** — never commit a key, not even the emulator's.
   - Cosmos emulator: use its fixed well-known key — it is published in the
     [Microsoft emulator docs](https://learn.microsoft.com/azure/cosmos-db/emulator#authentication)
     and printed by the emulator on startup. It only works against
     `localhost:8081`.
   - Real Cosmos account: its primary key from the Azure portal or
     `az cosmosdb keys list`.
3. `func start` was **restarted** after any change to `local.settings.json` — it
   reads settings once at startup.

A `FormatException: not a valid Base-64 string` from
`CosmosClient..ctor` / `CosmosDatabase.GetContainer` means `PrimaryKey` is still
the placeholder or was mistyped — it is the key, fix step 2.

## Test cases

Run top to bottom.

| ID | Area | Steps | Expected | Needs Cosmos |
|----|------|-------|----------|--------------|
| **TC-01** | .NET build | `dotnet build source/judotech.sln -c Release` | `Build succeeded`, `0 Error(s)` | no |
| **TC-02** | .NET unit tests | `dotnet test source/judotech.sln --filter "Category!=Integration"` | all test projects `Passed!`, 0 failed | no |
| **TC-03** | API host starts | terminal 2: `func start` | Startup lists the HTTP functions (`Login`, `Logout`, `CreateUser`, …); no unhandled exception; host reaches "lease acquired" | no |
| **TC-04** | API routes without Cosmos | `curl -s -X POST http://localhost:7071/api/Login -H "content-type: application/json" -d '{}' -w "\n[%{http_code}]\n"` | `400` with `email and password are required` — the host is up and routing works; validation happens before any Cosmos call | no |
| **TC-05** | Emulator reachable | `curl -sk -o /dev/null -w "%{http_code}\n" https://localhost:8081/_explorer/index.html` (or open it in a browser) | `200` — the Data Explorer loads. A bare `GET https://localhost:8081/` returning `401` also means the emulator is up (it wants a key). Endpoint paths differ between emulator builds; the Linux/preview build 404s on `/_explorer/emulator.pem`. | — |
| **TC-06** | API reaches Cosmos | with the prerequisites above met: `curl -s -o /dev/null -w "%{http_code}\n" "http://localhost:7071/api/ReadAllUser"` | `HTTP 200` (body `[]` or a list). Not `500` — a 500 with a Cosmos exception in the `func` log means the emulator or the key is wrong. | yes |
| **TC-07** | Create + read a user | `curl -s -X POST http://localhost:7071/api/CreateUser -H "content-type: application/json" -d '{"email":"smoke@test.nu","password":"pw","roles":["judoka"]}'` then `curl "http://localhost:7071/api/ReadUser?email=smoke@test.nu"` | Create returns `true`; Read returns the user JSON (password field empty/absent) | yes |
| **TC-08** | Login flow | `curl -s -X POST http://localhost:7071/api/Login -H "content-type: application/json" -d '{"email":"smoke@test.nu","password":"pw"}'` | `HTTP 200` with a login object containing a token | yes |
| **TC-09** | Cleanup | `curl -X POST "http://localhost:7071/api/DeleteUser?email=smoke@test.nu"` | `true`; `ReadAllUser` no longer lists it | yes |
| **TC-10** | Portal install | `cd judotech-portal && npm ci` | completes without error | no |
| **TC-11** | Portal checks | `npm run lint && npm run typecheck && npm test && npm run build` | all four `Tasks: N successful`; `apps/athlete/dist` produced | no |
| **TC-12** | Athlete dev server | terminal 3: `npm run dev:athlete`, open <http://localhost:5173> | Page loads with the app layout styled (sidebar / header from `@judotech/ui`); the placeholder text `test` is visible; no console errors. There are no real screens yet — `Dashboard` is a stub (TD-024). | no |
| **TC-13** | Static site build | for each of `judotech.web`, `judotech.web.calendar`, `judotech.web.club`, `judotech.web.referee`: `cd source/<site> && npm ci && npx gulp --environment development` | `Finished 'default'`; `public/` contains `index.html` | no |
| **TC-14** | Static site serves | `cd source/judotech.web && npx live-server public` | <http://localhost:8080> shows the calendar page with nav and footer | no |

## Recording a run

Copy the table, mark each `pass` / `fail` / `skip`, note the date, commit SHA,
and OS. A failing TC is a blocker for the change that caused it — capture the
error output.

### Baseline — 2026-09-03, Windows 11 (`main` + `fix/appinsights-2x` + `fix/ui-tailwind-source`)

Full run, all 14 **pass**. Two issues were found and fixed during the run:

| TC | Result | Note |
|----|--------|------|
| TC-01, TC-02 | pass | core.tests 6, api.tests 2 |
| TC-03 | pass **after fix** | `func start` first crashed: `TypeLoadException: ITelemetryInitializer`. `Microsoft.ApplicationInsights.WorkerService` 3.1.2 (Dependabot) is incompatible with the 2.x Functions worker AI package. Pinned to 2.23.0 (TD-052). |
| TC-04 | pass | |
| TC-05 | pass | `200` on `/_explorer/index.html` |
| TC-06–TC-09 | pass | user round-trip create → read → login → delete against the emulator |
| TC-10, TC-11 | pass | |
| TC-12 | pass **after fix** | Page first rendered unstyled — Tailwind v4 was not scanning `@judotech/ui`. Added `@source "../"` to `packages/ui/src/styles/index.css` (TD-055). |
| TC-13, TC-14 | pass | all four sites build and serve |

## Beyond smoke

Full UAT (real login against a hardened API, role checks, the portal's actual
screens) is defined per feature. The auth flow specifically is covered by
`source/judotech.api.tests/AuthenticationIntegrationTests.cs` (run against the
emulator) once MVP-002 lands.
