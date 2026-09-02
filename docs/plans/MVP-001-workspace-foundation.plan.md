# Implementation Plan — MVP-001: Workspace Foundation

Source MVP: `docs/mvp/MVP-001-workspace-foundation.md`
Architecture reference: `docs/architecture/overview.md`
Target executor: Cursor (stepwise, one phase per commit)
Branch: continue on `introduce-claude` (or a fresh `feature/workspace-foundation` cut from it)

---

## 1. Goal

Turn the repository into a consistent, documented, testable foundation before any
new application work starts. Concretely, after this plan:

- The direction of the codebase (`source/` vs `judotech-portal/`) is decided and
  recorded.
- Documentation lives only under `docs/`, is internally consistent, and has an
  entry point and the stub structure the workflow prompts expect.
- Coding and testing standards match the languages actually in use (C# / .NET 8
  and TypeScript / React).
- Toolchain versions (.NET, Node.js) are pinned and match CI.
- The `judotech-portal` workspace has shared config, a working root build, no
  stale dependencies, and a place for shared domain code.
- Every actively developed component has a working install / build / run / test
  loop, and at least one real automated test.
- CI validates build + tests for all active components on pull requests, and
  deployment is a separate, gated step.
- Every open issue from the architecture overview has a disposition (fixed,
  backlogged, or accepted via ADR).

Non-goals: new features, rewriting the .NET API or the Gulp/Pug sites, building
out `@judotech/ui`, infrastructure changes beyond gating existing deploys.

---

## 2. Assumptions

1. **Environment**: the executor has (or can install) .NET SDK 8, Node.js LTS,
   `func` (Azure Functions Core Tools), `git`, and internet access for package
   installs. Cosmos DB is **not** available locally; tests that need it are
   marked and skipped by default.
2. **Package manager for the portal is npm** (a `package-lock.json` already
   exists at `judotech-portal/`). No switch to pnpm/yarn.
3. **`source/` becomes maintenance-only**; `judotech-portal/` is the active
   development target. The existing `judotech.api` remains the backend the portal
   will consume. This is ratified in ADR-0001 (Phase 2); if the owner rejects it,
   Phases 5–7 scope changes — see Risks.
4. **Language policy = English** for all code, identifiers, comments, commit
   messages and documentation; Swedish is allowed only in domain/business data
   and (later) user-facing UI copy. Ratified in ADR-0003.
5. **Test frameworks**: xUnit for .NET, Vitest + React Testing Library for the
   portal. Ratified in ADR-0005.
6. **Turborepo is adopted** for portal task orchestration (the root `build`
   script already assumes it). Ratified in ADR-0004.
7. **Node.js is pinned to 22 LTS** (current CI uses 20; this plan bumps CI to 22).
   If the owner wants to stay on 20, change the pin in Phase 4 and Phase 7.
8. `main` is protected or will be; the executor does **not** merge or push to
   `main` without explicit approval (per `docs/standards/git.md`).
9. Existing secrets in GitHub Actions (`secrets.judotech`, `secrets.judotech_FFFF`,
   `secrets.AZURE_CREDENTIALS`) stay as-is; this plan only reorganizes when they
   are used.
10. The empty `documentation/` directory on disk is leftover from a rename and
    can be deleted.
11. It is acceptable to add new `*.tests` projects to `source/judotech.sln`.
12. No production deploy happens during this MVP.

---

## 3. Proposed file changes

Legend: **A** = add, **M** = modify, **D** = delete.

### Documentation

| Change | Path | Purpose |
|---|---|---|
| D | `documentation/` (empty dir) | Remove leftover duplicate tree |
| A | `docs/README.md` | Documentation entry point / reading order |
| M | `docs/architecture/overview.md` | Fix `documentation/` → `docs/` paths; update to post-decision state; add disposition note |
| A | `docs/architecture/decisions/README.md` | ADR index + process |
| A | `docs/architecture/decisions/adr-template.md` | ADR template |
| A | `docs/architecture/decisions/0001-source-vs-portal-scope.md` | Direction decision |
| A | `docs/architecture/decisions/0002-documentation-location.md` | Docs live in `docs/` |
| A | `docs/architecture/decisions/0003-language-policy.md` | English-only code + docs |
| A | `docs/architecture/decisions/0004-frontend-workspace.md` | npm workspaces + Turborepo + structure |
| A | `docs/architecture/decisions/0005-test-frameworks.md` | xUnit + Vitest |
| A | `docs/architecture/decisions/0006-environments-and-deployment.md` | Single prod, gated manual deploy, CI never deploys |
| A | `docs/architecture/decisions/0007-source-auth-review.md` | Auth scheme review outcome |
| A | `docs/architecture/technical-debt.md` | Register: every overview §11/§13 item + disposition |
| A | `docs/development/README.md` | Development process (branching, review, tests/docs required, AI rules) |
| A | `docs/development/setup.md` | Clone-to-running per component |
| A | `docs/development/ci-cd.md` | Workflows, secrets, branch protection |
| A | `docs/development/workflow.md` | MVP → plan → implement → complete loop |
| M | `docs/standards/coding.md` | Reframe as shared principles; link per-stack standards |
| A | `docs/standards/coding-dotnet.md` | C# / .NET conventions matching real code |
| A | `docs/standards/coding-typescript-react.md` | TS / React conventions matching real code |
| M | `docs/standards/testing.md` | Per-stack frameworks, commands, requirements |
| M | `docs/standards/documentation.md` | Correct any `docs/` path drift; keep |
| M | `docs/standards/git.md` | Replace `pytest` pre-commit wording with per-stack checks |
| M | `docs/mvp/MVP-001-workspace-foundation.md` | Status → in progress → done; check acceptance list |
| A | `docs/dependencies/README.md` | Dependency policy (pinning, review, when an ADR is needed) |

### Repo-root tooling

| Change | Path | Purpose |
|---|---|---|
| A | `.gitignore` (repo root) | Root ignore rules (node_modules, bin/obj, dist, .env, local.settings.json, coverage, TestResults, .vs, .idea, .DS_Store) |
| A | `.editorconfig` | Shared formatting rules (C#, TS/JS, JSON, MD) |
| A | `.nvmrc` | Pin Node.js version |
| M | `.vscode/settings.json` | Add format-on-save, eslint, omnisharp/roslyn settings; keep existing keys |
| A | `.vscode/extensions.json` | Recommended extensions |
| A | `.github/workflows/security-secret-scan.yml` | gitleaks scan on PR/push |
| A | `.github/pull_request_template.md` | PR description checklist referencing plan/MVP |

### .NET (`source/`)

| Change | Path | Purpose |
|---|---|---|
| A | `source/global.json` | Pin .NET SDK to 8.0.x |
| A | `source/.editorconfig` | C#-specific analyzer/style rules (or rely on root) |
| A | `source/Directory.Build.props` | Shared `LangVersion`, `Nullable`, `TreatWarningsAsErrors` (opt-in), analyzers |
| A | `source/judotech.core.tests/judotech.core.tests.csproj` | xUnit test project |
| A | `source/judotech.core.tests/HashPasswordTests.cs` | Real unit test (pure function) |
| A | `source/judotech.core.tests/SettingsTests.cs` | Container-mapping test |
| A | `source/judotech.api.tests/judotech.api.tests.csproj` | xUnit test project |
| A | `source/judotech.api.tests/AuthenticationIntegrationTests.cs` | Port of `TestAuthenticationApi`, `[Trait("Category","Integration")]`, skipped without Cosmos |
| M | `source/judotech.sln` | Add the two test projects |
| M | `source/judotech.api/AuthenticatorApi.cs` | Remove `TestAuthenticationApi` `[Function]` (logic moved to test) |
| M | `source/judotech.api/local.settings_sample.json` | Document every env var the code reads; drop unused keys or comment them |
| A | `source/README.md` | Mark `source/` maintenance-only; point to ADR-0001 |

### `judotech-portal/`

| Change | Path | Purpose |
|---|---|---|
| M | `judotech-portal/package.json` | Add `packageManager`, `engines`, root scripts (`lint`, `test`, `typecheck`, `dev`, `build`), devDep `turbo` |
| A | `judotech-portal/turbo.json` | Pipelines: `build`, `lint`, `test`, `typecheck` |
| A | `judotech-portal/tsconfig.base.json` | Shared compiler options |
| A | `judotech-portal/tsconfig.json` | Solution-style references to packages/apps (for root `typecheck`) |
| A | `judotech-portal/.nvmrc` | Match root Node pin |
| A | `judotech-portal/packages/config/package.json` | `@judotech/config` |
| A | `judotech-portal/packages/config/eslint.base.js` | Shared flat ESLint config |
| A | `judotech-portal/packages/config/tsconfig.base.json` | Re-export / base tsconfig |
| A | `judotech-portal/packages/config/tailwind-theme.css` | Shared Tailwind v4 theme tokens |
| A | `judotech-portal/packages/config/vitest.base.ts` | Shared Vitest config |
| M | `judotech-portal/packages/ui/package.json` | Remove `clean`; add `exports`/`types`, `scripts` (`lint`, `test`, `typecheck`); devDeps for test |
| A | `judotech-portal/packages/ui/tsconfig.json` | Extends base; enables project reference |
| A | `judotech-portal/packages/ui/eslint.config.js` | Extends `@judotech/config` |
| A | `judotech-portal/packages/ui/vitest.config.ts` | Extends base |
| A | `judotech-portal/packages/ui/src/components/Button.test.tsx` | Real test |
| M | `judotech-portal/packages/ui/src/styles/index.css` | Import shared theme from `@judotech/config` (or leave, see Risks) |
| A | `judotech-portal/packages/core/package.json` | `@judotech/core` |
| A | `judotech-portal/packages/core/tsconfig.json` | Extends base |
| A | `judotech-portal/packages/core/eslint.config.js` | Extends `@judotech/config` |
| A | `judotech-portal/packages/core/vitest.config.ts` | Extends base |
| A | `judotech-portal/packages/core/src/index.ts` | Public surface (minimal) |
| A | `judotech-portal/packages/core/src/api/http-client.ts` | Minimal fetch wrapper (no business logic) |
| A | `judotech-portal/packages/core/src/api/http-client.test.ts` | Real test |
| M | `judotech-portal/apps/athlete/package.json` | Remove `@types/react-router-dom`; add `test`/`typecheck` scripts; add Vitest + Testing Library devDeps |
| M | `judotech-portal/apps/athlete/tsconfig.app.json` | Extend base; add project reference to `packages/*` |
| M | `judotech-portal/apps/athlete/eslint.config.js` | Extend `@judotech/config` |
| A | `judotech-portal/apps/athlete/vitest.config.ts` | Extends base; jsdom |
| A | `judotech-portal/apps/athlete/src/test/setup.ts` | Testing Library setup |
| A | `judotech-portal/apps/athlete/src/pages/Dashboard.test.tsx` | Real test |
| M | `judotech-portal/apps/athlete/postcss.config.mjs` + `.bak` | Resolve the `.bak` duplication (keep one, delete `.bak`) |
| M | `judotech-portal/readme.md` | Update to reflect what actually exists after this MVP |

### CI/CD (`.github/workflows/`)

| Change | Path | Purpose |
|---|---|---|
| A | `.github/workflows/ci-dotnet.yml` | PR/push: restore + build + test `source/judotech.sln` on .NET 8, no deploy |
| A | `.github/workflows/ci-portal.yml` | PR/push: `npm ci` + `turbo lint typecheck test build` |
| A | `.github/workflows/ci-web-legacy.yml` | PR/push: build all 4 Gulp sites (`gulp --production`), no upload |
| M | `.github/workflows/deploy_function.yml` | `.NET 8`, `environment: production`, stays `workflow_dispatch` |
| M | `.github/workflows/deploy_web.yml` | `environment: production`, matrix/site param, stays `workflow_dispatch` |
| D | `.github/workflows/ci_api.yml` | Replaced by `ci-dotnet.yml` + `deploy_function.yml` |
| D | `.github/workflows/ci_web.yml` | Replaced by `ci-web-legacy.yml` + `deploy_web.yml` |
| M | `codeql.yml` → move to `.github/workflows/codeql.yml` | Fix undefined `matrix.language`, drop .NET 3.1, analyze `csharp` + `javascript`, update action versions |

---

## 4. Step-by-step TODO grouped into phases

> Rules for the executor:
> - Do one phase at a time. Run the phase's verification before committing.
> - Commit with the exact message under each phase heading. Do not push or open a
>   PR without approval.
> - If a step cannot be completed (missing tool, failing command that is not
>   caused by your change), stop and record it under Risks / open questions in
>   this file, then continue only if the owner agrees.
> - ADRs in Phase 2 contain **recommended** decisions. Flag each `Status:
>   proposed` and list it in the PR description for owner ratification; proceed on
>   the recommendation so later phases are unblocked.

---

### Phase 1 — Documentation structure — DONE (commit `b5fbae3`)

Commit message: `Consolidate documentation under docs/ and add foundation stubs`

- [x] 1.1 Confirm `documentation/` on disk is empty (`git ls-files documentation/`
      returns nothing; directory has no files). Delete the empty directory.
- [x] 1.2 In `docs/architecture/overview.md`, replace every `documentation/`
      path reference with `docs/`. Update the tree diagram: the top-level doc
      folder is `docs/`, not `documentation/`. Remove the "(note: not docs/)"
      aside.
- [x] 1.3 Add `docs/README.md`: one paragraph on what `docs/` contains, plus a
      recommended reading order (overview → standards → development → decisions →
      roadmap → mvp → plans). List each subfolder with a one-line purpose.
- [x] 1.4 Create `docs/architecture/decisions/` with:
      - `README.md` — what an ADR is, numbering, statuses (proposed / accepted /
        superseded), how to add one.
      - `adr-template.md` — sections: Title, Status, Context, Decision,
        Consequences, Alternatives considered.
- [x] 1.5 Create `docs/development/` with placeholder files that state their
      purpose and a "TODO: filled in Phase 3/7" note:
      `README.md`, `setup.md`, `ci-cd.md`, `workflow.md`.
- [x] 1.6 Create `docs/dependencies/README.md` as a stub ("filled in Phase 8").
- [x] 1.7 Fix any stale path references in `docs/standards/documentation.md` and
      `docs/standards/git.md` that point at a non-`docs/` location (do not change
      their substance yet). — Note: `git.md` had no non-`docs/` path references;
      fixed one real mismatch in `documentation.md` (`docs/roadmap/` →
      `docs/roadmap.md`).
- [x] 1.8 Add `.github/pull_request_template.md` referencing the MVP/plan and a
      short checklist (tests pass, docs updated, no secrets, scope matches plan).
- [x] **Verify**: `find docs -type f` shows the new structure; no `documentation/`
      directory remains; `grep -rn "documentation/" docs/` returns nothing except
      intentional prose. Markdown renders (no broken relative links —
      spot-check in editor preview).

---

### Phase 2 — Architecture decisions — DONE (staged, not committed)

Commit message: `Record foundational architecture decisions (ADR 0001-0007)`

- [x] 2.1 Write `0001-source-vs-portal-scope.md` (Status: proposed). Decision:
      `source/` is maintenance-only (security/critical fixes to `judotech.api`
      and `judotech.web` only); `judotech-portal/` is the active target; the
      portal consumes the existing `judotech.api`; a future ADR will decide any
      API rewrite. List which paths are "active" vs "frozen".
- [x] 2.2 Write `0002-documentation-location.md` (accepted). Decision: all
      project docs under `docs/`; `documentation/` removed.
- [x] 2.3 Write `0003-language-policy.md` (proposed). Decision: English for code,
      identifiers, comments, commits, docs; Swedish only in business data files
      and future UI copy. Consequence: existing Swedish docs
      (`functional-requirements.md`, `non-functional-requirements.md`,
      `features.md`) get translated — add that as a technical-debt item, not a
      blocker.
- [x] 2.4 Write `0004-frontend-workspace.md` (proposed). Decision: npm workspaces
      (keep), adopt Turborepo for task running, `packages/{ui,core,config}`,
      `apps/{public,athlete,trainer,referee}` created on demand, `@judotech/*`
      scope, packages consumed from source within the monorepo.
- [x] 2.5 Write `0005-test-frameworks.md` (proposed). Decision: xUnit (.NET),
      Vitest + React Testing Library (portal); integration tests that need Cosmos
      are `[Trait("Category","Integration")]` / `describe.skip` by default.
- [x] 2.6 Write `0006-environments-and-deployment.md` (proposed). Decision: one
      `production` environment for now; all deploy workflows are
      `workflow_dispatch` only and bound to a protected GitHub `production`
      environment; CI workflows never deploy; Test/UAT tiers deferred.
- [x] 2.7 Write `0007-source-auth-review.md` (proposed). Record findings from
      `overview.md` + code: static hard-coded salt in `DbLogin.HashPassword`,
      SQL built by string concatenation in `GetUserFromToken`, no token expiry,
      `AuthorizationLevel.Function` only. Decision: acceptable for current
      internal/low-volume use; **must** be hardened before the portal exposes
      auth to end users. Each item becomes a technical-debt entry; not an MVP
      blocker.
- [x] 2.8 Create `docs/architecture/technical-debt.md`: a table with columns
      _ID, Source (overview §), Description, Disposition (fixed in MVP-001 /
      backlog / accepted), Owner, Notes_. Populate one row for every bullet in
      `overview.md` sections 11 and 13, and for the ADR-0007 items. — Done as
      TD-001..TD-039, grouped by area, with a Severity column added.
- [x] 2.9 Update `docs/architecture/overview.md`: add a short "Decisions" section
      linking ADR-0001..0007 and `technical-debt.md`; adjust section 1 and
      section 11/13 wording to say the items now have dispositions. — Section
      intros updated; individual §11/§13 bullets kept as the historical record
      (deeper per-bullet rewrite left for Phase 8).
- [x] 2.10 Update `docs/mvp/MVP-001-workspace-foundation.md` status to
      `in progress`.
- [x] **Verify**: every ADR file has all template sections filled; every
      overview §11/§13 bullet appears in `technical-debt.md`; links resolve.

---

### Phase 3 — Standards alignment — DONE (staged, not committed)

Commit message: `Align coding, testing and process standards with the real stack`

- [x] 3.1 Rewrite `docs/standards/coding.md` as **stack-agnostic principles
      only** (correctness, clarity, SOLID-where-useful, small functions, comment
      the why, no silent catch, no secrets in logs, tests for reusable logic).
      Remove Python-specific rules (`snake_case`, `pyproject.toml`,
      `service.py/repository.py`, `T | None`). Add a "Language-specific
      standards" section linking the two new files.
- [x] 3.2 Add `docs/standards/coding-dotnet.md`: target `net8.0`, nullable
      enabled, `PascalCase` types/methods, `camelCase` locals, file-scoped
      namespaces, `async` suffix, one type per file, `Directory.Build.props`
      analyzers, `dotnet format` before commit, no `catch {}` that swallows.
      Note current deviations to fix opportunistically (Active Record, static
      salt, string-built queries) with links to `technical-debt.md`.
- [x] 3.3 Add `docs/standards/coding-typescript-react.md`: TS strict, ESLint flat
      config from `@judotech/config`, Prettier for formatting, `PascalCase`
      components + files, hooks rules, no default exports for shared library
      code (allow for app pages), Tailwind for styling, path alias `@judotech/*`.
- [x] 3.4 Rewrite `docs/standards/testing.md`: table of _stack → framework →
      test file pattern → command_. .NET: xUnit, `*Tests.cs`, `dotnet test`.
      Portal: Vitest, `*.test.ts(x)`, `npm test` / `turbo test`. Keep the
      "regression test for every bug fix, tests for every feature" principles.
      Add "Integration tests requiring Cosmos are skipped in CI" note.
- [x] 3.5 Edit `docs/standards/git.md`: replace "run `pytest` before commit" with
      "run the checks for the stack you changed (`dotnet test` / `npm test`)
      before commit"; keep everything else. (Also updated the daily-workflow
      snippet.)
- [x] 3.6 Fill `docs/development/README.md` (development process): branching model
      (`feature/ fix/ docs/ refactor/ chore/` off `main`), review expectations,
      when docs are required, when tests are required, AI-assistant rules (mirror
      `.claude/settings.json` and `git.md`), definition of done.
- [x] 3.7 Fill `docs/development/workflow.md`: the MVP → plan → implement →
      complete loop, which folders hold what (`docs/mvp/`, `docs/plans/`), how a
      phase maps to a commit, how `technical-debt.md` is used.
- [x] **Verify**: `grep -rn "pytest\|pyproject\|snake_case" docs/standards/`
      returns only intentional historical mentions (ideally none);
      `coding.md` links resolve to the two new files. — No matches in
      `docs/standards/`; all `coding.md` links resolve. Also refreshed
      overview.md sections 6 and 9 which described the old standards.

---

### Phase 4 — Toolchain pinning & editor baseline — DONE (staged, not committed)

Commit message: `Pin .NET and Node toolchains and add shared editor config`

Environment found: .NET SDKs 3.1.426 / 9.0.315 / 9.0.317 (**no 8.0.x**);
Node v24.4.1; npm 11.4.2.

- [x] 4.1 Add repo-root `.gitignore` covering: `node_modules/`, `dist/`,
      `dist-ssr/`, `**/bin/`, `**/obj/`, `.vs/`, `.idea/`, `.DS_Store`,
      `*.user`, `**/local.settings.json`, `.env`, `.env.*`, `!.env.sample`,
      `**/TestResults/`, `coverage/`, `*.log`, `.turbo/`. Keep the existing
      per-project `.gitignore` files.
- [x] 4.2 Add `source/global.json`. Used
      `{ "sdk": { "version": "8.0.0", "rollForward": "major", "allowPrerelease": false } }`
      — requests 8.0 (which CI installs) and rolls forward to 9.x locally since
      no 8.0.x is installed. Verified: `dotnet --version` in `source/` → 9.0.317,
      no "SDK not found" error.
- [x] 4.3 Add `source/Directory.Build.props` with shared `<Nullable>`,
      `<ImplicitUsings>`, `<LangVersion>latest</LangVersion>`, `<AnalysisLevel>latest</AnalysisLevel>`,
      `<EnableNETAnalyzers>`. `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>`
      explicit. Scoped away from the legacy `Judoka.VideoStream*` projects by
      `MSBuildProjectName` condition.
- [x] 4.4 Add `.nvmrc` (repo root) and `judotech-portal/.nvmrc`. **Pinned `24`,
      not `22`** — matches the dev machine and current LTS (see Risk 5 update).
- [x] 4.5 Add `judotech-portal/package.json` fields: `"engines": { "node": ">=22" }`,
      `"packageManager": "npm@11.4.2"`.
- [x] 4.6 Add `.editorconfig` (repo root) with the core rules plus a `[*.cs]`
      section carrying a few analyzer/style preferences. Also added
      `.markdownlint.json` (disables MD013/MD029/MD060 etc.) so the docs stop
      generating editor warnings.
- [x] 4.7 Add `.vscode/extensions.json` (`ms-dotnettools.csharp`,
      `ms-azuretools.vscode-azurefunctions`, `dbaeumer.vscode-eslint`,
      `esbenp.prettier-vscode`, `bradlc.vscode-tailwindcss`,
      `editorconfig.editorconfig`).
- [x] 4.8 Update `.vscode/settings.json` — existing keys preserved; added
      format-on-save, eslint fix-on-save, `files.eol`, per-language formatters,
      `eslint.workingDirectories` for `judotech-portal`.
- [x] 4.9 Added a Prerequisites section to `docs/development/setup.md` (pinned
      versions table) and a "Dev container" note that the Dockerfile does not yet
      bundle .NET / Node. Dockerfile left unchanged (not blocking).
- [~] **Verify**: `dotnet --version` respects `global.json` → **OK** (9.0.317).
      `git check-ignore` resolves → **OK**. `node -v` (24) matches `.nvmrc` →
      **OK**. `dotnet build source/judotech.sln` → **FAILS**, but with the
      *pre-existing* 53 errors / 6 warnings (identical count before and after
      Phase 4). `Directory.Build.props` did not regress anything. Root cause is
      TD-040 (`judotech.core` broken by commit `f0b94b4`) — see Risk 15.

---

### Phase 5 — Portal workspace structure — DONE (staged, not committed)

Commit message: `Add shared portal config, working root build, and packages/core`

> **Phase 5 addendum (separate commit): fix the `judotech.core` build (TD-040).**
> At the owner's request, `source/judotech.core/DbUser.cs` was restored to a
> superset of the pre- and post-`f0b94b4` property shapes so every call site
> compiles again. `dotnet build source/judotech.sln` → 0 errors; `func start`
> hosts all 15 functions; `GET /api/HashPassword` returns 200. Suggested commit
> message: `Fix judotech.core build by restoring removed DbUser members (TD-040)`.
> This unblocks Phases 6 and 7's .NET work.

- [x] 5.1 Add `judotech-portal/tsconfig.base.json` with the common options
      currently duplicated in `apps/athlete/tsconfig.*`.
- [x] 5.2 Create `packages/config` (`@judotech/config`).
      **Deviations:** no `tsconfig.base.json` in the package — the canonical base
      is `judotech-portal/tsconfig.base.json` and packages extend it by relative
      path (avoids package-`exports`-for-tsconfig complexity). No
      `tailwind-theme.css` — the ~780-line `packages/ui/src/styles/index.css`
      stays the theme source (Risk 8 fallback taken; theme extraction → TD).
      `vitest.base.js` (not `.ts`) exports a plain `testBase` object that
      packages spread into their own config. Exports: `./eslint`, `./vitest`.
- [x] 5.3 Add root `judotech-portal/turbo.json` (turbo 2.x `tasks` schema):
      `build`/`lint`/`typecheck`/`test` all `dependsOn: ["^build"]`, `build`
      outputs `dist/**`; `dev` non-cached + persistent.
- [x] 5.4 Update root `judotech-portal/package.json` scripts (`dev`/`build`/
      `lint`/`typecheck`/`test` → `turbo *`; kept `*:athlete`). Added
      `turbo` ^2 devDep (resolved to 2.10.12).
- [x] 5.5 Add `judotech-portal/tsconfig.json` (solution file, `files: []`,
      references to `packages/core`, `packages/ui`, `apps/athlete`).
      **Deviation:** root `typecheck` runs via `turbo typecheck` (each package
      has its own `tsc` script) rather than a root `tsc -b`, which avoids
      `composite` requirements on the non-composite `noEmit` projects.
- [x] 5.6 Refactor `apps/athlete`: `tsconfig.app.json` extends base (dropped
      `baseUrl` — deprecated in TS 7; `paths` now relative to the tsconfig);
      `eslint.config.js` re-exports `@judotech/config/eslint`; removed
      `@types/react-router-dom`; added `typecheck`/`test` scripts, `@judotech/ui`
      + `@judotech/core` deps, `@judotech/config` + vitest/testing-library/jsdom
      devDeps. `postcss.config.mjs.bak` deleted (`git rm`); there is no
      `postcss.config.mjs` — Tailwind v4 runs via `@tailwindcss/vite`, no PostCSS
      config needed.
- [x] 5.7 Create `packages/core` (`@judotech/core`): `package.json`,
      `src/index.ts` (`HttpClient`, `HttpError`, `createHttpClient`, `User`
      placeholder), `src/api/http-client.ts` (fetch wrapper — baseUrl, JSON,
      error normalisation), `tsconfig.json`, `eslint.config.js`,
      `vitest.config.ts`. (Had to expand a parameter-property constructor —
      disallowed by `erasableSyntaxOnly`.)
- [x] 5.8 Refactor `packages/ui`: removed `clean`; added scripts + devDeps;
      added `tsconfig.json`, `eslint.config.js` (turns off
      `react-refresh/only-export-components` — it's a library not an HMR app —
      and downgrades `react-hooks/set-state-in-effect` to warn for the
      unadopted template code, TD-020/021), `vitest.config.ts`, `vitest.setup.ts`,
      and **`svg.d.ts`** (needed so `tsc` resolves the `*.svg?react` imports in
      `src/icons`). `exports` map for `.` and `./styles/index.css`. Export
      surface unchanged.
- [x] 5.9 Ran `npm install` — added 155 packages, removed 9 (`clean` + deps,
      `@types/react-router-dom`). Lockfile updated. (13 npm-audit advisories,
      pre-existing / dev-only — noted, not addressed.)
- [x] 5.10 Rewrote `judotech-portal/readme.md` (real structure, `turbo`
      commands, known gaps). Scratch notes dropped.
- [x] **Verify** (from `judotech-portal/`):
      `npm run lint` → **pass** (`@judotech/ui` 2 warnings, non-blocking).
      `npm run typecheck` → **pass** (3/3).
      `npm run build` → **pass**, `apps/athlete/dist` produced (95 modules).
      `npm run dev:athlete` → **pass** (HTTP 200, serves `main.tsx`; full React
      render not asserted — no headless browser).
      `npm test` → **pass** (3/3, no test files yet — Phase 6).
      `grep clean packages/ui/package.json` → nothing.
      `grep react-router-dom apps/athlete/package.json` → only `^7.9.6`.

---

### Phase 6 — Testing setup

Commit message: `Add xUnit and Vitest test suites with a passing test per component`

- [ ] 6.1 Create `source/judotech.core.tests`:
      - `dotnet new xunit -o source/judotech.core.tests` then adjust
        `TargetFramework` to `net8.0` and add a `ProjectReference` to
        `judotech.core`.
      - `HashPasswordTests.cs`: assert `DbLogin.HashPassword("x")` is
        non-empty, deterministic (same input → same output), and differs for
        different input.
      - `SettingsTests.cs`: assert `Settings.Instance.Containers` maps
        `Users`→`/email`, `Competitions`→`/name`, `Logins`→`/email`.
- [ ] 6.2 Create `source/judotech.api.tests`:
      - xUnit project, `ProjectReference` to `judotech.api` and `judotech.core`.
      - `AuthenticationIntegrationTests.cs`: move the body of
        `AuthenticatorApi.TestAuthenticationApi` here as a `[Fact]` decorated
        with `[Trait("Category", "Integration")]`; use a skip guard when
        `EndpointUrl`/`PrimaryKey` env vars are absent
        (`Skip.If`-style via `Xunit.SkippableFact`, or a manual
        `if (...) return;` with an explanatory `Assert.True(true)` — prefer
        `Xunit.SkippableFact`).
- [ ] 6.3 `dotnet sln source/judotech.sln add source/judotech.core.tests
      source/judotech.api.tests`.
- [ ] 6.4 Remove the `[Function("TestAuthenticationApi")]` method from
      `source/judotech.api/AuthenticatorApi.cs` (logic now lives in the test).
      Leave the other functions untouched.
- [ ] 6.5 Portal — shared test config already in `packages/config/vitest.base.ts`
      (Phase 5). Now add per-package wiring:
      - `apps/athlete`: `vitest.config.ts` (jsdom env), `src/test/setup.ts`
        (`import '@testing-library/jest-dom'`), `src/pages/Dashboard.test.tsx`
        rendering `<Dashboard/>` and asserting it shows `test`.
      - `packages/ui`: `src/components/Button.test.tsx` rendering `<Button/>`
        and asserting it is in the document.
      - `packages/core`: `src/api/http-client.test.ts` using a mocked `fetch`.
- [ ] 6.6 Add `"test"` scripts everywhere they are missing so `turbo test`
      fans out.
- [ ] 6.7 Update `docs/standards/testing.md` and `docs/development/setup.md` with
      the exact commands.
- [ ] **Verify**:
      `dotnet test source/judotech.sln` → all pass (integration test reported
      as skipped, not failed);
      `cd judotech-portal && npm test` → all pass;
      total: at least one real passing test in `judotech.core`, `judotech.api`
      (skipped-integration counts as wired), `athlete`, `ui`, `core`.

---

### Phase 7 — CI/CD separation

Commit message: `Split CI from deployment and add portal and legacy-web pipelines`

- [ ] 7.1 Add `.github/workflows/ci-dotnet.yml`: triggers `pull_request` +
      `push` to `main`; `ubuntu-latest`; `actions/setup-dotnet@v4` with
      `dotnet-version: 8.0.x`; steps: `dotnet restore`, `dotnet build -c Release
      --no-restore`, `dotnet test -c Release --no-build --filter
      Category!=Integration`. No Azure steps.
- [ ] 7.2 Add `.github/workflows/ci-portal.yml`: triggers `pull_request` +
      `push` to `main`; `actions/setup-node@v4` with `node-version-file:
      judotech-portal/.nvmrc` and `cache: npm`; `working-directory:
      judotech-portal`; steps: `npm ci`, `npm run lint`, `npm run typecheck`,
      `npm run test`, `npm run build`.
- [ ] 7.3 Add `.github/workflows/ci-web-legacy.yml`: `pull_request` + `push` to
      `main`; matrix over `[judotech.web, judotech.web.calendar,
      judotech.web.club, judotech.web.referee]`; `npm ci || npm install` +
      `npx gulp --production` in each `source/<site>`; no upload.
- [ ] 7.4 Modify `.github/workflows/deploy_function.yml`: keep
      `on: workflow_dispatch`; add `environment: production` to the job; bump
      `setup-dotnet` to `8.0.x`; keep publish + `Azure/functions-action` with
      the existing secret.
- [ ] 7.5 Modify `.github/workflows/deploy_web.yml`: keep `workflow_dispatch`;
      add `environment: production`; optionally add a `site` input
      (default `judotech.web`); keep the blob upload + `az logout`.
- [ ] 7.6 Delete `.github/workflows/ci_api.yml` and
      `.github/workflows/ci_web.yml`.
- [ ] 7.7 Move `codeql.yml` → `.github/workflows/codeql.yml` and fix it:
      remove `dotnet-version: 3.1` / `3.1.301`; set
      `matrix: { language: ['csharp', 'javascript-typescript'] }`; use
      `github/codeql-action/*@v3`; add `actions/setup-dotnet@v4` (8.0.x) only for
      the `csharp` leg; `dotnet build source/judotech.sln` for autobuild input.
- [ ] 7.8 Fill `docs/development/ci-cd.md`: table of every workflow (trigger,
      what it does, does it deploy), the list of secrets and what each is for,
      and the required branch-protection settings for `main` (require
      `ci-dotnet`, `ci-portal`, `ci-web-legacy`, `CodeQL` to pass; no direct
      pushes; linear history optional).
- [ ] 7.9 Add `.github/workflows/security-secret-scan.yml`: `gitleaks/gitleaks-action`
      on `pull_request` + `push`.
- [ ] **Verify**: `actionlint` (if available) passes on all workflow files, or
      YAML parses cleanly; grep confirms no workflow triggered by `push` contains
      an `Azure/` or `az storage` deploy step; `ci-*` workflows reference
      `.nvmrc` / `8.0.x`. (Actual green runs require pushing the branch — do that
      only with approval; note the result here afterward.)

---

### Phase 8 — MVP close-out

Commit message: `Complete MVP-001 workspace foundation and record dispositions`

- [ ] 8.1 Fill `docs/dependencies/README.md`: pin policy (exact for tools via
      `global.json`/`.nvmrc`/`packageManager`; caret ranges for libs; lockfiles
      committed), update cadence, when a new dependency needs an ADR, how to
      record a removal. Note the resolved items (`clean`, `@types/react-router-dom`)
      and the .NET commented-out package refs in `judotech.core.csproj` (decide:
      delete the comments).
- [ ] 8.2 Verify lockfiles: `judotech-portal/package-lock.json` present and in
      sync (`npm ci` succeeds from clean); each `source/*/packages.config` or
      `*.csproj` restores. Record any package manager without a committed lock.
- [ ] 8.3 Fill `docs/development/setup.md` for real: prerequisites, then a
      copy-pasteable "clone to running" block for each active component
      (`judotech.api`, `judotech-portal` athlete app) and each maintenance
      component (the 4 static sites). Actually run each block on a clean checkout
      / clean `node_modules` and fix the docs until they work. Record which were
      verified and on what OS.
- [ ] 8.4 Confirm `source/judotech.api/local.settings_sample.json` lists exactly
      the env vars the code reads (`EndpointUrl`, `PrimaryKey`, `DatabaseId`;
      `Host.CORS`). Remove or comment the unused `*ContainerId` keys. Ensure
      `.gitignore` excludes `local.settings.json`.
- [ ] 8.5 Walk `docs/architecture/technical-debt.md`: set the final Disposition
      for every row (fixed in MVP-001 / backlog / accepted). For "backlog" rows,
      make sure they are specific enough to become future MVPs.
- [ ] 8.6 Update `docs/architecture/overview.md` "Decisions" and sections 11/13
      to point at `technical-debt.md` and note the MVP-001 outcome.
- [ ] 8.7 Update `docs/mvp/MVP-001-workspace-foundation.md`: status `done`; walk
      the 26 acceptance criteria and check each off with a one-line pointer to
      where it was satisfied (file / workflow / ADR). Any not met → list under a
      "Deferred" heading with rationale.
- [ ] 8.8 Change all Phase-2 ADRs from `Status: proposed` to `Status: accepted`
      **only after the owner confirms**; otherwise leave proposed and note it.
- [ ] 8.9 Update `README.md` (repo root) top section: point to `docs/README.md`
      and `docs/development/setup.md` as the entry points; keep the Azure CLI
      notes but move the long provisioning block into `docs/development/` if it
      clutters (optional).
- [ ] 8.10 Prepare PR description (do not open/merge without approval): summary of
      completed work, validation performed (commands + results), docs updated,
      remaining risks/deferred items, suggested title
      `MVP-001: Establish workspace foundation`.
- [ ] **Verify**: `docs/mvp/MVP-001-workspace-foundation.md` shows all 26
      criteria addressed or explicitly deferred; `dotnet test source/judotech.sln`
      and `cd judotech-portal && npm ci && npm run lint && npm run typecheck &&
      npm test && npm run build` all green; `git status` clean;
      no secret-bearing files staged.

---

## 5. Risks / open questions

### Decisions that need the owner (block ratification, not execution)

1. **ADR-0001 (source vs portal)** — if the owner wants the .NET API *rewritten*
   inside the portal rather than kept, Phases 5–7 change substantially (no
   `ci-dotnet` emphasis, a new `packages/api` or backend app instead). Proceeding
   on "keep and consume".
2. **ADR-0003 (language)** — English-only means the Swedish requirements docs
   should be translated. Plan treats translation as backlog, not MVP scope.
   Owner may want it in scope.
3. **ADR-0004 (Turborepo)** — adds a dependency and a config file. Alternative:
   `npm run <script> --workspaces --if-present` with no Turbo. Chosen Turbo
   because the existing root script already calls it and it scales to more apps.
4. **ADR-0006 (environments)** — plan assumes a single `production` environment
   and manual deploys. If Test/UAT are needed now, Phase 7 grows.
5. **Node.js version** — Phase 4 pinned **24** (dev machine has v24.4.1, npm
   11.4.2, and Node 24 is current LTS). The plan originally said 22; the old CI
   used 20; README mentions 24. `engines` allows `>=22`. If the owner wants 22,
   change `.nvmrc` (x2), `packageManager`, and the Phase 7 CI workflows.

### Execution risks

6. **No Cosmos DB locally** — `judotech.api` cannot be fully run or
   integration-tested by the executor. Mitigated: unit tests target pure code;
   integration tests are skipped with a guard; `setup.md` documents the Cosmos
   emulator as optional. Some acceptance verification for the API path will be
   "builds + unit tests pass" only.
7. **`dotnet build` may emit new analyzer warnings** after
   `Directory.Build.props` / `AnalysisLevel`. Plan explicitly does **not** set
   `TreatWarningsAsErrors`; warnings are logged as debt. If the build already
   fails today, Phase 4 stops and reports.
8. **Tailwind v4 shared theme** — `packages/ui/src/styles/index.css` is ~780
   lines and CSS-first. Extracting a shared `@theme` into `packages/config` may
   break the cascade / `@custom-variant dark`. Fallback: leave the UI CSS as-is,
   have `packages/config` ship only ESLint/TS/Vitest presets, and note theme
   sharing as debt. Do not spend more than one attempt here.
9. **`packages/ui` template code** — the large unexported component set (sidebar,
   header, icons) references imports/assets that may not typecheck in isolation.
   Adding `ui/tsconfig.json` + `typecheck` could surface many errors. Mitigation:
   scope `ui` `typecheck`/`lint` to the exported surface
   (`src/index.ts` + its transitive deps) via `include`, or set
   `"skipLibCheck": true` and a narrow `include`; record the rest as debt. Do
   not try to fix template code in this MVP.
10. **`rolldown-vite`** — `apps/athlete` uses `vite: npm:rolldown-vite@7.2.5`
    (pre-release). Vitest may expect standard Vite. If Vitest fails to resolve,
    pin `vitest` to a compatible version or add `vite` (standard) as a dev-only
    resolution for tests. Record if encountered.
11. **CI green cannot be fully verified without pushing.** The executor validates
    YAML and logic locally; real runs need the branch pushed, which requires
    approval. Acceptance criterion 15/16 is "workflows exist and are correct by
    inspection" until a push happens.
12. **`.sln` edits** — `dotnet sln add` rewrites GUIDs/format; verify
    `source/judotech.sln` still opens and `dotnet build` works after Phase 6.
13. **Secret scan false positives** — `gitleaks` may flag the sample
    `local.settings_sample.json` placeholder or the historical "Clean history
    (no secrets)" commit. Add a `.gitleaksignore` or config allowlist rather than
    editing history.
14. **Scope size** — this is 8 phases / ~80 steps. If time-boxed, the minimum
    viable subset for "foundation" is Phases 1–4 + 6 + 7 (docs, decisions,
    standards, pinning, tests, CI). Phase 5 (portal restructure) and Phase 8
    polish can be a follow-up MVP if needed — but then acceptance criteria
    18–21 slip.
15. **~~`source/judotech.core` does not compile (TD-040) — BLOCKER for Phases 6 &
    7's .NET work.~~ RESOLVED.** Fixed after Phase 5 at the owner's request:
    `DbUser` restored to a superset of both property shapes (option (b), minimal
    mechanical fix). `dotnet build source/judotech.sln` → 0 errors / 6
    pre-existing nullable warnings; `func start` hosts all 15 functions;
    `GET /api/HashPassword` verified. Phases 6 and 7 proceed with their .NET
    work as originally planned. Choosing one coherent `DbUser` model is now
    TD-045 (backlog). Original text kept below for context.

    Discovered during the Phase 4 build check: commit `f0b94b4`
    ("initalized claude and first code base") reworked `DbUser`'s properties
    (removed `Email`, `FullName`, `Personnumber`, `Adress`, `PostalCode`, `City`,
    `PrimaryPhone`, `SecondaryPhone`, `Attendance`, `Borde`, `Diff`, `License`,
    `Club`, `Zone`, `Roles`; added `First`, `Lastname`, `Started`, `BirthDate`,
    `Total`, `ShouldHaveGrade`) but did **not** update the constructors, the
    `DbUser(string email)` loader, `Delete()`, `CosmosDatabase` (which keys on
    `user.Email`) or `AuthenticatorApi` — 53 build errors. Options:
    - (a) Owner fixes `DbUser` and the call sites to a coherent model, then
      Phase 6/7 proceed.
    - (b) Owner asks the executor to do a mechanical fix — either re-add the
      removed properties, or trim the constructors/`Delete()` and adjust
      `CosmosDatabase`'s partition-key usage. This is more than a foundation-MVP
      concern because it changes the API's user model and Cosmos keying, so it
      needs an explicit decision (and probably its own ADR under ADR-0001's
      "maintenance fix" umbrella).
    - (c) Proceed with Phase 5 (portal, independent of .NET), and defer Phases 6
      & 7's .NET legs until the build is fixed; CI for `.NET` is added but
      expected-red until then.
    Phase 4's toolchain files were added regardless and did not regress the
    build (identical 53 errors / 6 warnings before and after).

### Open questions to resolve during execution

- Does `judotech.web` actually build with current Node 22 + `gulp 5`? (Phase 7.3
  will tell.)
- Are there GitHub `secrets` beyond the three known ones? Confirm in repo
  settings before finalizing `docs/development/ci-cd.md`.
- Should `source/judotech.VideoStream*` (net48, not in sln, Windows-only) be
  moved to an `archive/` folder or left? Recommend: note in `technical-debt.md`,
  leave in place.
- Should the `docs/claude-prompts/` folder stay under `docs/` or move to
  `.claude/`? Not blocking; recommend leaving.
