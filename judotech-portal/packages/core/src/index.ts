/**
 * Public surface of `@judotech/core`.
 *
 * This package is deliberately thin for now (MVP-001). Domain models, hooks and
 * feature API clients are added here as the portal grows.
 */

export { HttpClient, HttpError, createHttpClient } from "./api/http-client";
export type {
  HttpClientOptions,
  RequestOptions,
} from "./api/http-client";

/**
 * A JudoTech user. Placeholder shape — the authoritative model lives in the
 * API (`source/judotech.api`) and is not stable yet (see ADR-0007, TD-040).
 */
export interface User {
  id: string;
  firstName: string;
  lastName: string;
  roles: string[];
}
