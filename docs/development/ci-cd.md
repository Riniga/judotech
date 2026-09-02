# CI/CD

All workflows live in `.github/workflows/`. Continuous integration and
deployment are separate: **CI never deploys** (ADR-0006).

## Workflows

| Workflow | Trigger | What it does | Deploys? |
|----------|---------|--------------|----------|
| `ci-dotnet.yml` | push/PR to `main`, manual | `dotnet restore` + `build -c Release` + `test --filter "Category!=Integration"` on .NET 8 | No |
| `ci-portal.yml` | push/PR to `main`, manual | `npm ci` + `lint` + `typecheck` + `test` + `build` in `judotech-portal/` (Node from `.nvmrc`) | No |
| `ci-web-legacy.yml` | push/PR to `main`, manual | `npx gulp` build of the frozen Gulp/Pug sites (`calendar`, `club`, `referee` required; `judotech.web` non-blocking — TD-046) | No |
| `codeql.yml` | push/PR to `main`, weekly | CodeQL analysis for `csharp` and `javascript-typescript` | No |
| `security-secret-scan.yml` | push/PR to `main`, manual | gitleaks secret scan (allowlist in `.gitleaks.toml`) | No |
| `deploy_function.yml` | **manual only** (`workflow_dispatch`) | Publish `source/judotech.api` and deploy to the `judotech` Azure Function App | Yes — `production` environment |
| `deploy_web.yml` | **manual only** (`workflow_dispatch`, pick a `site`) | Build a static site and upload to the `storagejudotech` `$web` container | Yes — `production` environment |

## Secrets

| Secret | Used by | Purpose |
|--------|---------|---------|
| `judotech_FFFF` | `deploy_function.yml` | Azure Function App publish profile |
| `AZURE_CREDENTIALS` | `deploy_web.yml` | Azure service-principal login for blob upload |
| `GITHUB_TOKEN` | `security-secret-scan.yml` | Provided automatically by Actions |

The old `judotech` publish-profile secret (used by the removed `ci_api.yml`) is
no longer referenced and can be deleted.

## The `production` environment

Both deploy workflows are bound to a GitHub **environment** named `production`.
Configure it in repository settings → Environments:

- add at least one **required reviewer** so a deploy needs manual approval;
- optionally restrict deployments to the `main` branch.

Until the environment exists the deploy workflows still run on dispatch but
without the approval gate.

## Branch protection for `main`

Recommended settings (repository settings → Branches → add rule for `main`):

- Require a pull request before merging.
- Require status checks to pass:
  - `CI - .NET / build-and-test`
  - `CI - Portal / verify`
  - `CI - Legacy static sites / build` (the non-`allow-failure` legs)
  - `CodeQL`
  - `Security - Secret scan / gitleaks`
- Require branches to be up to date before merging.
- Do not allow direct pushes (no bypass).

## Notes

- `ci_api.yml` and `ci_web.yml` (which built **and** deployed on every push,
  targeting .NET 3.1) were removed in MVP-001 Phase 7.
- `codeql.yml` was moved from the repo root into `.github/workflows/` (it was
  never an active workflow before) and fixed (removed the .NET 3.1 pin and the
  undefined `matrix.language`).
- Real green runs can only be confirmed once the branch is pushed and a PR is
  opened.
