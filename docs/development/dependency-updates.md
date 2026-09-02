# Handling Dependabot Updates

Dependabot opens pull requests to keep dependencies current and to patch security
advisories. This document is how we take them in and keep the PR list tidy.

Related: [`../dependencies/README.md`](../dependencies/README.md) (the policy),
[`ci-cd.md`](ci-cd.md) (the checks that gate merges).

## Configuration

`.github/dependabot.yml` covers four ecosystems, **grouped** so each produces one
PR per run instead of one per dependency:

| Ecosystem | Directory | Group label |
|-----------|-----------|-------------|
| npm — portal | `/judotech-portal` (workspaces, single lockfile) | `portal` |
| npm — legacy sites | `/source/judotech.web*` (all four) | `legacy-web` |
| NuGet | `/source` (the `.sln`: core, api, test projects) | `dotnet` |
| GitHub Actions | `/` | `actions` |

Weekly, Mondays. Each ecosystem has one group for version updates and one for
security updates. `source/judotech.VideoStream*` is not covered — it is dormant
(ADR-0001) and not in the solution.

## The safety net

Every Dependabot PR runs the same CI as any other PR: `ci-dotnet`, `ci-portal`,
`ci-web-legacy`, `codeql`, `security-secret-scan`. With branch protection on
`main` (see [`ci-cd.md`](ci-cd.md)), **a PR that breaks the build or tests
cannot be merged.** That is what makes batch-merging safe.

## Auto-merge

`.github/workflows/dependabot-auto-merge.yml` turns on GitHub auto-merge for
low-risk updates — patch bumps, and minor bumps that are not
`direct:production`. Those merge themselves once CI is green. Everything else
(major bumps, production minor bumps) waits for a person.

If branch protection requires a review, auto-merge still waits for that review;
`GITHUB_TOKEN` cannot approve. For fully hands-off low-risk merges, either drop
the review requirement for Dependabot or add an approval step backed by a
dedicated PAT / GitHub App.

## Taking in a batch

1. **Merge outstanding foundation work first.** Rebasing dozens of Dependabot
   PRs onto a large refactor is wasted effort — land the refactor, let Dependabot
   re-evaluate, then deal with what remains.
2. **Regroup existing PRs** after a `dependabot.yml` change: comment
   `@dependabot recreate` on one PR per ecosystem; superseded individual PRs
   close automatically.
3. **Security PRs first**, then version PRs, ecosystem by ecosystem.
4. For each grouped PR: check CI is green, skim the changelog links in the PR
   body, merge.
5. If a PR has drifted: comment `@dependabot rebase` (keep changes, update base)
   or `@dependabot recreate` (rebuild from scratch). No local work needed.
6. If an update must not happen: comment `@dependabot ignore this dependency`
   (or add an `ignore:` block to `dependabot.yml` for a lasting rule).

## Useful `@dependabot` comments

| Comment | Effect |
|---------|--------|
| `@dependabot rebase` | Rebase the PR on the latest base branch |
| `@dependabot recreate` | Rebuild the PR from scratch (picks up config changes) |
| `@dependabot merge` | Merge once CI passes |
| `@dependabot ignore this minor version` | Skip this version, keep future ones |
| `@dependabot ignore this dependency` | Stop updating this dependency |
| `@dependabot reopen` | Reopen a PR closed by mistake |

## When a grouped update fails CI

Do not merge it. Either:

- pin the offending package back with an `ignore:` entry (with a version
  constraint) and let the rest of the group through, or
- fix the incompatibility on a normal `fix/` or `chore/` branch and close the
  Dependabot PR.

Record anything non-obvious in [`../architecture/technical-debt.md`](../architecture/technical-debt.md).
