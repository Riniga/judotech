# MVP-002: `judotech.api` Security Hardening

Status: proposed
Date: 2026-09-02
Related: `docs/architecture/decisions/0007-source-auth-review.md`,
`docs/architecture/technical-debt.md` (TD-032–TD-039, TD-002, TD-045),
`docs/architecture/decisions/0001-source-vs-portal-scope.md`

This is the planned next increment after MVP-001. It makes `source/judotech.api`
safe enough for `judotech-portal` to build authenticated end-user features on
top of. Per ADR-0001 this is the "future ADR / hardening" that must happen
before the portal exposes login or personal data.

## Goal

`judotech.api` handles authentication and user data without the weaknesses
recorded in ADR-0007: no injectable query strings, no shared static salt, no
secrets in logs, expiring sessions, real authorization on every endpoint, and
no password material leaving the API. The auth flow is covered by automated
tests (including the integration test that MVP-001 left skipped).

## Scope

### In scope

Each item maps to a technical-debt row.

1. **Password hashing (TD-032).** Per-user salt; a modern KDF with sensible
   parameters; existing hashes migrated or invalidated on next login.
2. **Query construction (TD-033).** All Cosmos queries use parameterised
   `QueryDefinition`; no string concatenation of request values anywhere in
   `judotech.core`.
3. **Logging (TD-034).** No password, token, hash, or full request body is ever
   logged. A check (test or analyzer) guards this.
4. **Session tokens (TD-035).** Tokens expire; expiry is enforced on validation;
   there is a way to revoke. Decide sliding vs absolute expiry.
5. **Authorization (TD-036).** Every state-changing and data-returning endpoint
   checks the caller's identity and role. A caller can only read/modify their
   own data unless they hold an elevating role.
6. **Login robustness (TD-037).** Constant-time hash comparison; the
   unknown-email path returns a clean failure, never a null-dereference.
7. **Data exposure (TD-038).** No endpoint returns password hashes or other
   secret fields. `ReadAllUser` (or its replacement) returns a safe projection.
8. **CORS (TD-039).** Locked to the known portal origin(s) per environment, not
   `*`.
9. **User model (TD-045).** `DbUser` settles on one coherent shape (not the
   current superset), with the Cosmos partition-key implications worked out and
   a migration path for existing documents.
10. **Testability (TD-002, minimum).** The database is injected rather than
    reached through `DatabaseBase.GetDefaultDatabase()` / env vars in the data
    layer, enough that the auth flow can be unit-tested without Cosmos. The
    `judotech.api.tests` integration test is un-skipped and runs against the
    Cosmos emulator in a dedicated CI job (or documented as a manual gate).
11. **An ADR** recording the hardened auth design (hashing, tokens, authz
    model) — supersedes the "review outcome" framing of ADR-0007.

### Out of scope

- The in-memory singleton caches (`Users` / `Competitions`) beyond what item 10
  requires — that is MVP-003.
- Any portal auth UI or flow — that is a later portal MVP, unblocked by this one.
- Rewriting the API onto a different hosting model or framework.
- The Gulp/Pug sites, `DbCompetition`, and unrelated `judotech.core` cleanup.
- New product features.

## Expected value

- **Unblocks the portal.** The portal team can build login and personal pages
  against an API that is safe to expose.
- **Removes the high-severity debt** (TD-032, TD-033, TD-034, TD-038) that would
  otherwise be a breach waiting to happen once real users exist.
- **Makes the API testable**, so future changes to it are verifiable.
- **Settles the `DbUser` model**, removing the ambiguity MVP-001 parked.

## Acceptance criteria

1. No Cosmos query in `judotech.core` is built by string concatenation of
   caller-supplied values; a test demonstrates an injection attempt fails
   safely.
2. Password hashing uses a per-user salt and a modern KDF; a test covers hash
   uniqueness for equal passwords across users.
3. A test (or analyzer rule) fails if a password, token, or hash is written to a
   log.
4. Session tokens carry an expiry that is enforced; an expired token is rejected;
   there is a revoke path with a test.
5. Every user/competition endpoint rejects an unauthenticated or unauthorized
   caller; tests cover "own data" vs "other's data" vs "elevated role".
6. Login uses constant-time comparison and handles the unknown-email case
   without throwing.
7. No API response body contains a password hash; a test asserts this for the
   user-listing and user-read endpoints.
8. CORS is configured per environment to the portal origin(s), not `*`.
9. `DbUser` has a single documented shape; a short migration note explains what
   happens to existing Cosmos documents.
10. The database is injectable; the auth flow has unit-test coverage that does
    not touch Cosmos; the previously-skipped integration test runs in CI (or is
    documented as a gated manual step).
11. An ADR records the hardened design and marks ADR-0007 superseded.
12. `dotnet test source/judotech.sln` is green; CI on the PR is green.

## Notes

- Implementation plan: [`../plans/002-api-security-hardening.plan.md`](../plans/002-api-security-hardening.plan.md).
- Do this on a fresh `feature/002-api-security-hardening` branch (MVP-001 is
  merged).
- MVP-003 (API structure — DI everywhere, remove the in-memory caches,
  repository boundary) is the natural follow-up and may merge into this if the
  work overlaps heavily.
