# ADR-0007: `source/` authentication scheme — review outcome

- Status: accepted
- Date: 2026-09-02
- Deciders: project owner
- Related: ADR-0001, `docs/architecture/overview.md` sections 3.1, 11, 13,
  `docs/architecture/technical-debt.md`

## Context

The portal (ADR-0001) will consume `source/judotech.api` for authentication.
Before building on it, the existing scheme was reviewed against the code
(`source/judotech.core/DbLogin.cs`, `CosmosDatabase.cs`,
`source/judotech.api/AuthenticatorApi.cs`, `UserApi.cs`, `CompetitionApi.cs`).

Findings:

| # | Finding | Location |
|---|---------|----------|
| A | Password hashing uses a **single hard-coded salt** (`"AzureWebsite"`) for every user (PBKDF2-HMACSHA256, 100k iterations otherwise reasonable). | `DbLogin.HashPassword` |
| B | Token lookup builds a Cosmos SQL string by **concatenating the raw token** into the query. Injection risk if a non-GUID value reaches it. | `CosmosDatabase.GetUserFromToken` |
| C | The **plaintext password is written to the log** during login. | `AuthenticatorApi.Login` |
| D | Session **tokens never expire** and are not rotated; a token is valid until an explicit logout. | `DbLogin`, `CosmosDatabase.LoginUser` |
| E | All endpoints use a single shared `AuthorizationLevel.Function` key. Most user/competition endpoints perform **no per-user or role check** (only `CompetitionApi.UpdateCompetition` checks a role). | `UserApi`, `CompetitionApi` |
| F | Hash comparison on login is a plain `==` string compare (not constant-time); `ReadUser` for an unknown email can return a null-ish user that is then dereferenced. | `DbLogin.LoginUser` |
| G | `ReadAllUser` returns every user object **including the password hash**. | `UserApi.ReadAllUser` |
| H | CORS is configured as `*` in the sample settings. | `local.settings_sample.json` |

## Decision

- The current scheme is **accepted as-is for the present low-volume, internal
  use** of `source/judotech.api`. It is not rewritten in MVP-001.
- The scheme **must be hardened before the portal exposes authentication or
  personal data to end users.** A dedicated ADR and plan will cover that work;
  it is a prerequisite for the portal's first authenticated release, tracked
  against ADR-0001's "future API decision".
- Each finding above is recorded as a backlog item in
  `docs/architecture/technical-debt.md` with a severity. Findings A, B, C and G
  are marked high severity.
- No finding blocks MVP-001, because MVP-001 adds no authenticated end-user
  functionality.

## Consequences

- MVP-001 proceeds without touching auth code, except that Phase 6 moves the
  HTTP self-test out of the API (no behaviour change to auth).
- The portal team must treat `judotech.api` auth as provisional and must not
  ship an end-user login against it until the hardening ADR is done.
- The backlog items give the hardening work a concrete starting checklist.

## Alternatives considered

- **Fix the auth issues in MVP-001.** Rejected: security-sensitive changes to a
  deployed API are their own scoped effort with their own testing needs, not
  foundation work; ADR-0001 keeps `source/` maintenance-only.
- **Block MVP-001 until auth is fixed.** Rejected: MVP-001 delivers value
  (structure, tests, CI) that is independent of auth and does not increase the
  exposure of the existing scheme.
- **Declare the scheme acceptable long-term.** Rejected: findings A, B, C and G
  are not acceptable for public end-user data.
