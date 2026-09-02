# ADR-0004: Front-end workspace — npm workspaces + Turborepo, `@judotech/*` packages

- Status: proposed
- Date: 2026-09-02
- Deciders: project owner
- Related: ADR-0001, MVP-001 Phase 5, `judotech-portal/readme.md`

## Context

`judotech-portal/` is the active target (ADR-0001). Today it has:

- a root `package.json` with npm `workspaces` (`apps/*`, `packages/*`) and a
  `build` script that calls `turbo build`, though `turbo` is not installed and
  there is no `turbo.json`;
- one app, `apps/athlete` (Vite + React 19);
- one shared package, `packages/ui` (`@judotech/ui`), consumed by the app
  directly as TypeScript source via a Vite alias and a `tsconfig` path;
- an empty `packages/core` directory;
- ESLint and TypeScript configuration copied inline into the app.

`judotech-portal/readme.md` sketches a wider structure
(`apps/{public,trainer,athlete,referee}`, `packages/{ui,core,config}`, a root
`tsconfig.base.json`) that does not exist yet.

## Decision

- **Package manager: npm** with workspaces. A `package-lock.json` already
  exists; no switch to pnpm or yarn.
- **Task runner: Turborepo.** Add `turbo` as a root dev dependency and a
  `turbo.json` with `build`, `lint`, `typecheck` and `test` pipelines. The
  existing root `build` script already assumes it, and it scales as apps are
  added.
- **Packages:**
  - `@judotech/ui` — shared React components and theme.
  - `@judotech/core` — domain types, hooks, and API clients (created minimally
    in MVP-001).
  - `@judotech/config` — shared ESLint, TypeScript and test configuration.
- **Apps** (`apps/public`, `apps/trainer`, `apps/referee`) are created on demand,
  not scaffolded ahead of need. `apps/athlete` stays the only app in MVP-001.
- **Packages are consumed from source** within the monorepo (no separate build
  or publish step). A shared root `tsconfig.base.json` plus per-package
  `tsconfig.json` files with project references give type-checking across the
  workspace.
- **Scope:** `@judotech/*` for all workspace packages.

## Consequences

- MVP-001 Phase 5 adds `turbo.json`, `tsconfig.base.json`, `packages/config`,
  a minimal `packages/core`, and per-package config/test files, and wires root
  scripts (`dev`, `build`, `lint`, `typecheck`, `test`) through Turborepo.
- One more dependency (`turbo`) and one more config file to maintain.
- Consuming packages from source keeps the loop simple but means every consumer
  compiles the shared code; acceptable at this size.
- The large unexported template code in `packages/ui` is not addressed here —
  see `docs/architecture/technical-debt.md`.
- If the team later needs independently versioned/published packages, a build
  and release step can be added without changing the layout.

## Alternatives considered

- **No task runner** (`npm run <script> --workspaces --if-present`). Simpler, no
  dependency, but no caching, no dependency-aware ordering, and the root script
  already references `turbo`. Rejected.
- **Nx.** More capable, heavier, more opinionated than needed for a few React
  apps. Rejected.
- **Build and publish packages to a registry.** Unnecessary for a single-repo
  team; adds release overhead. Rejected for now.
- **pnpm.** Good fit for monorepos, but npm already works and the lockfile
  exists; switching has a cost with no pressing benefit. Rejected.
