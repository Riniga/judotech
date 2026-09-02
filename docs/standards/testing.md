# Testing Standard

How the workspace is tested. Framework choices are recorded in
[`../architecture/decisions/0005-test-frameworks.md`](../architecture/decisions/0005-test-frameworks.md).

## Frameworks

| Stack | Framework | Test file pattern | Location | Run |
|-------|-----------|-------------------|----------|-----|
| C# / .NET (`source/`) | xUnit | `<Type>Tests.cs` | `source/<project>.tests/` | `dotnet test source/judotech.sln` |
| TypeScript / React (`judotech-portal/`) | Vitest + React Testing Library (`jsdom`) | `*.test.ts` / `*.test.tsx` | next to the code under test | `npm test` (from `judotech-portal/`; Turborepo fans out) |
| Python scripts (`source/`) | none required | — | — | — |

## Principles

- Tests are deterministic and run offline.
- Prefer fast unit tests; add integration or end-to-end tests only where they
  earn their cost.
- Every new feature ships with tests. Every bug fix ships with a regression test
  that fails before the fix.
- Refactoring does not change tests except to follow renamed symbols; existing
  tests stay green.
- Keep fixtures small; never depend on production data.

## Test levels

- **Unit** — a function or class in isolation. The default.
- **Integration** — collaboration between modules, services or a data store.
- **End-to-end** — a complete user or business workflow. Not yet in use.

## External services

Tests that need an external service (currently: Azure Cosmos DB for
`judotech.api`) must be marked and excluded from CI:

- .NET: `[Trait("Category", "Integration")]`; CI runs
  `dotnet test --filter Category!=Integration`.
- Portal: guard with an environment check or `describe.skip`; do not let them run
  by default.

Running them locally needs the Cosmos DB emulator or a real endpoint; see
[`../development/setup.md`](../development/setup.md).

## Requirements by change type

| Change | Required tests |
|--------|----------------|
| New module / component | Happy path and the main error paths |
| Bug fix | Regression test |
| New feature | Unit tests, plus integration tests where a boundary is involved |
| Refactor | Existing tests remain green |

## CI

- CI runs the unit test suites for every actively developed component on every
  pull request (see [`../development/ci-cd.md`](../development/ci-cd.md)).
- All tests must pass before merge. A newly failing test is investigated, not
  skipped.
