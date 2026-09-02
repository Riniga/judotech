# Dependency Management

How third-party dependencies and tool versions are managed across the workspace.

## Tool versions — pinned exactly

| Tool | Pinned by | Value |
|------|-----------|-------|
| .NET SDK | `source/global.json` | `8.0.0`, `rollForward: major` (prefers 8.x; uses a newer major only if 8.x is absent) |
| Node.js | `.nvmrc`, `judotech-portal/.nvmrc` | `24` |
| npm | `judotech-portal/package.json` → `packageManager` | `npm@11.4.2` |

CI installs the pinned versions (`ci-dotnet.yml` uses `8.0.x`; the Node
workflows read `.nvmrc`). `judotech-portal/package.json` also sets
`engines.node >= 22` as a lower bound.

Changing a pinned tool version is a deliberate change: update the file(s) above
**and** the matching CI workflow(s) in the same pull request, and note it in the
architecture overview if it affects contributors.

## Library versions — compatible ranges

- **npm** (`judotech-portal`): caret ranges (`^x.y.z`). `package-lock.json` is
  committed and is the source of truth; CI runs `npm ci` (fails if
  `package.json` and the lockfile disagree).
- **NuGet** (`source/`): exact versions on every `<PackageReference>`. There is
  no `packages.lock.json`; `dotnet restore` resolves from nuget.org. Adding
  lock files (`<RestorePackagesWithLockFile>`) is optional future work.
- Reference a package **explicitly** if code uses it directly, even when a
  transitive path exists — e.g. `Newtonsoft.Json` in `judotech.core` /
  `judotech.api` (also required by `Microsoft.Azure.Cosmos` >= 3.32).
- The legacy `source/judotech.VideoStream*` projects use classic
  `packages.config`; they are dormant (ADR-0001) and not maintained.

## Lockfiles in the repository

| Path | Manager |
|------|---------|
| `judotech-portal/package-lock.json` | npm (workspaces) |
| `source/judotech.web/package-lock.json` | npm |
| `source/judotech.web.calendar/package-lock.json` | npm |
| `source/judotech.web.club/package-lock.json` | npm |
| `source/judotech.web.referee/package-lock.json` | npm |
| `source/judotech.VideoStreamCapture/package-lock.json` | npm (stray — the project is C#) |

Every actively used npm project has a committed lockfile. The .NET projects
have none by design (see above).

## Updating dependencies

- Most updates come in as **Dependabot** pull requests — see
  [`../development/dependency-updates.md`](../development/dependency-updates.md)
  for how they are grouped, auto-merged, and taken in as a batch.
- Review updates deliberately; do not blanket-run `npm audit fix` /
  `dotnet outdated` and commit the result.
- A **security** advisory that affects shipped code is a `fix/` branch on its
  own (or a Dependabot security PR).
- Manual routine bumps go on a `chore/update-dependencies` branch, CI green
  before merge.
- Run the stack's checks after any bump (`dotnet test` / `npm run lint typecheck
  test build`).

## When a dependency change needs an ADR

Write an ADR (`docs/architecture/decisions/`) when a dependency:

- changes the build or task-runner model (e.g. adding/removing Turborepo — see
  ADR-0004);
- changes the test framework (ADR-0005);
- introduces a new runtime platform, language, or major framework;
- is hard to reverse once code depends on it.

Routine library additions do not need an ADR — a note in the pull request is
enough.

## Changes recorded during MVP-001

- Removed `clean` from `@judotech/ui` (unexplained, unused) — TD-022.
- Removed the stale `@types/react-router-dom` v5 from `apps/athlete`
  (`react-router-dom` v7 ships its own types) — TD-023.
- Removed three commented-out `Microsoft.Azure.Functions.Worker.*` package
  references from `judotech.core.csproj`.
- Added `turbo`, `vitest`, `@testing-library/*`, `jsdom`, and `@judotech/config`
  to the portal (Phases 5–6).
- `apps/athlete` still carries unused `@tailwindcss/postcss` / `autoprefixer` /
  `postcss` and redundant ESLint-plugin devDependencies — TD-042, TD-044.
- 13 open `npm audit` advisories in the portal tree (dev/transitive) — TD-043;
  Dependabot security updates chip away at these.
- Deleted a stray empty `source/judotech.VideoStreamCapture/package-lock.json`
  (no matching `package.json`).
- Added `.github/dependabot.yml` (grouped, weekly) and a Dependabot auto-merge
  workflow — Phase 7 addendum.
