# JudoTech Documentation

All project documentation lives in this `docs/` directory. Source code explains
*how*; these documents explain *why* and *what*.

## Where to start

Read in this order:

1. [`architecture/overview.md`](architecture/overview.md) — what the workspace
   contains today, how it is built, and what is not yet implemented.
2. [`standards/`](standards/) — coding, testing, Git and documentation
   conventions.
3. [`development/`](development/) — how to set up, build, run and ship the
   workspace, and the day-to-day process.
4. [`architecture/decisions/`](architecture/decisions/) — the record of
   significant decisions and why they were made.
5. [`roadmap.md`](roadmap.md) — intended direction.
6. [`mvp/`](mvp/) and [`plans/`](plans/) — the current scoped delivery increment
   and its implementation plan.

## Contents

| Path | Purpose |
|------|---------|
| [`architecture/overview.md`](architecture/overview.md) | System and workspace structure, dependencies, build process, open questions |
| [`architecture/decisions/`](architecture/decisions/) | Architecture Decision Records (ADRs) |
| [`architecture/technical-debt.md`](architecture/) | Register of known issues and their disposition *(added in MVP-001)* |
| [`standards/coding.md`](standards/coding.md) | Shared coding principles |
| [`standards/testing.md`](standards/testing.md) | Testing standard |
| [`standards/git.md`](standards/git.md) | Git workflow |
| [`standards/documentation.md`](standards/documentation.md) | Documentation standard |
| [`development/`](development/) | Setup, development process, CI/CD, MVP workflow |
| [`dependencies/`](dependencies/) | Dependency management policy |
| [`roadmap.md`](roadmap.md) | Planned evolution of the platform |
| [`features.md`](features.md) | Feature notes *(partly Swedish)* |
| [`functional-requirements.md`](functional-requirements.md) | Functional requirements *(Swedish)* |
| [`non-functional-requirements.md`](non-functional-requirements.md) | Non-functional requirements *(Swedish)* |
| [`mvp/`](mvp/) | Scoped delivery increments |
| [`plans/`](plans/) | Step-by-step implementation plans |
| [`claude-prompts/`](claude-prompts/) | Reusable prompt templates for the AI-assisted workflow |
| `azure.drawio` | Azure infrastructure sketch |

## Conventions

- Written in English (see [`standards/documentation.md`](standards/documentation.md)).
- File and folder names use `kebab-case`.
- Update documentation in the same pull request as the change it describes.

Some tables above reference files that are stubs or are added by a later phase of
[`mvp/001-workspace-foundation.md`](mvp/001-workspace-foundation.md).
