# Architecture Overview

This document describes the current state of the JudoTech workspace as of 2026-09-02.
It documents what exists in the repository today. Where information is missing or
cannot be confirmed from the repository, this is stated explicitly rather than assumed.

## Decisions and technical debt

The open questions and observations in sections 11 and 13 are being resolved
through MVP-001 (`docs/plans/MVP-001-workspace-foundation.plan.md`). Decisions
are recorded as ADRs; deferred and accepted items are tracked in the debt
register.

- [`decisions/`](decisions/) — Architecture Decision Records:
  - ADR-0001 — `source/` is maintenance-only; `judotech-portal/` is the active target
  - ADR-0002 — all documentation lives under `docs/`
  - ADR-0003 — English for code and docs; Swedish only in domain data and UI copy
  - ADR-0004 — front-end workspace: npm workspaces + Turborepo, `@judotech/*` packages
  - ADR-0005 — test frameworks: xUnit (.NET), Vitest + Testing Library (portal)
  - ADR-0006 — one production environment; deployments are separate and gated
  - ADR-0007 — `source/` authentication scheme review outcome
- [`technical-debt.md`](technical-debt.md) — every section 11 / 13 item and every
  ADR-0007 finding, each with a disposition (fixed in MVP-001, accepted via ADR,
  or backlog).

## 1. Project overview

JudoTech is described in the root `README.md` as an ecosystem for managing the
technical needs of a judo community: memberships, rankings, competition systems,
result boards and related functionality. The intended users are competitors,
coaches, referees, organizers and supporters.

The repository is a single Git repository (`git@github.com:Riniga/judotech.git`,
default branch `main`) licensed under GPL-3.0.

The repository currently contains **two parallel generations of code**:

1. `source/` – an earlier implementation: a .NET solution (core library + Azure
   Functions API), several Gulp/Pug static websites, some .NET Framework video
   streaming experiments, and data-import scripts.
2. `judotech-portal/` – a newer npm-workspaces monorepo intended to become the
   front-end platform (React + Vite + Tailwind). This is the "initial version"
   referred to in `docs/claude-prompts/initera-projektet/`.

How the two generations relate is now decided in ADR-0001: `source/` is
maintenance-only and `judotech-portal/` is the active development target, which
consumes the existing `source/judotech.api` as its backend for now.

The `docs/roadmap.md` file expresses an intent to grow the platform into
many applications (public site, club portal, referee site, coach site, calendar,
athlete mobile app, results site, scoreboard app, competition server, streaming
server/app, care app, intercom app). Most of these do not exist yet.

## 2. Current workspace structure

```text
judotech/
├─ README.md                     # Setup notes: Azure CLI, conda, .NET, Docker
├─ LICENSE                        # GPL-3.0
├─ judotech_env.bat              # conda env activation helper
├─ .editorconfig, .gitignore, .nvmrc, .markdownlint.json, .gitleaks.toml
├─ .github/workflows/            # CI/CD (see section 5.5)
├─ .vscode/                       # settings, tasks, launch, extensions
├─ devcontainer/dockerfile        # Ubuntu 22.04 base dev container
│
├─ docs/                          # All project documentation
│  ├─ README.md                   # Documentation entry point / reading order
│  ├─ features.md
│  ├─ functional-requirements.md          # Swedish
│  ├─ non-functional-requirements.md      # Swedish
│  ├─ roadmap.md
│  ├─ azure.drawio
│  ├─ architecture/               # overview.md (this doc), coding.png, decisions/
│  ├─ standards/                  # coding.md, documentation.md, git.md, testing.md
│  ├─ development/                # setup, process, ci-cd, workflow (stubs)
│  ├─ dependencies/               # dependency policy (stub)
│  ├─ mvp/                        # MVP-001-workspace-foundation.md
│  ├─ plans/                      # MVP-001-workspace-foundation.plan.md
│  └─ claude-prompts/             # AI-assisted workflow prompt templates
│
├─ source/                        # Earlier implementation
│  ├─ judotech.sln                # Contains judotech.core + judotech.api
│  ├─ judotech.core/              # .NET 8 class library (domain + Cosmos DB)
│  ├─ judotech.api/               # .NET 8 isolated Azure Functions (HTTP API)
│  ├─ judotech.web/               # Gulp + Pug static site ("Webplats för Svensk Judo")
│  ├─ judotech.web.calendar/      # Gulp + Pug static site (events)
│  ├─ judotech.web.club/          # Gulp + Pug static site (club/athlete)
│  ├─ judotech.web.referee/       # Gulp + Pug static site (referee)
│  ├─ judotech.VideoStreamCapture/  # .NET Framework 4.8 WinForms (streaming experiment)
│  ├─ judotech.VideoStreamView/     # .NET Framework 4.8 WinForms
│  ├─ judotech.VideoStreamStorage/  # .NET Framework 4.8 console
│  ├─ judotech.integrations.members/  # Jupyter notebook: member import from Excel
│  └─ buildscript.sh
│
└─ judotech-portal/               # Newer front-end monorepo (npm workspaces + Turborepo)
   ├─ package.json                # workspaces: apps/*, packages/*; turbo scripts
   ├─ turbo.json                  # task pipelines (build/lint/typecheck/test)
   ├─ tsconfig.base.json          # shared TS compiler options
   ├─ readme.md
   ├─ apps/
   │  └─ athlete/                 # Vite + React 19 app (only app that exists)
   └─ packages/
      ├─ ui/                      # @judotech/ui — shared React components + theme
      ├─ core/                    # @judotech/core — domain types, hooks, API clients
      └─ config/                  # @judotech/config — shared ESLint + Vitest config
```

`packages/config`, `packages/core` (minimal) and `tsconfig.base.json` were added
in MVP-001 Phase 5. `apps/public`, `apps/trainer` and `apps/referee` are still to
be created on demand (ADR-0004).

## 3. Major components

### 3.1 `source/judotech.core` (.NET 8 class library)

Domain and data-access layer, referenced by `judotech.api`.

- `DatabaseBase` – abstract base defining user, login and competition operations.
  `GetDefaultDatabase()` returns `CosmosDatabase`.
- `CosmosDatabase` – Azure Cosmos DB implementation (`Microsoft.Azure.Cosmos`
  3.34.0). Reads `EndpointUrl`, `PrimaryKey`, `DatabaseId` from environment
  variables. Creates database/containers on demand.
- `Settings` – lazy singleton mapping container names to partition key paths:
  `Users` → `/email`, `Competitions` → `/name`, `Logins` → `/email`.
- `Users`, `Competitions` – lazy singletons caching all rows in memory, with a
  `Refresh()` method.
- `DbUser`, `DbCompetition`, `DbLogin` – active-record style models; each has
  `Create()` / `Update()` / `Delete()` that call the database directly. `DbUser`
  also loads itself from the database in a constructor.
- `DbLogin` – token-based auth: a `Guid` token stored in the `Logins` container;
  `GetUserFromToken` resolves a token to a user.
- Password hashing uses `Microsoft.AspNetCore.Cryptography.KeyDerivation`.
- `Logger` – lazy singleton logging helper.
- Roles are stored as a list of strings on `DbUser` (e.g. `judoka`, `coach`,
  `referee`, `admin`, `manager` are referenced in code and docs).

> **`DbUser` was reworked in commit `f0b94b4` but left half-done** — 53 build
> errors from constructors and the DB/API layer still referencing removed
> members. Fixed in MVP-001 (Phase 5 addendum) by restoring `DbUser` as a
> superset of both property shapes; the solution now builds and the Functions
> host runs. Choosing one coherent model is tracked as TD-045.

### 3.2 `source/judotech.api` (.NET 8 isolated Azure Functions v4)

HTTP-triggered REST-style API. `Program.cs` uses the isolated worker model with
Application Insights. All functions use `AuthorizationLevel.Function`.
Serialization uses `Newtonsoft.Json`.

- `UserApi` – `CreateUser`, `CreateUsers`, `ReadUser`, `ReadAllUser`,
  `UpdateUser`, `DeleteUser`.
- `CompetitionApi` – `CreateCompetition`, `CreateCompetitions`,
  `ReadCompetition`, `ReadAllCompetitions`, `UpdateCompetition` (role-checked
  against `manager`).
- `AuthenticatorApi` – `Login`, `Logout`, `HashPassword`. (The ad-hoc
  HTTP-invoked `TestAuthenticationApi` self-test was removed in MVP-001 Phase 6;
  its flow moved to `judotech.api.tests`.)

Local configuration is via `local.settings.json` (copied from
`local.settings_sample.json`; not committed).

### 3.3 `source/judotech.web*` (static websites)

Four separate static sites, each built the same way: `gulp` + `gulp-pug` +
`gulp-uglify` / `gulp-csso`, plain JavaScript, `pico.css`. Each has
per-environment config files (`development`, `production`, `test`, `uat`) and its
own `package.json`.

- `judotech.web` – "the main website" per docs; the only site wired into CI.
- `judotech.web.calendar` – events/calendar.
- `judotech.web.club` – club/athlete pages, plus member data JSON and a Python
  import script under `source/data/`.
- `judotech.web.referee` – referee pages, includes a "Domararvode" (referee fee)
  form.

The relationship between these sites and the intended `Judotech.Web.*` submodule
architecture in `docs/features.md` is aspirational; the sites are
currently standalone.

### 3.4 `source/judotech.VideoStream*` (.NET Framework 4.8)

Three Windows-only projects for capturing, viewing and storing video streams.
They are **not part of `judotech.sln`** and have no CI. They appear to be
experiments related to the streaming items on the roadmap.

### 3.5 `source/judotech.integrations.members` and data scripts

- `getmembersfromexcel.ipynb` and
  `judotech.web.club/source/data/personregister.py` – Python/pandas scripts that
  read a member register exported from SportAdmin (`kansli.sportadmin.se`) as
  Excel and produce JSON. `personregister.py` reads a hard-coded Google Drive
  path (`G:\Min enhet\...\Gradering.xlsx`). This is a manual, local process.

### 3.6 `judotech-portal/apps/athlete` (React app)

The only application in the new monorepo.

- Build tool: Vite 7 (installed as `npm:rolldown-vite`), `@vitejs/plugin-react`,
  `@tailwindcss/vite`, `vite-plugin-svgr`.
- Framework: React 19, `react-router-dom` 7.
- Styling: Tailwind CSS v4.
- Language: TypeScript ~5.9, strict mode, ESLint 9 flat config
  (`typescript-eslint`, React Hooks, React Refresh plugins).
- `@judotech/ui` is resolved via a Vite alias and a `tsconfig` path directly to
  the package source (no build step for the library).
- Current content is minimal: `App.tsx` sets up a router with `AppLayout` and a
  `Dashboard` page; `Dashboard` renders a placeholder (`<p>test</p>`).

### 3.7 `judotech-portal/packages/ui` (`@judotech/ui`)

Shared React component library. `package.json` sets `main` to `src/index.ts`
(consumed as source, not built). Peer dependency on React 19.

- `src/index.ts` currently exports only `Button`, `AppLayout` and
  `ThemeProvider`.
- `src/` also contains a large set of components not yet exported: an app
  shell (`AppSidebar`, `AppHeader`, `Header`, `Backdrop`, `SidebarWidget`),
  header widgets (`NotificationDropdown`, `UserDropdown`), theme toggles, a
  `common/` and `ui/` set of primitives (alert, avatar, badge, button, dropdown,
  images, modal, table, videos), ~80 SVG icons, and `styles/index.css`
  (~780 lines: Tailwind v4 theme, Outfit font, `.dark` custom variant).
- `context/ThemeContext.tsx` – light/dark theme stored in `localStorage`,
  toggling a `dark` class on `<html>`. `context/SidebarContext.tsx` – sidebar
  expand/collapse state.
- These components strongly resemble an existing admin dashboard template; the
  origin and licensing of that template are **not documented**.
- `package.json` listed an unexplained dependency `"clean": "^4.0.2"` (removed
  in MVP-001 Phase 5).

> **Updated (MVP-001 Phase 5):** `packages/core` is now a real package
> (`@judotech/core`) with a minimal `HttpClient` and a `User` type placeholder.
> `packages/ui` gained `tsconfig.json` / `eslint.config.js` / `vitest.config.ts`,
> an `svg.d.ts`, and an `exports` map; its exported surface still comprises only
> `Button`, `AppLayout` and `ThemeProvider` (TD-021).

## 4. Existing dependencies

### 4.1 .NET (`source/`)

| Project | Target | Key packages |
|---|---|---|
| `judotech.core` | `net8.0` | `Microsoft.Azure.Cosmos` 3.34.0, `Microsoft.AspNetCore.Cryptography.KeyDerivation` 8.0.20 |
| `judotech.api` | `net8.0`, Azure Functions v4 isolated | `Microsoft.Azure.Functions.Worker` 2.0.0, `...Worker.Sdk` 2.0.2, `...Extensions.Http.AspNetCore` 2.0.1, `Microsoft.ApplicationInsights.WorkerService` 2.23.0, `Newtonsoft.Json` (transitive/explicit via code) |
| `judotech.VideoStream*` | `net48` | Not analyzed in detail |

### 4.2 Static sites (`source/judotech.web*`)

Node tooling: `gulp` 5, `gulp-pug` 5, `gulp-uglify`, `gulp-csso`,
`gulp-autoprefixer`, `gulp-concat`, `gulp-rename`, `gulp-changed`, `gulp-clean`,
`merge-stream`, `yargs`. CSS: `pico.css`. Node LTS (docs mention Node 20/22).

### 4.3 `judotech-portal`

Root: npm workspaces + Turborepo (`turbo` ^2 as a dev dependency, `turbo.json`
with `build` / `lint` / `typecheck` / `test` / `dev` pipelines). Node 24 pinned
via `.nvmrc`; npm 11 pinned via `packageManager`. As of MVP-001 Phase 5.

`apps/athlete` runtime: `react` ^19.2, `react-dom` ^19.2, `react-router-dom`
^7.9, plus workspace deps `@judotech/ui` and `@judotech/core`.
`apps/athlete` dev: `vite` (`npm:rolldown-vite@7.2.5`), `@vitejs/plugin-react`,
`tailwindcss` ^4.1, `@tailwindcss/vite`, `vite-plugin-svgr`, `typescript` ~5.9,
`vitest` ^3, `@testing-library/*`, `jsdom`, ESLint 9 + `@judotech/config`.
(The stale `@types/react-router-dom` v5 was removed in Phase 5.)

`packages/ui`: peer `react` / `react-dom` ^19.2 and `react-router-dom` ^7; dev
`typescript` ~5.9, `@judotech/config`, `vitest`, `@testing-library/*`, `jsdom`.
(The unexplained `clean` dependency was removed in Phase 5.)

`packages/core`: dev `typescript` ~5.9, `vitest`, `@judotech/config`. No runtime
dependencies yet.

`packages/config`: `@eslint/js`, `typescript-eslint`, `eslint-plugin-react-hooks`,
`eslint-plugin-react-refresh`, `globals`; peer `eslint` ^9, optional `vitest`.

### 4.4 Python

`pandas`, `openpyxl` (used by the member-import scripts; installed manually per
`source/judotech.web.club/readme.md`). No `requirements.txt`, `pyproject.toml` or
environment lock file is committed. `README.md` describes a conda environment
`judotech` (`python=3`, `nodejs=24`).

## 5. Build and development process

### 5.1 Local environment (from root `README.md`)

- Tools: VS Code, Azure CLI, Miniconda/Anaconda, Docker Desktop (optional dev
  container in `devcontainer/dockerfile`).
- Conda env `judotech` created with Python 3 + Node.js; activated via
  `judotech_env.bat`.
- Azure resources are provisioned manually with `az` commands documented in
  `README.md` (resource group `Judoka`, app service plan, storage account,
  Cosmos DB).

### 5.2 API (`source/judotech.api`)

- Created with `func init . --worker-runtime dotnetIsolated --target-framework
  net8.0`.
- Run locally: `func start --csharp` (or `startapi.bat`). Debug by attaching to
  the `func` process.
- Requires `local.settings.json` with Cosmos DB settings.
- Tested manually with Postman (per `README.md`).

### 5.3 Static sites (`source/judotech.web*`)

- `npm install`, then `gulp --environment <development|test|uat|production>` and
  `gulp watch`; served locally with `live-server` on the `public/` output.

### 5.4 `judotech-portal`

- `npm install` at the repo `judotech-portal/` root.
- `npm run dev:athlete` – Vite dev server for the athlete app.
- `npm run build:athlete` – `tsc -b && vite build`.
- `npm run preview:athlete` – Vite preview.
- `npm run build` (root) – currently non-functional (see 4.3).

### 5.5 CI/CD (`.github/workflows/`)

Reorganised in MVP-001 Phase 7 (ADR-0006). CI never deploys; deployment is
manual and gated on a `production` environment. Full detail:
[`../development/ci-cd.md`](../development/ci-cd.md).

| Workflow | Trigger | What it does | Deploys? |
|---|---|---|---|
| `ci-dotnet.yml` | push/PR to `main`, manual | restore + build (Release) + test (`Category!=Integration`) on .NET 8 | No |
| `ci-portal.yml` | push/PR to `main`, manual | `npm ci` + lint + typecheck + test + build in `judotech-portal/` | No |
| `ci-web-legacy.yml` | push/PR to `main`, manual | `gulp` build of the frozen sites (`calendar`/`club`/`referee` required, `judotech.web` non-blocking) | No |
| `codeql.yml` | push/PR to `main`, weekly | CodeQL for `csharp` + `javascript-typescript` | No |
| `security-secret-scan.yml` | push/PR to `main`, manual | gitleaks | No |
| `deploy_function.yml` | manual only | publish + deploy `judotech.api` to the `judotech` Function App | Yes (`production`) |
| `deploy_web.yml` | manual only (choose `site`) | build + upload a static site to `storagejudotech` `$web` | Yes (`production`) |

Removed: `ci_api.yml` and `ci_web.yml` (built **and** deployed on every push,
pinned .NET 3.1). `codeql.yml` moved from the repo root into
`.github/workflows/` and fixed.

Secrets: `judotech_FFFF` (function publish profile), `AZURE_CREDENTIALS`
(blob upload). The old `judotech` publish-profile secret is now unused.

## 6. Coding standards

`docs/standards/` contains (as of MVP-001 Phase 3):

- `coding.md` – language-neutral principles: SOLID where it helps, simplicity
  over cleverness, small focused units, comment the *why*, no silent exception
  handling, no secrets in logs, no query building by string concatenation,
  tests for reusable logic. Links to the per-stack standards below.
- `coding-dotnet.md` – C# / .NET conventions matching the real code (`net8.0`,
  nullable, `PascalCase`, file-scoped namespaces, `dotnet format`, thin
  Functions, parameterised Cosmos queries) and a list of known deviations.
- `coding-typescript-react.md` – TypeScript / React conventions (strict TS,
  shared ESLint/Prettier via `@judotech/config`, named exports from packages,
  function components, Tailwind theme tokens, API calls via `@judotech/core`).
- `testing.md` – frameworks per stack (xUnit for .NET, Vitest + Testing Library
  for the portal), test file patterns, run commands, and the rule that
  Cosmos-dependent tests are excluded from CI.
- `git.md` – branch from `main`; prefixes `feature/ fix/ docs/ refactor/
  chore/`; small focused commits in English; PRs against `main`; CI green before
  merge; never commit secrets; AI assistants do not commit/push/merge.
- `documentation.md` – English, Markdown, `kebab-case`, short and practical;
  update docs in the same PR as the change; defines the `docs/` tree.

Earlier revisions of this document noted that the standards described a Python
project; that was corrected in MVP-001 Phase 3. Language policy is set by
ADR-0003.

Actual conventions visible in the code:

- .NET: `PascalCase` types/methods, `net8.0`, nullable enabled, implicit usings,
  `Newtonsoft.Json` attributes on models, singletons for shared state.
- TypeScript: ESLint flat config, strict `tsconfig`, `PascalCase` components,
  Tailwind utility classes, path alias `@judotech/ui`.
- Swedish appears in code comments, some identifiers, `readme.md` files,
  requirements documents and business data. `coding.md` asks for English-only.

## 7. Development process

- `docs/claude-prompts/` defines an AI-assisted, document-driven
  workflow: initialize the project → identify an MVP
  (`docs/mvp/MVP-001-...`) → create an implementation plan (`docs/plans/`) →
  implement in small steps → complete the MVP and open a PR. These prompts
  reference `docs/development/`, `docs/standards/*`,
  `docs/architecture/overview.md` and ADRs under
  `docs/architecture/decisions/`. The `docs/architecture/decisions/`,
  `docs/development/` and `docs/dependencies/` trees are currently stubs
  (created in MVP-001, Phase 1).
- `git.md` describes the intended Git workflow (feature branches, small PRs to
  `main`, CI green before merge).
- `.claude/settings.json` grants an AI assistant read-only and edit/write tools
  and requires confirmation for `commit`, `push` and file deletion.
- Versioning scheme (root `README.md`): `Major.Minor.Patch` plus a stability
  suffix (`alpha`, `beta`, `RC`). `roadmap.md` places the project at
  `v1.0.0-alpha`.

How closely the actual history follows this process is **unknown**; recent
commits (`Mail functionality and half tailwind`, `added personregiseter from
sportadmin`, `Clean history (no secrets)`) suggest direct commits to `main`
during early setup.

## 8. Design patterns

Observed in `source/judotech.core`:

- **Active Record** – `DbUser`/`DbCompetition`/`DbLogin` carry their own
  persistence methods.
- **Singleton** (via `Lazy<T>`) – `Users`, `Competitions`, `Settings`, `Logger`.
- **Abstract base / template** – `DatabaseBase` with a `CosmosDatabase`
  implementation, allowing an alternative store in principle.
- **In-memory cache with explicit refresh** – `Users.AllUsers` /
  `Competitions.AllCompetitions`.

Observed in `judotech-portal`:

- **Monorepo / workspaces** with a shared UI package consumed as source.
- **React context providers** for cross-cutting UI state (theme, sidebar).
- **Layout + `<Outlet/>` routing** (`AppLayout` wraps routed pages).

No formal architectural pattern (layered, hexagonal, clean architecture, CQRS,
etc.) is documented.

## 9. Testing strategy

- `docs/standards/testing.md` defines xUnit for .NET and Vitest + React Testing
  Library for the portal (MVP-001 Phase 3; ADR-0005).
- **Test suites now exist (MVP-001 Phase 6):**
  - `source/judotech.core.tests` (xUnit) — `HashPassword` and `Settings`
    container-mapping tests. 6 tests.
  - `source/judotech.api.tests` (xUnit) — function-registration reflection
    checks, plus a skipped `[Trait("Category","Integration")]` end-to-end auth
    test (needs Cosmos).
  - `judotech-portal`: `@judotech/core` (`HttpClient`), `@judotech/ui` (`Button`)
    and `apps/athlete` (`Dashboard`) each have a Vitest suite.
  - Run with `dotnet test source/judotech.sln` and `npm test` (from
    `judotech-portal/`). CI excludes integration tests via
    `--filter Category!=Integration`.
- **When this section was first written, no automated tests existed** — no test
  projects, no `*.test.*` files, no `test` script on the athlete app.
- The legacy static sites still have the default failing `test` script.
- A CI test step is added in MVP-001 Phase 7.

## 10. Existing integrations

Confirmed from the repository:

- **Azure Cosmos DB** – primary datastore for the API (`Users`, `Competitions`,
  `Logins` containers). Configured via `EndpointUrl` / `PrimaryKey` /
  `DatabaseId` environment variables.
- **Azure Functions** – hosts `judotech.api` (function app name `judotech`).
- **Azure Storage static website hosting** – hosts `source/judotech.web`
  (`storagejudotech`, `$web` container).
- **Azure Application Insights** – telemetry for the API.
- **SportAdmin** (`kansli.sportadmin.se`) – member register exported manually to
  Excel and imported by Python scripts; no API integration.
- **Google Drive** – a hard-coded local path in `personregister.py` for the
  grading spreadsheet.

Referenced but not integrated:

- **Smoothcomp** – only appears as a saved HTML page under
  `source/judotech.web.calendar/source/images/`. No code integration.
- **Email / SMTP** – `functional-requirements.md` lists "e-postbekräftelse vid
  registrering" as in progress (🔄) and a commit is titled "Mail functionality",
  but **no email-sending code is present in the repository**. Status unknown.
- **Turborepo** – referenced by the root `build` script but not installed.
  *(Installed and configured in MVP-001 Phase 5.)*

## 11. Architectural observations

Each observation below is tracked in
[`technical-debt.md`](technical-debt.md) with a disposition (fixed in MVP-001,
accepted via an ADR, or backlog).

- The repository holds two generations of code. `source/` is a working-ish .NET +
  static-site stack; `judotech-portal/` is a fresh front-end monorepo that is
  essentially a scaffold. Their relationship is decided in ADR-0001.
- The API combines domain logic and persistence in the model classes (Active
  Record) and reads Azure config from environment variables directly in the data
  layer. There is no dependency injection of the database, no repository
  abstraction boundary at the API level, and singletons cache all users and
  competitions in process memory.
- Authentication is a custom scheme: SHA-derived password hash plus an opaque
  GUID token stored in a Cosmos container. `GetUserFromToken` builds a Cosmos
  SQL query by string concatenation of the token value.
- ~~`ci_api.yml` deploys to production on every push to `main`~~ — fixed in
  MVP-001 Phase 7 (CI/CD split, ADR-0006). There is still no staging/test/UAT
  environment (TD-012, deferred).
- ~~The `.NET 3.1.x` SDK pin in CI does not match the `net8.0` projects.~~ Fixed
  in MVP-001 Phase 4 & 7.
- The `judotech-portal` UI package is consumed directly as TypeScript source
  (via alias / tsconfig paths) rather than being built and versioned; it also
  contains a large amount of unexported template code.
- The standards documents (`docs/standards/`) describe a Python project, which
  does not match the current C#/.NET and TypeScript/React codebase.
- Language is mixed (English and Swedish) across code, docs and data, contrary
  to the English-only rule in `coding.md`.
- No test infrastructure exists in either generation.

## 12. Planned evolution of the workspace

From `docs/roadmap.md` and `docs/features.md` (intent, not
commitments, and not all consistent with each other):

- `v1.0.0-alpha` (stated current): Core, API, Referee at `1.0.0-alpha.1`.
- `v1.0.0-beta` (next): first complete application; Core and API marked done;
  `Judotech.Web`, `Judotech.Web.Login`, `Judotech.Web.Club`,
  `Judotech.Web.Referee` in progress.
- Later, unscheduled: Coach web, Calendar web, Athlete mobile app, Results web,
  Scoreboard app, Competition server, Streaming server/app, Care app, Intercom
  app.

`judotech-portal/readme.md` describes the near-term monorepo target:

- Apps: `public` (visitors), `trainer` (coaches), `athlete`, `referee`.
- Packages: `ui` (shared components/theme), `core` (domain logic, types, hooks,
  API clients), `config` (shared tsconfig / eslint / tailwind config).
- Root `tsconfig.base.json`; optionally adopt Turborepo for builds; add Tailwind
  for table UI.

`docs/claude-prompts/` implies the immediate next step is to define
`docs/mvp/MVP-001-workspace-foundation.md` establishing the workspace foundation
before feature work begins.

## 13. Open questions and areas not yet implemented

The items below are tracked in [`technical-debt.md`](technical-debt.md) with a
disposition. Several are resolved or scheduled by MVP-001; the list is kept here
as the record of what was open before that work.

Structure and direction:

- ~~What is the intended relationship between `source/` and `judotech-portal/`?~~
  Decided in ADR-0001 (`source/` maintenance-only, portal active, portal consumes
  `judotech.api`). Whether to rewrite the API is a future ADR.
- Which standards actually apply? The current standards target Python; there are
  no C# or TypeScript/React standards. (Fixed in MVP-001 Phase 3.)

`judotech-portal` (not yet implemented):

- `packages/core` (empty), `packages/config`, root `tsconfig.base.json`.
- `apps/public`, `apps/trainer`, `apps/referee`.
- A working root build (`turbo` is referenced but absent).
- Exporting and documenting the `@judotech/ui` components; clarifying the origin
  and license of the bundled template; explaining the `clean` dependency.
- Any real application logic — `Dashboard` is a placeholder; there is no API
  client, auth flow, routing structure, or data model in the front end.
- Removing the stale `@types/react-router-dom` (v5) alongside `react-router-dom`
  v7.

`source/` (incomplete or unverified):

- ~~No automated tests anywhere.~~ Fixed in MVP-001 Phase 6.
- ~~CI targets the wrong .NET SDK version; CI and deployment are not separated.~~
  Fixed in MVP-001 Phase 4 & 7. No non-production environment (TD-012, deferred).
- Email/registration-confirmation functionality status is unclear (TD-028).
- `judotech.web.club` / `.calendar` / `.referee` now have build CI
  (`ci-web-legacy.yml`); `judotech.web` itself does not build (TD-046).
- The video streaming projects are outside the solution and unbuilt (TD-018).
- Member import is a manual local script with a hard-coded path.
- Security review needed for the custom auth scheme (string-concatenated Cosmos
  queries, token handling, password hashing approach).

Process:

- `docs/development/` and `docs/architecture/decisions/` exist as stubs only
  (created in MVP-001, Phase 1); the development process and ADR log still need
  to be written (MVP-001, Phase 3 and Phase 2). `docs/mvp/` and `docs/plans/`
  now hold the MVP-001 documents.
- Whether branch/PR discipline from `git.md` is being followed in practice is
  unknown.

Non-functional requirements (`non-functional-requirements.md`): response time
(<1s for 95% of calls), encryption at rest for user data, 99.9% availability —
none of these have verification, monitoring, or evidence in the repository
beyond Application Insights being enabled.
