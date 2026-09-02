# Coding Standard — TypeScript / React

Applies to everything under `judotech-portal/`. Read [`coding.md`](coding.md)
first for the language-neutral principles.

## Toolchain

- TypeScript `strict` mode, via the shared `tsconfig.base.json`
  (ADR-0004). Per-package `tsconfig.json` extends it.
- ESLint flat config from `@judotech/config`; do not re-declare rules per
  package beyond what is package-specific.
- Prettier owns formatting. Do not fight it with manual formatting or with
  ESLint style rules.
- Node.js version is pinned by `.nvmrc`; npm is the package manager (ADR-0004).
- Run `npm run lint` and `npm run typecheck` before commit (Turborepo fans these
  out).

## Naming and files

- Components: `PascalCase`, one component per file, file named after the
  component (`Button.tsx`).
- Hooks: `useCamelCase`.
- Other modules, variables, functions: `camelCase`; types and interfaces:
  `PascalCase`.
- Test files sit next to the code: `Button.test.tsx`.

## Modules and exports

- Shared library code (`packages/*`) uses **named exports** and re-exports its
  public surface from `src/index.ts`. Do not deep-import across packages.
- App code (`apps/*`) may use a default export for route/page components, named
  exports elsewhere.
- Import workspace packages by their scope name (`@judotech/ui`,
  `@judotech/core`), never by a relative path into another package.

## React

- Function components only; no class components.
- Follow the rules of hooks (enforced by `eslint-plugin-react-hooks`).
- Keep components small; extract logic into hooks or `@judotech/core`.
- Cross-cutting state goes through a context provider (as `ThemeContext` /
  `SidebarContext` already do), not prop drilling.
- Side effects belong in `useEffect` / event handlers, not in render.

## Styling

- Tailwind CSS v4 utility classes. Shared theme tokens come from
  `@judotech/config` (ADR-0004); do not hard-code colours or spacing that the
  theme already defines.
- No separate CSS-in-JS library.

## Domain logic and data

- API calls go through a client in `@judotech/core`, not `fetch` scattered in
  components.
- Types shared between packages live in `@judotech/core`.

## Tests

- Vitest + React Testing Library, `jsdom` for component tests. See
  [`testing.md`](testing.md).
- Test behaviour and rendered output, not implementation details.

## Known deviations

`packages/ui` contains a large body of unexported template components that do not
yet meet this standard (TD-020, TD-021). They are excluded from lint/typecheck
scope until adopted. Do not import from them.
