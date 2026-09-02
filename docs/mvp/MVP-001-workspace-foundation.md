# MVP-001: Workspace Foundation

Status: in progress
Date: 2026-09-02
Related: `docs/architecture/overview.md`, `docs/roadmap.md`, `docs/standards/`,
`docs/plans/MVP-001-workspace-foundation.plan.md`,
`docs/architecture/decisions/` (ADR-0001–0007),
`docs/architecture/technical-debt.md`

This MVP defines the work required to turn the current repository into a solid,
consistent foundation before feature development on new applications begins. It
describes outcomes, not implementation. It does not contain an implementation
plan.

## Goal

Establish a single, well-understood workspace where:

- the scope and direction of the codebase are explicitly decided and documented,
- standards, documentation and tooling match the technology actually in use,
- every actively developed component can be built, run and tested the same way,
- automated checks protect `main`, and
- adding the next application is a repeatable, low-friction step.

The goal is a **trustworthy baseline**, not new product functionality.

## Scope

### In scope

The following foundation areas are in scope. Each is expressed as a target
outcome; how it is achieved is left to a later implementation plan.

1. **Direction and boundaries**
   - A decision record stating the intended relationship between `source/` and
     `judotech-portal/`: what is kept, replaced, wrapped or frozen, and which
     parts are in active development versus maintenance-only.
   - A clear statement of which components new application work will build on.

2. **Workspace structure**
   - A documented, consistent top-level layout for the active workspace,
     including where shared code, configuration and per-application code live.
   - Naming conventions for packages, applications and directories.
   - Placeholder or aspirational directories are either populated with a minimal
     real structure or removed, so the tree reflects reality.

3. **Documentation**
   - All project documentation consolidated under `docs/` with no duplicate or
     contradicting trees, and internal links/paths correct (including
     `docs/architecture/overview.md`).
   - The document types promised by the standards and workflow prompts exist as
     at least a stub with a defined purpose: architecture overview, decision
     records, development process, MVP register, implementation plans.
   - A short index/entry point that tells a new contributor what to read first.
   - A decision on documentation language and on Swedish business content, applied
     consistently.

4. **Architecture**
   - The architecture overview reflects the post-decision state (see area 1),
     including a named high-level structure for the front-end workspace and the
     role of the existing API.
   - Known architectural risks from the overview (in-process caches, string-built
     data queries, no persistence abstraction at the API boundary, combined
     CI/deploy, single environment) are captured as decision records or backlog
     items with an owner and a disposition (accept / fix later / fix now).

5. **Coding standards**
   - Coding standards exist for each language in active use (C# / .NET and
     TypeScript / React) and match the real code.
   - The existing Python-oriented standard is either scoped explicitly to the
     data-import scripts or retired.
   - A single source of truth for lint/format configuration per stack, referenced
     from the standards.

6. **Development process**
   - A written development process covering branching, reviews, when
     documentation and tests are required, and how AI-assisted changes are
     handled, consistent with `docs/standards/git.md`.
   - The MVP → plan → implement → complete workflow referenced by the prompt
     templates is documented and its required directories exist.

7. **Testing**
   - A chosen test approach and framework for each actively developed stack,
     documented in the testing standard.
   - Test execution is wired into the workspace so a single documented command
     runs the tests for a component.
   - At least one meaningful automated test exists per actively developed
     component, proving the toolchain works end to end.
   - The ad-hoc HTTP self-test in the API is replaced or reclassified.

8. **Build and tooling**
   - Every actively developed component has working, consistent scripts for
     install, build, run and test.
   - The `judotech-portal` root build either works or the unused tooling
     reference is removed; shared TypeScript/lint/style configuration is
     available to all workspace packages.
   - .NET SDK and Node.js versions are pinned in a discoverable way and match
     what CI uses.
   - CI is separated from deployment; CI validates build and tests for all
     actively developed components (including `judotech-portal`) on pull
     requests; the .NET version mismatch in CI is resolved.
   - A decision on environments (at minimum: how production deploys are gated).

9. **Dependencies**
   - Unexplained or stale dependencies identified in the overview are removed or
     justified (for example the `clean` package and the obsolete
     `@types/react-router-dom`).
   - A dependency policy is documented: how versions are pinned, how updates are
     reviewed, and when a decision record is required.
   - Lockfiles are present and consistent for every package manager in use.

10. **Developer experience**
    - A single "from clone to running" path is documented and verified on a clean
      machine for each actively developed component.
    - Sample environment/configuration files cover every required setting, with
      no secrets committed.
    - The dev container (or an equivalent documented setup) can build and run the
      active components.
    - Editor configuration (formatting on save, recommended extensions) is shared
      through the repository.

11. **Security baseline**
    - Secret-handling expectations are documented and verified against the
      current repository and CI configuration.
    - The custom authentication scheme in `source/` has a recorded review outcome
      (safe to build on / must change before reuse / out of scope), so front-end
      work is not blocked by an unknown.

### Out of scope

- Any new application feature work, UI screens, or pages.
- Migrating or rewriting the `source/` .NET API or the Gulp/Pug sites.
- Implementing the applications listed in `docs/roadmap.md`.
- Building the `@judotech/ui` component set beyond what is needed to prove the
  build, test and consumption path.
- Infrastructure provisioning changes beyond documenting and gating what already
  exists.
- Performance, availability and encryption targets from
  `docs/non-functional-requirements.md` (tracked separately).
- Choosing a long-term hosting or framework strategy for the new front end beyond
  the structural decision in area 1.

## Expected value

- **Simplifies future development:** a new application starts from a known
  structure, shared configuration and a working build/test/run loop instead of
  ad-hoc setup.
- **Improves maintainability:** standards, tooling and documentation describe the
  code that actually exists, so they can be trusted and enforced.
- **Improves consistency:** one documentation tree, one process, one way to build
  and test, one set of pinned tool versions.
- **Reduces technical debt:** the known issues from the architecture overview are
  either fixed or explicitly accepted with a rationale, rather than left
  undiscovered.
- **Prepares for additional applications:** the workspace can absorb a new app
  with predictable effort, and CI will validate it automatically.
- **Reduces onboarding time and risk:** a contributor (human or AI-assisted) can
  become productive by following documented, verified steps.

## Acceptance criteria

The MVP is complete when all of the following are true.

### Direction and structure

1. A decision record exists that defines the relationship between `source/` and
   `judotech-portal/` and names the components new work will build on.
2. The active workspace has a documented layout and naming convention, and the
   repository tree matches it (no empty aspirational directories presented as
   real).

### Documentation

3. All project documentation lives under `docs/` with a single, non-contradicting
   structure; there is no parallel `documentation/` tree.
4. `docs/architecture/overview.md` is updated to the post-decision state and all
   its internal path references are correct.
5. Stubs with a stated purpose exist for: development process, decision records,
   MVP register, and implementation plans.
6. A documentation entry point tells a new contributor what to read first and in
   what order.
7. Documentation language and handling of Swedish business content are decided
   and applied consistently across `docs/`.

### Standards

8. Coding standards exist for C#/.NET and for TypeScript/React and are consistent
   with the current code.
9. The Python-oriented standard is either explicitly scoped to the data-import
   scripts or removed.
10. The testing standard names a concrete approach and framework for each
    actively developed stack.

### Process

11. A written development process covers branching, review, required tests and
    docs, and AI-assisted change rules, and does not conflict with
    `docs/standards/git.md`.
12. The MVP → plan → implement → complete workflow is documented and its
    referenced directories exist.

### Testing and CI

13. A single documented command runs the tests for each actively developed
    component.
14. At least one meaningful automated test passes for each actively developed
    component.
15. CI runs build and tests for every actively developed component (including
    `judotech-portal`) on pull requests to `main`.
16. CI no longer deploys as a side effect of every push; production deployment is
    a separate, explicitly triggered or gated step.
17. The .NET version used by CI matches the version targeted by the projects, and
    the toolchain versions (.NET, Node.js) are pinned in the repository.

### Build and tooling

18. Every actively developed component has working `install`, `build`, `run` and
    `test` entry points, documented in one place.
19. The `judotech-portal` root build succeeds, or the unused build-tool reference
    is removed; shared TypeScript, lint and style configuration is consumable by
    all workspace packages.

### Dependencies

20. Every dependency flagged as unexplained or stale in the architecture overview
    is removed or has a recorded justification.
21. A dependency management policy is documented, and lockfiles are present and
    consistent for each package manager in use.

### Developer experience and security

22. A "clone to running" path is documented for each actively developed component
    and has been verified on a clean environment.
23. Sample configuration files cover all required settings; a check confirms no
    secrets are committed and CI secret usage is documented.
24. Shared editor configuration (formatting, recommended extensions) is present in
    the repository.
25. The `source/` authentication scheme has a recorded review outcome that states
    whether new work may build on it as-is.

### Overall

26. Every known issue listed in `docs/architecture/overview.md` sections 11 and
    13 has a disposition: fixed in this MVP, converted to a tracked backlog item,
    or accepted in a decision record with a rationale.
