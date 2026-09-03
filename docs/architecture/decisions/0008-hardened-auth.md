# ADR-0008: Hardened authentication design for `judotech.api`

- Status: proposed
- Date: 2026-09-03
- Deciders: project owner
- Supersedes: [ADR-0007](0007-source-auth-review.md)
- Related: MVP-002 (`docs/mvp/002-api-security-hardening.md`), ADR-0001,
  `docs/architecture/technical-debt.md` (TD-032…TD-039, TD-002, TD-045)

## Context

ADR-0007 reviewed the existing `judotech.api` authentication and recorded eight
findings (static password salt, string-concatenated Cosmos queries, plaintext
passwords in logs, non-expiring tokens, missing per-endpoint authorization,
non-constant-time comparison, password hashes in responses, `CORS: *`). It
concluded the scheme is acceptable only for the current low-volume internal use
and **must be hardened before the portal exposes authentication or personal
data**. MVP-002 does that. This ADR is the design it implements.

The API is an isolated-worker Azure Functions app over Cosmos DB. `DbUser`,
`DbLogin` and `DbCompetition` are Active-Record models; the data layer is
reached through `DatabaseBase.GetDefaultDatabase()`. MVP-002 introduces the
minimum seam needed to make auth testable (an `IJudoDatabase` abstraction and an
`IAuthService`); the broader repository/DI refactor is MVP-003.

## Decision

### Password hashing

- **PBKDF2-HMAC-SHA256**, a fresh **128-bit random salt per password**, **600 000
  iterations** (OWASP 2023), 256-bit output.
- Stored as a self-describing string: `pbkdf2$sha256$<iterations>$<salt-b64>$<hash-b64>`.
- Verification re-derives with the stored parameters and compares with
  `CryptographicOperations.FixedTimeEquals`.
- **Migrate on login**: a stored value with no `$` delimiters is a legacy hash
  (static salt `"AzureWebsite"`, 100 000 iterations). `Verify` still accepts it;
  on a successful login with a legacy or under-strength hash the password is
  re-hashed to the current format and persisted. No bulk migration, no forced
  reset.
- The client sends the **plaintext password over TLS**; the server hashes. The
  `HashPassword` HTTP endpoint (a hashing oracle) is removed.
- Argon2id is a better KDF but needs a new dependency; PBKDF2 with these
  parameters is acceptable and uses the package already referenced. Revisit if a
  dependency becomes justified.

### Session tokens

- On login the server issues a **256-bit token** from `RandomNumberGenerator`,
  base64url-encoded, returned to the caller **once**.
- Cosmos stores only **`SHA-256(token)`** (`tokenHash`), never the token itself.
- The login document carries `CreatedUtc` and `ExpiresUtc`. Expiry is
  **absolute**, default **12 hours**, from `AuthOptions.TokenLifetimeMinutes`.
  Sliding expiry is out of scope.
- `Authenticate(token)` hashes the presented token, looks the hash up, and
  rejects if `ExpiresUtc <= now`.
- Revoke = delete the login document (`Logout`). The Logins container also has a
  `DefaultTimeToLive` equal to the token lifetime so expired documents
  self-purge.
- Tokens are opaque, not JWT: server-side revoke stays simple and there is no
  signing key to manage.

### Authorization

- The caller presents the token in the **`Authorization: Bearer <token>`**
  header — never the query string or body.
- `Login` and `Logout` are `AuthorizationLevel.Anonymous` (the portal calls them
  without a function key). Every other endpoint keeps the function key **and**
  requires a valid bearer token.
- Helpers: `RequireUser` → 401 if unauthenticated; `RequireRole(role)` → 403;
  `RequireSelfOrRole(targetEmail, role)` → a caller may act on their own record,
  otherwise needs an elevating role.
- Endpoint policy:
  - `CreateUser` / `CreateUsers` / `ReadAllUser` → role `admin`.
  - `ReadUser` / `UpdateUser` / `DeleteUser` → self, or role `admin`.
  - `ReadCompetition` / `ReadAllCompetitions` → any authenticated user.
  - `CreateCompetition(s)` / `UpdateCompetition` → role `manager`.
- The ad-hoc `IsAuthoraized` / `AuthoraizedRequest` (token in the request body)
  is removed.

### Data exposure

- Cosmos queries use parameterised `QueryDefinition` (`.WithParameter`); no
  request value is concatenated into a query string anywhere in `judotech.core`.
- Responses use a `UserResponse` DTO with no password material. `DbUser`'s hash
  field is read from Cosmos but never serialised outward
  (`ShouldSerialize…() => false`).

### CORS

- Restricted to the portal origin(s) per environment, from a comma-separated
  `AllowedOrigins` setting (local default `http://localhost:5173`). Applied as
  ASP.NET CORS middleware in `Program.cs` **and** configurable on the Azure
  Function App (defence in depth). `Host.CORS: "*"` is removed from the sample.

### User model

- `DbUser` settles on **one shape**: identity + auth (`Id` = `Email`, `Email`,
  `PasswordHash`, `Roles`, `Active`) plus the reworked athlete-profile fields
  (`First`, `Lastname`, `Started`, `BirthDate`, `Age`, `Grade`,
  `ShouldHaveGrade`, `Total`). The address-book fields (`FullName`,
  `Personnumber`, `Adress`, `PostalCode`, `City`, `PrimaryPhone`,
  `SecondaryPhone`, `License`, `Club`, `Zone`, `Attendance`, `Borde`, `Diff`)
  are dropped — that data belongs to the SportAdmin member register, a separate
  concern, not to app users.
- `Email` stays the id and Cosmos partition key (`/email`).
- **Migration**: existing Cosmos documents keep their extra fields; reads ignore
  unknown JSON; no data is lost. An optional cleanup script is future work.

### Testability

- `IJudoDatabase` is extracted from `DatabaseBase`. `AuthService : IAuthService`
  depends on `IJudoDatabase` + `IPasswordHasher` + `TimeProvider` +
  `AuthOptions`, so the auth flow is unit-tested with an in-memory database and a
  `FakeTimeProvider`.
- The API classes become instance types with constructor injection, registered
  in `Program.cs`.
- `judotech.api.tests/AuthenticationIntegrationTests` is un-skipped and runs
  against the Cosmos emulator, in a dedicated non-required CI job
  (`ci-dotnet-integration`, scheduled + manual) and locally via `docker run`.

## Consequences

- MVP-002 Phases 2–6 implement the above; each phase keeps `dotnet build` +
  `dotnet test --filter Category!=Integration` green.
- Endpoints that previously worked with just the function key now return 401
  without a bearer token. No live consumer depends on them today (the portal has
  no auth; the frozen `judotech.web*` sites are not deployed) — see MVP-002
  Risk 8.
- 600 000 PBKDF2 iterations add ~0.2–0.4 s per login on a Functions cold start —
  acceptable for a login endpoint, and configurable.
- The `Users` / `Competitions` in-memory singletons and the remaining Active
  Record usage are **not** removed here (MVP-003); some `.Result` / nullable
  warnings (TD-050) remain.
- ADR-0007's "review outcome" framing is superseded; its findings become
  "fixed in MVP-002" in the debt register.

## Alternatives considered

- **JWT bearer tokens.** Standard, stateless, but loses cheap server-side
  revoke, needs signing-key management, and gains nothing here. Rejected.
- **Argon2id hashing.** Stronger, but a new native dependency for a
  maintenance-mode API. Deferred; PBKDF2 at OWASP parameters is sufficient.
- **Full repository pattern + DI everywhere now.** The right end state, but
  feature-scale; MVP-002 takes the minimum seam and MVP-003 finishes it.
- **Force-reset all existing passwords.** Cleaner than migrate-on-login but
  needs an email reset flow (out of scope). Migrate-on-login with a legacy
  fallback is transparent and reversible.
- **Keep CORS on the Azure Function App only.** Works, but code-level CORS keeps
  local dev and the config in one place; both are used.
