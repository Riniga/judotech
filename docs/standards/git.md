# Git Workflow

This document defines the Git workflow for this workspace.

The goal is to keep changes small, traceable, reviewable, and safe to merge.

## Branches

- Always work on a branch. Never work directly on `main`.
- `main` represents the latest stable version of the workspace.
- Use short, descriptive branch names based on the change being made.

Recommended prefixes:

```text
feature/<short-name>
fix/<short-name>
docs/<short-name>
refactor/<short-name>
chore/<short-name>
```

Examples:

```text
feature/reporting-dashboard
fix/devops-import-error
docs/update-coding-standard
refactor/shared-config-loader
chore/update-dependencies
```

## Commits

- Commit after each completed and verified todo item.
- Do not commit large batches of unrelated changes.
- Do not commit unfinished or unverified code.
- Run relevant tests before committing.
- For code changes, run `pytest` before commit unless the plan explicitly states otherwise.
- Keep commits focused and easy to understand.

## Commit Messages

Write commit messages in English.

Use a short imperative sentence describing what changed.

Good examples:

```text
Add report status model
Fix Azure DevOps query pagination
Update coding standard for Python 3.13
Refactor shared configuration loader
Document local development setup
```

Avoid vague messages such as:

```text
fix stuff
updates
changed files
forgot this
wip
```

## Pull Requests

- Create pull requests against `main`.
- Do not merge locally into `main` unless explicitly agreed.
- The pull request should describe the purpose of the change.
- Reference the relevant plan, MVP, requirement, or issue when applicable.
- CI must pass before merge.
- Documentation should be updated in the same pull request when the change affects behavior, structure, setup, or architecture.

## Push and Merge Rules

- Do not push or merge without explicit approval when working with an AI assistant.
- Do not force-push shared branches unless explicitly agreed.
- Prefer small pull requests over large, mixed changes.
- Keep documentation, code, and tests aligned.

## What Must Never Be Committed

Never commit:

- Personal access tokens
- Passwords
- API keys
- Connection strings containing credentials
- `.env` files
- Local secrets or credential files
- `__pycache__/`
- `*.pyc`
- Generated temporary data
- Local exports that are not intended to be versioned
- Large generated reports unless explicitly approved

See `.gitignore` for the complete ignore list.

## Working With AI Assistants

When Claude Code, GitHub Copilot, or another AI assistant is used:

- The assistant must follow the active plan before implementation.
- The assistant must not commit, push, merge, or delete branches without explicit approval.
- The assistant should show the intended change before applying broad refactoring.
- The assistant should implement one planned todo item at a time.
- The assistant should update the plan as work progresses.

## Recommended Daily Workflow

```bash
git status
git switch main
git pull
git switch -c feature/<short-name>
```

Work in small steps:

```bash
git status
pytest
git add .
git commit -m "Add clear description of change"
git push -u origin feature/<short-name>
```

Then create a pull request against `main`.

## Recovery and Safety

Before risky operations, check the current state:

```bash
git status
git branch
git log --oneline --decorate -10
```

If the branch state is unclear, stop and inspect before continuing.

Do not run destructive commands such as the following unless the impact is fully understood:

```bash
git reset --hard
git clean -fd
git push --force
```
