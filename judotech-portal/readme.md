# JudoTech Portal

The front-end monorepo for JudoTech and the active development target for new
work (see `docs/architecture/decisions/0001-source-vs-portal-scope.md`).

## Structure

```text
judotech-portal/
├─ apps/
│  └─ athlete/          # @judotech/athlete — Vite + React 19 app
├─ packages/
│  ├─ ui/               # @judotech/ui     — shared React components + Tailwind theme
│  ├─ core/             # @judotech/core   — domain types, hooks, API clients
│  └─ config/           # @judotech/config — shared ESLint + Vitest config
├─ tsconfig.base.json   # shared TypeScript compiler options (packages extend this)
├─ tsconfig.json        # solution file (project references, for editors)
├─ turbo.json           # task pipelines
└─ package.json         # npm workspaces + root scripts
```

More apps (`public`, `trainer`, `referee`) are added on demand, not scaffolded
ahead of need (ADR-0004).

## Requirements

Node 24 (see `.nvmrc`) and npm 11. With `nvm`: `nvm install && nvm use`.

## Commands

Run from `judotech-portal/`:

| Command | What it does |
|---------|--------------|
| `npm install` | Install all workspaces |
| `npm run dev` | `turbo dev` — run every app's dev server |
| `npm run dev:athlete` | Just the athlete app (Vite, port 5173) |
| `npm run build` | `turbo build` — build every app |
| `npm run lint` | `turbo lint` — ESLint across the workspace |
| `npm run typecheck` | `turbo typecheck` — `tsc` across the workspace |
| `npm test` | `turbo test` — Vitest across the workspace |

## Conventions

- Coding standard: `docs/standards/coding-typescript-react.md`.
- Packages are consumed from source (no build/publish step) via the
  `@judotech/*` scope and workspace links.
- Shared ESLint config: `@judotech/config/eslint`. Shared Tailwind theme:
  `@judotech/ui/styles/index.css`.

## Known gaps

`@judotech/ui` still contains a large set of unadopted admin-template components
and only exports `Button`, `AppLayout` and `ThemeProvider` (TD-020 / TD-021 in
`docs/architecture/technical-debt.md`). The athlete app is a scaffold — no real
screens yet.
