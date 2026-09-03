# Technical Debt Register

Every known issue from `docs/architecture/overview.md` sections 11 and 13, plus
the authentication findings from
[`decisions/0007-source-auth-review.md`](decisions/0007-source-auth-review.md),
with a disposition:

- **Fixed in MVP-001 (Phase N)** — resolved by this MVP.
- **Accepted (ADR-XXXX)** — a deliberate decision not to change it now.
- **Backlog** — real work, deferred; should become a future MVP.

Severity applies to Backlog items: **high** = address before the portal ships
authenticated/user-facing features; **medium** = address opportunistically;
**low** = cosmetic or low-risk.

Owner `unassigned` means no one has picked it up yet.

## Architecture & workspace

| ID | Description | Source | Disposition | Severity | Owner |
|----|-------------|--------|-------------|----------|-------|
| TD-001 | Two code generations (`source/`, `judotech-portal/`) with no documented relationship | §11, §13 | Accepted (ADR-0001) | – | – |
| TD-002 | `judotech.api` mixes domain logic and persistence (Active Record), no DI of the database, no repository boundary at the API layer | §11 | Backlog | medium | unassigned |
| TD-003 | `Users` / `Competitions` singletons cache all rows in process memory with manual `Refresh()` | §11 | Backlog | low | unassigned |
| TD-004 | Standards documents describe a Python project; no C#/.NET or TS/React standards | §11, §13 | Fixed in MVP-001 (Phase 3) | – | – |
| TD-005 | English/Swedish mixed across code, docs and data | §11 | Accepted (ADR-0003) going forward | – | – |
| TD-006 | Existing Swedish requirements docs (`functional-requirements.md`, `non-functional-requirements.md`, `features.md`) not translated | §11 (ADR-0003) | Backlog | low | unassigned |
| TD-007 | No automated test infrastructure in either generation | §11, §13 | Fixed in MVP-001 (Phase 6) | – | – |
| TD-008 | No development-process doc, no ADR log | §13 | Fixed in MVP-001 (Phase 2 & 3) | – | – |
| TD-009 | Unclear whether branch/PR discipline is followed | §13 | Fixed in MVP-001 (Phase 3 doc; Phase 7 branch protection) | – | – |

## Build, CI/CD & tooling

| ID | Description | Source | Disposition | Severity | Owner |
|----|-------------|--------|-------------|----------|-------|
| TD-010 | CI pins .NET `3.1.x` while projects target `net8.0` | §11, §13 | Fixed in MVP-001 (Phase 4 & 7) | – | – |
| TD-011 | `ci_api.yml` / `ci_web.yml` deploy to production on every push; CI and CD combined | §11, §13 | Fixed in MVP-001 (Phase 7; ADR-0006) | – | – |
| TD-012 | No Test/UAT environment despite roadmap tiers | §11, §13 | Accepted (ADR-0006) — deferred | medium | unassigned |
| TD-013 | `judotech-portal` root build calls `turbo` but it is not installed and there is no `turbo.json` | §13 | Fixed in MVP-001 (Phase 5; ADR-0004) | – | – |
| TD-014 | No `packages/config`, no root `tsconfig.base.json` | §13 | Fixed in MVP-001 (Phase 5) | – | – |
| TD-015 | `packages/core` is an empty directory | §13 | Fixed in MVP-001 (Phase 5, minimal) | – | – |
| TD-016 | `judotech.web.club` / `.calendar` / `.referee` have no CI | §13 | Fixed in MVP-001 (Phase 7, build-only) | – | – |
| TD-017 | `judotech.web.club` / `.calendar` / `.referee` have no deployment workflow | §13 | Partly fixed — `deploy_web.yml` now takes a `site` input covering all four; only `judotech.web` was previously deployable | low | unassigned |
| TD-018 | Video-streaming projects (`judotech.VideoStream*`, net48) are outside `judotech.sln` and unbuilt | §13 | Accepted (ADR-0001) — kept dormant in place | low | unassigned |
| TD-046 | `source/judotech.web` failed to build: `sub-layout.pug` `extends`'d a non-existent path, and `gulpfile.js` referenced a phantom `../judotech.web/` "layout" project in the `scripts` / `styles` / `images` tasks. | MVP-001 Phase 7 | Fixed in MVP-001 (Phase 7 addendum) — corrected the `extends` to the sibling `layout.pug`, dropped the `../judotech.web/` references, guarded the empty `configurations` / `data` tasks, added a `.gitignore`. Builds dev + prod; `ci-web-legacy` now runs all four sites as required. | – | – |
| TD-047 | The `judotech` GitHub Actions secret (old function publish profile) is no longer referenced by any workflow | MVP-001 Phase 7 | Backlog — delete in repo settings | low | unassigned |

## Front-end (`judotech-portal`)

| ID | Description | Source | Disposition | Severity | Owner |
|----|-------------|--------|-------------|----------|-------|
| TD-019 | `@judotech/ui` consumed as raw source, not built or versioned | §11 | Accepted (ADR-0004) | – | – |
| TD-020 | `packages/ui` contains a large set of unexported components resembling an admin template; origin and licence undocumented | §11, §13 | Backlog | medium | unassigned |
| TD-021 | Only 3 of ~100 `packages/ui` components are exported; the rest are not wired up, typed or tested | §13 | Backlog | medium | unassigned |
| TD-022 | `packages/ui` has an unexplained `clean` dependency | §11, §13 | Fixed in MVP-001 (Phase 5) | – | – |
| TD-023 | `apps/athlete` has both `react-router-dom` v7 and the stale `@types/react-router-dom` v5 | §13 | Fixed in MVP-001 (Phase 5) | – | – |
| TD-024 | No real app logic: `Dashboard` is a placeholder; no API client, auth flow, routing structure or data model | §13 | Backlog (feature work; a minimal `HttpClient` lands in Phase 5) | medium | unassigned |
| TD-025 | `apps/{public,trainer,referee}` do not exist | §13 | Accepted (ADR-0004) — created on demand | – | – |
| TD-026 | `apps/athlete` uses `vite: npm:rolldown-vite` (pre-release) | plan Risks | Accepted for now — Vitest 3 works with it (verified Phase 5) | low | unassigned |
| TD-027 | `apps/athlete` has a leftover `postcss.config.mjs.bak` | plan | Fixed in MVP-001 (Phase 5) — deleted; no PostCSS config needed with Tailwind v4 `@tailwindcss/vite` | – | – |
| TD-041 | Tailwind theme tokens live only in `packages/ui/src/styles/index.css` (~780 lines); not extracted into `@judotech/config` for reuse by non-UI packages | MVP-001 Phase 5 (Risk 8) | Backlog | low | unassigned |
| TD-042 | `apps/athlete` still carries unused `@tailwindcss/postcss`, `autoprefixer`, `postcss` devDeps (Tailwind v4 uses the Vite plugin) | MVP-001 Phase 5 | Backlog | low | unassigned |
| TD-043 | `npm audit` reports 13 advisories in the portal dependency tree (dev-only / transitive) | MVP-001 Phase 5 | Backlog — Dependabot security updates (`.github/dependabot.yml`) address these over time | medium | unassigned |
| TD-044 | `apps/athlete` `devDependencies` still list ESLint plugins now provided transitively by `@judotech/config` | MVP-001 Phase 5 | Backlog | low | unassigned |
| TD-055 | Tailwind v4 auto-detection did not scan the `@judotech/ui` package, so `apps/athlete` rendered with no styling on any `@judotech/ui` class (sidebar, header, `Button`, layout). | MVP-002 Phase 0 smoke test (TC-12) | Fixed — added `@source "../"` to `packages/ui/src/styles/index.css`; built CSS 45 kB → 76 kB, `bg-brand-500` / `min-h-screen` / `lg:ml-[290px]` etc. now generated. | – | – |
| TD-048 | ~8 open Dependabot PRs on `main` (transitive security bumps in the legacy static sites) predate MVP-001 and its CI | MVP-001 Phase 7 addendum | Backlog — triage per `docs/development/dependency-updates.md` after MVP-001 merges (they gain `ci-web-legacy` coverage then) | medium | unassigned |
| TD-049 | Stale remote branches: `feature/containerapp`, `feature/saveprofile`, `features/react` | MVP-001 Phase 7 addendum | Backlog — delete after confirming they hold nothing wanted | low | unassigned |

## `source/` — other

| ID | Description | Source | Disposition | Severity | Owner |
|----|-------------|--------|-------------|----------|-------|
| TD-028 | Email / registration-confirmation functionality status unknown (no email-sending code found) | §10, §13 | Backlog (investigate, then scope) | medium | unassigned |
| TD-029 | Member import is a manual local Python script with a hard-coded Google Drive path | §13 | Backlog | low | unassigned |
| TD-030 | Smoothcomp referenced only by a saved HTML file; no integration and unclear intent | §10 | Backlog | low | unassigned |
| TD-031 | NFR targets (response <1s p95, encryption at rest, 99.9% availability) have no verification or monitoring | §13 | Backlog | medium | unassigned |
| TD-040 | `source/judotech.core` did not compile: commit `f0b94b4` reworked `DbUser`'s properties but left the constructors, `DbUser(string email)` loader, `Delete()`, `CosmosDatabase` and `AuthenticatorApi` referencing the removed members. 53 build errors. | MVP-001 Phase 4 build check | Fixed in MVP-001 (Phase 5 addendum) — `DbUser` restored to a **superset** of both property shapes so all call sites compile; solution builds (0 errors), `func start` hosts all 15 functions, `HashPassword` verified. Settling on one `DbUser` / Cosmos model remains open (see TD-045). | – | – |
| TD-045 | `DbUser` is a superset of the pre- and post-`f0b94b4` models (both the address-book fields and `First`/`Lastname`/`Started`/`BirthDate`/`Total`/`ShouldHaveGrade`). One coherent model needs to be chosen, along with the Cosmos partition-key implications. | TD-040 fix | Backlog | medium | unassigned |
| TD-050 | `judotech.api` produces ~13 nullable-reference warnings (`CS8600/CS8602/CS8603`) in `UserApi` / `CompetitionApi` / `AuthenticatorApi`; blocks turning on `TreatWarningsAsErrors` | Dependabot dotnet-group PR #42 (clean CI build) | Backlog | low | unassigned |
| TD-051 | `Microsoft.Azure.Cosmos` >= 3.32 requires an explicit `Newtonsoft.Json` reference; Dependabot's dotnet-group bump (Cosmos 3.34→3.62) failed CI until it was added | Dependabot PR #42 | Fixed — explicit `Newtonsoft.Json` 13.0.3 added to `judotech.core` and `judotech.api` (both use it directly anyway) | – | – |
| TD-052 | Dependabot bumped `Microsoft.ApplicationInsights.WorkerService` to 3.1.2 — incompatible with `Microsoft.Azure.Functions.Worker.ApplicationInsights` 2.x. `dotnet build` + `dotnet test` stay green but `func start` crashes with `TypeLoadException: ITelemetryInitializer`. CI never runs the host, so it was missed until the MVP-002 Phase 0 smoke test (TC-03). | MVP-002 Phase 0 smoke test | Fixed — pinned `Microsoft.ApplicationInsights.WorkerService` to 2.23.0; Dependabot `ignore` for `Microsoft.ApplicationInsights*` major bumps. Unblock when the Functions worker AI package supports AI 3.x. | – | – |
| TD-053 | `ci-dotnet` builds and unit-tests but never starts the Functions host, so a package/runtime incompatibility (TD-052) is invisible to CI | MVP-002 Phase 0 | Backlog — a smoke job that runs `func start` and hits `HashPassword`, or the `ci-dotnet-integration` job (MVP-002 Phase 6), would catch it | medium | unassigned |

## Authentication (`decisions/0007-source-auth-review.md`)

| ID | Description | Source | Disposition | Severity | Owner |
|----|-------------|--------|-------------|----------|-------|
| TD-032 | Password hashing uses a single hard-coded salt for all users (`DbLogin.HashPassword`) | ADR-0007 A | Fixed in MVP-002 (Phase 2) — `PasswordHasher`: per-password 128-bit random salt, PBKDF2 600k iters, `pbkdf2$sha256$…` format; legacy hashes re-hashed on next successful login | – | – |
| TD-033 | `CosmosDatabase.GetUserFromToken` builds SQL by concatenating the raw token (injection risk) | ADR-0007 B | Backlog | high | unassigned |
| TD-034 | Plaintext password written to the log during `AuthenticatorApi.Login` | ADR-0007 C | Fixed in MVP-002 (Phase 2) — `Login` rewritten via `AuthService` with no logging; the `CreateUser` request-body log line removed. `DbLogin.LoginUser`'s hash-logging is dead code, removed in Phase 3. The `Logger.Instance` → `ILogger` migration + a no-secret-logging test are Phase 5. | – | – |
| TD-035 | Session tokens never expire and are not rotated | ADR-0007 D | Backlog | medium | unassigned |
| TD-036 | Single shared function key; most user/competition endpoints do no per-user or role check | ADR-0007 E | Backlog | medium | unassigned |
| TD-037 | Login hash comparison is not constant-time; unknown-email path can dereference a null user | ADR-0007 F | Backlog | low | unassigned |
| TD-038 | `UserApi.ReadAllUser` returns every user object including the password hash | ADR-0007 G | Backlog | high | unassigned |
| TD-039 | CORS configured as `*` in the sample settings | ADR-0007 H | Backlog | medium | unassigned |

## Status

Reviewed at MVP-001 close-out (Phase 8): every row has a final disposition
(fixed in MVP-001, accepted via an ADR, or backlog).

## Suggested follow-up MVPs

The backlog groups naturally into a few scoped increments:

| Candidate MVP | Rows | Notes |
|---------------|------|-------|
| **[MVP-002 — `judotech.api` security hardening](../mvp/002-api-security-hardening.md)** ([plan](../plans/002-api-security-hardening.plan.md)) | TD-032…TD-039, TD-045, TD-002 (min) | Planned. Prerequisite before the portal ships any authenticated end-user feature (ADR-0001, ADR-0007). |
| **MVP-003 — `judotech.api` structure** | TD-002, TD-003, TD-045 | DI everywhere + repository boundary; remove the in-memory caches. May fold into MVP-002. |
| **`@judotech/ui` adoption** | TD-020, TD-021, TD-041 | Establish the template's licence, then export/type/test components as they are actually used. |
| **Portal tidy-up** (small, opportunistic) | TD-042, TD-043, TD-044 | Prune unused devDeps, address audit advisories. |
| **Legacy static sites** | TD-017 | Deployment workflow coverage — only if the Gulp/Pug sites are being kept rather than folded into the portal. |
| **Product questions** | TD-006, TD-028, TD-029, TD-030, TD-031 | Translation, email confirmation, member import, Smoothcomp intent, NFR monitoring — need product input, not just engineering. |

New debt discovered later is appended with the next free `TD-0NN` id.
