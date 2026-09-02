# ADR-0005: Test frameworks — xUnit for .NET, Vitest + Testing Library for the portal

- Status: proposed
- Date: 2026-09-02
- Deciders: project owner
- Related: MVP-001 Phase 6, `docs/standards/testing.md`

## Context

There are no automated tests anywhere in the repository. `docs/standards/testing.md`
currently mandates `pytest`, which matches none of the production code (C#/.NET
and TypeScript/React). A foundation MVP needs a chosen framework per active
stack so the standard is real and CI can run it.

## Decision

- **.NET: xUnit.** New test projects `source/judotech.core.tests` and
  `source/judotech.api.tests`, added to `source/judotech.sln`. Test files end in
  `Tests.cs`. Run with `dotnet test`.
- **Portal: Vitest + React Testing Library**, with `jsdom` for component tests.
  Test files are `*.test.ts` / `*.test.tsx` next to the code. Run with
  `npm test` (fanned out by Turborepo).
- **Integration tests that need external services** (Cosmos DB for
  `judotech.api`) are marked — `[Trait("Category", "Integration")]` in .NET,
  `describe.skip` / guarded in the portal — and are **excluded from CI by
  default**. CI runs unit tests only.
- `pytest` remains only for the Python data-import scripts, if tests are ever
  added there; it is removed from the shared testing standard.

## Consequences

- MVP-001 Phase 6 creates the test projects and adds at least one real passing
  test per active component (`judotech.core`, `judotech.api` wiring,
  `apps/athlete`, `packages/ui`, `packages/core`).
- `docs/standards/testing.md` is rewritten around these choices in Phase 3.
- The API's ad-hoc HTTP self-test (`AuthenticatorApi.TestAuthenticationApi`) is
  moved into `judotech.api.tests` as a skippable integration test and removed as
  a `[Function]`.
- Vitest shares Vite's config and transform pipeline, which suits the app; note
  the `rolldown-vite` pin as a compatibility risk (see the plan's Risks and
  `docs/architecture/technical-debt.md`).

## Alternatives considered

- **.NET: NUnit or MSTest.** Comparable; xUnit is the current community default
  for new .NET projects and integrates cleanly with `dotnet test`. No strong
  reason to prefer the others.
- **Portal: Jest.** Works, but needs its own transform setup separate from Vite;
  Vitest reuses the existing Vite config and is faster in watch mode.
- **Playwright / end-to-end now.** Valuable later; too heavy for a foundation
  MVP with one placeholder page.
