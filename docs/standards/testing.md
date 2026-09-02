# Testing Standard

## Purpose

This document defines the testing standards for all projects in the workspace.

## Test Framework

- Framework: `pytest`
- Test directory: `tests/`
- Test files: `test_<module>.py`
- Tests must be deterministic and runnable offline.
- Avoid external dependencies whenever possible.

## Test Principles

- Every bug fix should include a regression test.
- Every new feature should include tests.
- Prefer fast unit tests over slow integration tests.
- Keep tests isolated and repeatable.

## Test Levels

### Unit Tests
Test individual functions and classes in isolation.

### Integration Tests
Verify collaboration between modules, services or data sources.

### End-to-End Tests
Verify complete user or business workflows where appropriate.

## Running Tests

```bash
pytest
pytest -q
pytest tests/test_example.py
```

All tests should pass before creating a Pull Request.

## Test Requirements

| Change | Required Tests |
|---------|----------------|
| New module | Happy path and error scenarios |
| Bug fix | Regression test |
| New feature | Unit tests and integration tests where applicable |
| Refactoring | Existing tests must remain green |

## Test Data

- Keep fixtures small and reusable.
- Do not depend on production data.
- Prefer factories or fixtures over hard-coded data.

## CI

- All automated tests must pass before merging.
- New failing tests must be investigated before merge.

## Known Limitations

Project-specific limitations (database engines, authentication providers, external services, etc.) belong in the project's architecture or testing documentation—not in this shared standard.
