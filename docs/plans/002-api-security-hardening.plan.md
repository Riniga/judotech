# Implementation Plan — 002: `judotech.api` Security Hardening

Source MVP: `docs/mvp/002-api-security-hardening.md`
Decision to write: `docs/architecture/decisions/0008-hardened-auth.md` (supersedes ADR-0007)
Target executor: Cursor (stepwise, one phase per commit)
Branch: `feature/002-api-security-hardening` cut from `main`

---

## 1. Goal

`source/judotech.api` handles authentication and user data safely enough for the
portal to build a real login on:

- passwords hashed with a **per-user salt** and a modern KDF;
- session tokens that are **random, stored hashed, and expire**;
- **every** endpoint authenticates the caller and checks role / ownership;
- **no** password hash or secret ever appears in a response body or a log line;
- all Cosmos queries are **parameterised**;
- CORS restricted to the portal origin(s);
- one settled `DbUser` shape;
- the auth flow covered by unit tests (no Cosmos) plus the previously-skipped
  integration test wired to run against the Cosmos emulator.

Out of scope (→ MVP-003): removing the `Users` / `Competitions` in-memory
singletons, a full repository pattern for `DbCompetition`, portal auth UI.

---

## 2. Assumptions

1. **Executor environment**: .NET 8 SDK, `git`, internet for NuGet. **Docker may
   not be available**, so the Cosmos emulator cannot be assumed — unit tests use
   an in-memory fake; the integration test only has to compile and be wired.
2. **`main` is green** (`dotnet build` + `dotnet test --filter Category!=Integration`).
   Current package set: Cosmos 3.62.1, Functions Worker 2.5x, Newtonsoft.Json
   13.0.3 (explicit).
3. **No live API consumer breaks.** The portal has no auth yet; the frozen
   `judotech.web*` sites are not deployed. Endpoints becoming 401 without a
   bearer token is acceptable (flagged in Risks).
4. **Opaque tokens, not JWT.** Server-side revoke by deleting the login document
   is kept; a 256-bit random token is issued, its SHA-256 hash is stored.
5. **PBKDF2-HMAC-SHA256** with a random 128-bit salt and 600 000 iterations
   (OWASP 2023). Argon2id is noted as a future option but needs a new package.
6. **Migrate-on-login**: a legacy (static-salt) hash is re-hashed to the new
   format on the next successful login. No bulk migration, no forced reset.
7. **`DbUser` settles on the reworked athlete-profile shape** (`First`,
   `Lastname`, `Started`, `BirthDate`, `Grade`, `ShouldHaveGrade`, `Total`,
   `Active`, `Age`) + auth fields (`Email` = id / partition key, `PasswordHash`,
   `Roles`). The address-book fields are dropped — they belong to the separate
   SportAdmin member register, not to app users. **Owner confirms** (Risk 1).
8. **Absolute token expiry**, default 12 h, from config. Sliding expiry is out of
   scope.
9. `TimeProvider` (net8) is used for anything time-dependent so expiry is
   testable.
10. ADR-0008 is drafted `proposed` in Phase 1 and flipped to `accepted` in
    Phase 6 after owner review, exactly as ADR-0001…0007 were handled.
11. The assistant does not commit / push / merge (per `docs/standards/git.md`);
    the human reviews each phase diff and commits with the phase's message.

---

## 3. Proposed file changes

Legend: **A** add · **M** modify · **D** delete. The as-built record, including
deviations, lives in the phase checklists in section 4.

### `source/judotech.core`

| Change | Path | Purpose |
|---|---|---|
| A | `Auth/PasswordHasher.cs` | Salted PBKDF2 `Hash` / `Verify` / `NeedsRehash`; legacy-format fallback |
| A | `Auth/SessionToken.cs` | 256-bit CSPRNG token issue + SHA-256 hash for storage |
| A | `Auth/IAuthService.cs` | `LoginAsync`, `AuthenticateAsync(token)`, `LogoutAsync`, role/ownership helpers |
| A | `Auth/AuthService.cs` | Implementation over `IJudoDatabase` + `IPasswordHasher` + `TimeProvider` + `AuthOptions` |
| A | `Auth/AuthOptions.cs` | `TokenLifetime`, `Pbkdf2Iterations` — bound from config |
| A | `Data/IJudoDatabase.cs` | Interface extracted from `DatabaseBase` (user / login / competition ops) |
| M | `DatabaseBase.cs` | `implements IJudoDatabase`; `GetDefaultDatabase()` kept but `[Obsolete]` for non-DI callers |
| M | `CosmosDatabase.cs` | Parameterise every `QueryDefinition`; take an injected `CosmosClient`; token lookup by `tokenHash`; Logins container `DefaultTimeToLive` |
| M | `DbLogin.cs` | Pure model: `Id`, `Email`, `TokenHash`, `CreatedUtc`, `ExpiresUtc`. Static methods removed (→ `AuthService`) |
| M | `DbUser.cs` | Settle model (drop address-book fields); `Password` → `PasswordHash` with `[JsonIgnore]` on output; replace the 12-arg constructor; `[Obsolete]` on `DbUser(string email)` |
| M | `enums.cs` (`Settings`) | Add Logins TTL / container-properties config |
| M | `Users.cs`, `Competitions.cs` | Accept an `IJudoDatabase` (constructor or property) so tests don't hit Cosmos; behaviour otherwise unchanged |
| D | `Logger.cs` | Replaced by injected `ILogger` |

### `source/judotech.api`

| Change | Path | Purpose |
|---|---|---|
| M | `Program.cs` | Register `CosmosClient` (singleton), `IJudoDatabase`, `IAuthService`, `IPasswordHasher`, `AuthOptions`, `TimeProvider`; add ASP.NET CORS middleware from `AllowedOrigins` |
| A | `Auth/BearerToken.cs` | Extract `Authorization: Bearer <token>` from `HttpRequest` |
| A | `Auth/ApiAuth.cs` | `RequireUserAsync` → 401, `RequireRoleAsync` → 403, `RequireSelfOrRole` ownership helper |
| A | `Dtos/UserResponse.cs` | Safe user projection (no hash) |
| A | `Dtos/CreateUserRequest.cs` | Inbound create/registration shape (plaintext password) |
| M | `AuthenticatorApi.cs` | Instance class; inject `IAuthService`; `Login` takes plaintext, logs nothing sensitive; **delete `HashPassword` endpoint**; `Logout` via bearer token |
| M | `UserApi.cs` | Inject services; authenticate + authorize every function; return `UserResponse`; `ReadAllUser` admin-only projection |
| M | `CompetitionApi.cs` | Instance class; authenticate on read, authorize (`manager`) on write |
| M | `local.settings_sample.json` | Add `AllowedOrigins`, `Auth__TokenLifetimeMinutes`; replace `Host.CORS: "*"` with `http://localhost:5173` |
| D | `FunctionsAssemblyResolver.cs` | Dead code (resolution is commented out, never called) |

### Tests

| Change | Path | Purpose |
|---|---|---|
| A | `source/judotech.testsupport/judotech.testsupport.csproj` | Plain classlib shared by both test projects |
| A | `source/judotech.testsupport/InMemoryJudoDatabase.cs` | `IJudoDatabase` fake |
| A | `source/judotech.testsupport/RecordingLogger.cs` | Captures log entries for assertions |
| A | `source/judotech.testsupport/TestData.cs` | `DbUser` / `DbLogin` builders |
| M | `source/judotech.sln` | Add `judotech.testsupport`; both test projects reference it |
| A | `judotech.core.tests/Auth/PasswordHasherTests.cs` | Uniqueness, verify, `NeedsRehash`, legacy fallback |
| A | `judotech.core.tests/Auth/AuthServiceTests.cs` | Login ok / wrong pw / unknown email; token expiry; revoke; migrate-on-login; constant-time; no secret in `RecordingLogger` |
| A | `judotech.core.tests/Data/CosmosQueryTests.cs` | Every `CosmosDatabase` query builds a `QueryDefinition` with parameters (reflection / source assertion); malicious token string handled safely by the fake |
| A | `judotech.api.tests/Auth/AuthorizationTests.cs` | 401 no token · 403 wrong role · self vs other · admin override |
| A | `judotech.api.tests/Dtos/UserResponseTests.cs` | No `PasswordHash` member; round-trips |
| M | `judotech.api.tests/FunctionRegistrationTests.cs` | `HashPassword` gone; new endpoints present |
| M | `judotech.api.tests/AuthenticationIntegrationTests.cs` | Un-skip; adapt to `IAuthService`; keep `[Trait("Category","Integration")]` |

### Workflows & docs

| Change | Path | Purpose |
|---|---|---|
| A | `docs/development/smoke-test.md` | Startup runbook + TC-01…TC-14 smoke cases (Phase 0) |
| M | `docs/development/README.md` | Link the smoke-test doc |
| A | `.github/workflows/ci-dotnet-integration.yml` | Cosmos emulator service, `--filter Category=Integration`, `schedule` + `workflow_dispatch`, non-required |
| A | `docs/architecture/decisions/0008-hardened-auth.md` | The hardened design |
| M | `docs/architecture/decisions/0007-source-auth-review.md` | `Status: superseded by ADR-0008` |
| M | `docs/architecture/technical-debt.md` | TD-032…039, TD-045 → fixed; TD-050 note |
| M | `docs/architecture/overview.md` | §3.1 / §3.2 auth description; §11 update |
| M | `docs/development/setup.md` | Cosmos emulator (Docker) instructions + running integration tests |
| M | `docs/mvp/002-api-security-hardening.md` | Status → in progress → done; acceptance table |
| M | `docs/dependencies/README.md` | Any new package (`Microsoft.Extensions.Configuration.Binder` etc.) |

---

## 4. Step-by-step TODO grouped into phases

> Executor rules: one phase at a time; run the phase **Verify** before handing
> back; do not commit. If a step is blocked by something not caused by your
> change, stop and record it under Risks, then continue only if the owner
> agrees. ADR-0008 stays `proposed` until Phase 6.

---

### Phase 0 — Baseline runbook & smoke test — DONE (branch commit `b3fa78e`)

Commit message: `Add startup runbook and smoke-test cases`

Establish "how to run and verify the whole system" **before** touching auth, so
there is a known-good baseline and a repeatable check to run after every later
phase.

- [x] 0.1 `docs/development/smoke-test.md` written — "what runs where" table,
      "start everything" block, TC-01…TC-14, prerequisites section for the
      Cosmos cases.
- [x] 0.2 Linked from `docs/development/README.md` ("See also") and the top of
      `docs/development/setup.md`.
- [x] 0.3 Full run on 2026-09-03 (Windows 11), **all 14 pass**. Two issues found
      and fixed during the run (both invisible to `dotnet build` / `dotnet test`
      / portal CI):
      - **TD-052** — `func start` crashed: `TypeLoadException: ITelemetryInitializer`.
        `Microsoft.ApplicationInsights.WorkerService` 3.1.2 (Dependabot) is
        incompatible with the 2.x Functions worker AI package. Pinned to 2.23.0
        + Dependabot `ignore` for AI major bumps. **TD-053** filed (CI never
        starts the host).
      - **TD-055** — `apps/athlete` rendered unstyled: Tailwind v4 does not scan
        imported workspace packages. Added `@source "../"` to
        `packages/ui/src/styles/index.css`.
      Also: `local.settings_sample.json` kept as a placeholder (no key committed);
      `.gitleaks.toml` no longer allowlists it.
- [x] **Verify**: all 14 TCs executed; TC-01/02/10/11 green; TC-05–09 green
      against the Cosmos emulator; baseline recorded in `smoke-test.md`.

---

### Phase 1 — Auth foundations

Commit message: `Add hardened-auth ADR, DI seam, and test support`

- [ ] 1.1 Write `docs/architecture/decisions/0008-hardened-auth.md`
      (`Status: proposed`): password hashing (PBKDF2, random salt, 600k iters,
      versioned format, migrate-on-login), tokens (256-bit random, SHA-256
      hashed at rest, 12 h absolute expiry, revoke by delete + container TTL),
      authorization (bearer header, role + ownership, `Login`/`Logout`
      anonymous, rest keep the function key), DTO boundary, CORS. Mark it
      "supersedes ADR-0007" in the header.
- [ ] 1.2 Add `source/judotech.core/Data/IJudoDatabase.cs` — move the abstract
      method signatures from `DatabaseBase` onto an interface. `DatabaseBase`
      now `: IJudoDatabase`.
- [ ] 1.3 Add `source/judotech.core/Auth/IAuthService.cs` and `AuthOptions.cs`
      (interface + options only, no implementation yet).
- [ ] 1.4 Create `source/judotech.testsupport/` (classlib, `net8.0`,
      `<IsPackable>false</IsPackable>`, no `IsTestProject`). Reference
      `judotech.core`. Add `InMemoryJudoDatabase : IJudoDatabase`,
      `RecordingLogger : ILogger`, `TestData`.
- [ ] 1.5 `dotnet sln source/judotech.sln add source/judotech.testsupport`.
      Add a `ProjectReference` to `judotech.testsupport` from
      `judotech.core.tests` and `judotech.api.tests`.
- [ ] 1.6 Add `Microsoft.Extensions.Configuration.Binder` to `judotech.core` if
      `AuthOptions` binding needs it (check — the Functions host may already
      bring it transitively).
- [ ] **Verify**: `dotnet build source/judotech.sln -c Release`;
      `dotnet test --filter "Category!=Integration"` — all existing tests still
      green; no behaviour change.

---

### Phase 2 — Password hashing

Commit message: `Per-user salted password hashing`

- [ ] 2.1 `Auth/PasswordHasher.cs` + `IPasswordHasher`:
      - `Hash(string password)` → `pbkdf2$sha256$<iterations>$<salt-b64>$<hash-b64>`
        with a fresh 16-byte salt from `RandomNumberGenerator`,
        `AuthOptions.Pbkdf2Iterations` (default 600 000), 32-byte output.
      - `Verify(string password, string stored)` → parses the format;
        `CryptographicOperations.FixedTimeEquals` on the derived bytes.
      - Legacy fallback in `Verify`: if `stored` has no `$` delimiters, compare
        against the old scheme (`salt = ASCII("AzureWebsite")`, 100 000 iters).
      - `NeedsRehash(string stored)` → true for legacy format or a lower
        iteration count than configured.
- [ ] 2.2 `AuthService` (partial — login only for now): `LoginAsync(email,
      password)` loads the user via `IJudoDatabase`, `Verify`s, and if
      `NeedsRehash` re-hashes and persists. Returns a result type
      (`Succeeded` / `user`), never the hash.
- [ ] 2.3 Wire `AuthenticatorApi.Login` and `UserApi.CreateUser` /
      `CreateUsers` to hash **server-side from plaintext** (they currently call
      `DbLogin.HashPassword` on possibly-pre-hashed input). Update
      `CreateUserRequest` DTO.
- [ ] 2.4 **Delete the `HashPassword` `[Function]`** from `AuthenticatorApi.cs`
      (a hashing oracle; clients must send plaintext over TLS).
- [ ] 2.5 Keep `DbLogin.HashPassword` temporarily as `[Obsolete]` forwarding to
      `PasswordHasher` so nothing else breaks mid-refactor; remove in Phase 5.
- [ ] 2.6 Tests: `PasswordHasherTests` (equal passwords → different stored
      values; verify true/false; legacy verify; `NeedsRehash`);
      `AuthServiceTests.Login_MigratesLegacyHashOnSuccess`.
- [ ] **Verify**: `dotnet test --filter "Category!=Integration"` green;
      `FunctionRegistrationTests` updated (no `HashPassword`).

---

### Phase 3 — Tokens & query safety

Commit message: `Expiring hashed session tokens and parameterised queries`

- [ ] 3.1 `Auth/SessionToken.cs`: `Issue()` → 32 bytes CSPRNG, base64url string
      (returned to the caller); `Hash(token)` → base64 SHA-256 (stored).
- [ ] 3.2 Rewrite `DbLogin.cs` as a pure model: `Id` (= email), `Email`,
      `TokenHash`, `CreatedUtc`, `ExpiresUtc`. Remove `LoginUser`,
      `GetUserFromToken`, `Logout`, `HashPassword` static methods.
- [ ] 3.3 `AuthService`: `LoginAsync` now also creates the `DbLogin`
      (`ExpiresUtc = TimeProvider.GetUtcNow() + AuthOptions.TokenLifetime`,
      `TokenHash = SessionToken.Hash(token)`), returns the **plaintext token**
      once. `AuthenticateAsync(token)` → hash → `IJudoDatabase` lookup → reject
      if `ExpiresUtc <= now` → return the `DbUser`. `LogoutAsync(token)` →
      delete the login document.
- [ ] 3.4 `CosmosDatabase`:
      - `GetLoginByTokenHash`: `new QueryDefinition("SELECT * FROM c WHERE
        c.tokenHash = @h").WithParameter("@h", tokenHash)`.
      - **Audit every other `QueryDefinition`** in the file — the `SELECT * FROM
        Users` / `Competitions` ones take no user input but switch them to
        `FROM c` and confirm none interpolate.
      - Set `DefaultTimeToLive` on the Logins container (from `Settings`) so
        expired login docs self-purge.
- [ ] 3.5 `IJudoDatabase`: replace `GetUserFromToken(string)` with
      `GetLoginByTokenHash(string)` + keep `ReadUser`. Update `DatabaseBase` /
      `InMemoryJudoDatabase`.
- [ ] 3.6 Tests: `AuthServiceTests` — valid token resolves; expired token
      rejected (advance `FakeTimeProvider`); revoked token rejected; unknown /
      malformed token returns null, never throws; `CosmosQueryTests` — reflection
      or a source check that no `QueryDefinition` string contains `'` + `+`.
- [ ] **Verify**: `dotnet test --filter "Category!=Integration"` green.

---

### Phase 4 — Authorization on every endpoint

Commit message: `Require authentication and authorization on every endpoint`

- [ ] 4.1 `judotech.api/Auth/BearerToken.cs`: `TryGet(HttpRequest, out string
      token)` from the `Authorization: Bearer` header only (not query, not
      body).
- [ ] 4.2 `judotech.api/Auth/ApiAuth.cs` (uses injected `IAuthService`):
      - `RequireUserAsync(req)` → `DbUser` or `UnauthorizedResult` (401).
      - `RequireRoleAsync(req, role)` → `DbUser` or `ForbidResult`/403.
      - `RequireSelfOrRoleAsync(req, targetEmail, role)` → ownership check.
- [ ] 4.3 Apply:
      - `AuthenticatorApi.Login` → `AuthorizationLevel.Anonymous` (portal calls
        it without a function key); `Logout` → bearer token, self.
      - `UserApi.CreateUser` / `CreateUsers` → `RequireRoleAsync("admin")`.
      - `UserApi.ReadUser` / `UpdateUser` / `DeleteUser` →
        `RequireSelfOrRoleAsync(email, "admin")`.
      - `UserApi.ReadAllUser` → `RequireRoleAsync("admin")`.
      - `CompetitionApi.ReadCompetition` / `ReadAllCompetitions` →
        `RequireUserAsync`.
      - `CompetitionApi.CreateCompetition(s)` / `UpdateCompetition` →
        `RequireRoleAsync("manager")`; drop the ad-hoc `IsAuthoraized` +
        `AuthoraizedRequest` (token now comes from the header).
- [ ] 4.4 Make `AuthenticatorApi`, `UserApi`, `CompetitionApi` **instance
      classes** with constructor injection (`IAuthService`, `IJudoDatabase`,
      `ILogger<T>`). Remove `static` from the function methods.
- [ ] 4.5 Tests: `AuthorizationTests` — every endpoint returns 401 without a
      token, 403 with the wrong role, 200 for self / correct role; `ReadUser`
      for another user's email → 403 without `admin`, 200 with.
- [ ] **Verify**: `dotnet test --filter "Category!=Integration"` green.

---

### Phase 5 — Data boundary & logging

Commit message: `Stop returning password hashes and logging secrets`

- [ ] 5.1 `Dtos/UserResponse.cs` — `Email`, `First`, `Lastname`, `Roles`,
      `Active`, `Grade`, … (no hash). `DbUser.ToResponse()`.
- [ ] 5.2 `UserApi` returns `UserResponse` / `IEnumerable<UserResponse>`
      everywhere. `ReadAllUser` maps the cached list to responses.
- [ ] 5.3 `DbUser.PasswordHash` — `[JsonProperty("passwordHash")]` **and**
      `[JsonIgnore]` on the getter is impossible; instead add
      `public bool ShouldSerializePasswordHash() => false` (Newtonsoft) so it is
      read from Cosmos but never serialised outward. Add a test.
- [ ] 5.4 Delete `Logger.cs`. Replace every `Logger.Instance.Log(...)` with the
      injected `ILogger` using structured templates. **Remove** all log calls
      that include a password, hash, token, or full request body
      (`AuthenticatorApi.Login` ×3, `DbLogin.LoginUser` ×2, `CosmosDatabase`
      token log, `UserApi.CreateUser` request-body log).
- [ ] 5.5 Remove the now-unused `DbLogin.HashPassword` `[Obsolete]` forwarder
      and `AuthoraizedRequest.cs` if nothing references them.
- [ ] 5.6 Tests: `UserResponseTests` (no `PasswordHash` member; not in JSON);
      `AuthServiceTests.Login_NeverLogsSecrets` (drive success + failure with
      `RecordingLogger`, assert no entry contains the password / hash / token).
- [ ] **Verify**: `dotnet test --filter "Category!=Integration"` green;
      `grep -rn "Logger.Instance" source/` returns nothing.

---

### Phase 6 — Model, DI, CORS, integration CI, close-out

Commit message: `Settle DbUser, wire DI + CORS, add integration CI, complete MVP-002`

- [ ] 6.1 Settle `DbUser` (owner-confirmed shape — Risk 1): keep `Id`/`Email`,
      `PasswordHash`, `Roles`, `Active`, `Age`, and the reworked profile fields;
      **remove** `FullName`, `Personnumber`, `Adress`, `PostalCode`, `City`,
      `PrimaryPhone`, `SecondaryPhone`, `Attendance`, `Borde`, `Diff`,
      `License`, `Club`, `Zone`. Replace the 12-arg constructor with
      `DbUser(string email, string passwordHash, params string[] roles)` (or an
      initialiser). Mark `DbUser(string email)` `[Obsolete("load via
      IJudoDatabase")]`. Update `AuthenticationIntegrationTests` and any other
      caller. Add a "Migration" paragraph to ADR-0008 (existing Cosmos docs keep
      their extra fields; reads ignore them; an optional cleanup script is
      future work).
- [ ] 6.2 `Program.cs`: register `CosmosClient` as a singleton (endpoint + key
      from config); `IJudoDatabase` → `CosmosDatabase` (constructor takes the
      client); `IPasswordHasher`, `IAuthService`, `TimeProvider.System`;
      `services.Configure<AuthOptions>(config.GetSection("Auth"))`.
- [ ] 6.3 `CosmosDatabase` constructor takes `CosmosClient` + `Settings`;
      `GetContainer` uses the injected client (no more `new CosmosClient` per
      call). `Users` / `Competitions` singletons take `IJudoDatabase`.
- [ ] 6.4 CORS: `Program.cs` adds `services.AddCors(...)` + `app.UseCors(...)`
      reading a comma-separated `AllowedOrigins`. `local.settings_sample.json`:
      `"AllowedOrigins": "http://localhost:5173"`, `"Auth__TokenLifetimeMinutes":
      "720"`, remove `"Host": { "CORS": "*" }`. Document the Azure Function App
      CORS setting in `docs/development/ci-cd.md` / setup.
- [ ] 6.5 Delete `FunctionsAssemblyResolver.cs` (confirm `Program.cs` and the
      csproj don't reference it).
- [ ] 6.6 Un-skip `AuthenticationIntegrationTests`; adapt it to `IAuthService`
      (build a real `CosmosDatabase` from `EndpointUrl` / `PrimaryKey` /
      `DatabaseId` env vars; `Skip.If` those are absent using a small helper, so
      it is skipped — not failed — when no emulator is present).
- [ ] 6.7 `.github/workflows/ci-dotnet-integration.yml`: `on: [workflow_dispatch,
      schedule (nightly)]`; a `services:` Cosmos emulator container
      (`mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator`), wait-for-ready
      loop, import the emulator TLS cert, `dotnet test --filter
      "Category=Integration"` with the env vars set. `continue-on-error: true`
      / not added to branch protection (emulator is flaky).
- [ ] 6.8 Docs: ADR-0008 → `accepted` (after owner review); ADR-0007 →
      `superseded by ADR-0008`; `technical-debt.md` TD-032…039 + TD-045 →
      *Fixed in MVP-002*, TD-050 note (nullable warnings in the touched files
      cleared where practical); `overview.md` §3.1/§3.2 + §11; `setup.md`
      emulator + integration-test section; `docs/mvp/002-api-security-hardening.md`
      status `done` with an acceptance table (12 rows → where each is met).
- [ ] 6.9 PR description in a new `## 6. Pull request` section of this file
      (title `MVP-002: Harden judotech.api authentication`).
- [ ] **Verify**:
      `dotnet build source/judotech.sln -c Release` → 0 errors;
      `dotnet test --filter "Category!=Integration"` → all green, ≥ 1 real test
      per new area (hashing, tokens, queries, authz, DTO, no-secret-logging);
      `func start` hosts the API and `Login` (anonymous) works against the
      emulator if available;
      `grep -rn "Logger.Instance\|AzureWebsite\|Host.*CORS.*\*\|SELECT \* FROM .*'\" *+" source/` → nothing;
      acceptance table in the MVP doc complete.

---

## 5. Risks / open questions

1. **`DbUser` shape (blocks Phase 6.1).** The plan drops the address-book
   fields. If the SportAdmin member import is expected to populate `DbUser`
   rather than its own store, that decision reverses and Phase 6 grows. Owner
   confirms before Phase 6. Recommendation stands: member register ≠ app users.
2. **No Docker for the executor.** Everything is unit-testable via
   `InMemoryJudoDatabase`; the integration test only compiles + is wired.
   Real end-to-end confidence needs the emulator (`docker run` locally, or the
   nightly `ci-dotnet-integration` job). Acceptance criterion 10 is met by
   "wired + green unit coverage", not by a green emulator run on the PR.
3. **Cosmos emulator on GitHub Actions is slow and flaky** (2–5 min start, TLS
   cert dance). The integration job is `schedule` + `workflow_dispatch`,
   `continue-on-error`, **not** a required check. The documented local
   `docker run` path is the primary way to run it.
4. **Isolated Functions + ASP.NET CORS middleware.** `ConfigureFunctionsWebApplication`
   enables the ASP.NET pipeline so `app.UseCors()` should work, but it is less
   battle-tested than the Azure Function App CORS setting. Keep both
   (defence-in-depth); verify locally; document the portal-side origin.
5. **`AuthorizationLevel` change.** `Login` / `Logout` move to `Anonymous` so the
   portal can call them without a function key; other endpoints keep `Function`
   *and* gain bearer auth. If the owner wants everything `Anonymous` + bearer
   only, that is a small follow-up.
6. **Migrate-on-login vs force-reset.** Plan migrates silently on next login with
   a legacy-format fallback in `Verify`. If the owner prefers invalidating all
   existing hashes, that needs a password-reset flow (email) which is out of
   scope — then the fallback is removed and existing test users are recreated.
7. **PBKDF2 cost.** 600 000 iterations adds ~0.2–0.4 s per login on a Functions
   cold start. Acceptable for a login endpoint; note it, make it configurable
   (`AuthOptions.Pbkdf2Iterations`).
8. **Breaking the frozen `judotech.web*` sites.** They contain `authenticator.js`
   that may call `Login` / `ReadUser`. They are not deployed and
   `ci-web-legacy` only builds (does not run) them, so a runtime break is
   invisible to CI. Flag in the MVP doc; a follow-up updates or retires that JS.
9. **`Users` / `Competitions` singletons still call `.Result`** and cache
   everything in memory. The plan only injects `IJudoDatabase` into them for
   testability; the deeper fix (remove the singletons, async all the way) is
   MVP-003. Some nullable / sync-over-async warnings (TD-050) will remain.
10. **`InternalsVisibleTo` vs public test types.** `judotech.testsupport` is a
    normal classlib with `public` fakes — no `InternalsVisibleTo` needed. If
    `AuthService` internals need testing directly, add
    `[assembly: InternalsVisibleTo("judotech.core.tests")]` to `judotech.core`.
11. **Newtonsoft `ShouldSerialize` for `PasswordHash`.** Works with the
    Functions default Newtonsoft serialisation. If the API is later switched to
    `System.Text.Json`, replace with `[JsonIgnore]` on a separate write model.
    Covered by `UserResponseTests` so a regression is caught.

---

## Notes for the executor

- Prefer many small commits **within** a phase if it helps review, but the phase
  boundary is the reviewable unit and carries the commit message above.
- Every phase must leave `dotnet build` + `dotnet test --filter
  "Category!=Integration"` green. Never commit a red phase.
- If Phase 4/5 surface a large number of pre-existing nullable warnings in the
  files you touch, fix the ones in your diff; do not chase the rest (TD-050).
