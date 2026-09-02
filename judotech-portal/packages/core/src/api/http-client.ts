/**
 * Minimal fetch wrapper for talking to the JudoTech API.
 *
 * Intentionally small: it normalises the base URL, sends/parses JSON, and turns
 * non-2xx responses into a thrown {@link HttpError}. Feature-specific calls
 * belong in their own modules that use this client, not here.
 */

export class HttpError extends Error {
  readonly status: number;
  readonly statusText: string;
  readonly body: unknown;

  constructor(status: number, statusText: string, body: unknown) {
    super(`HTTP ${status} ${statusText}`);
    this.name = "HttpError";
    this.status = status;
    this.statusText = statusText;
    this.body = body;
  }
}

export interface HttpClientOptions {
  /** Base URL that every request path is resolved against. */
  baseUrl: string;
  /** Extra headers sent on every request. */
  headers?: Record<string, string>;
  /** Injectable fetch, mainly for tests. Defaults to the global `fetch`. */
  fetch?: typeof fetch;
}

export interface RequestOptions {
  method?: string;
  headers?: Record<string, string>;
  body?: unknown;
  signal?: AbortSignal;
}

export class HttpClient {
  private readonly baseUrl: string;
  private readonly baseHeaders: Record<string, string>;
  private readonly doFetch: typeof fetch;

  constructor(options: HttpClientOptions) {
    this.baseUrl = options.baseUrl.replace(/\/$/, "");
    this.baseHeaders = options.headers ?? {};
    this.doFetch = options.fetch ?? globalThis.fetch;
  }

  async request<T>(path: string, options: RequestOptions = {}): Promise<T> {
    const hasBody = options.body !== undefined;
    const response = await this.doFetch(`${this.baseUrl}${path}`, {
      method: options.method ?? (hasBody ? "POST" : "GET"),
      headers: {
        ...(hasBody ? { "content-type": "application/json" } : {}),
        ...this.baseHeaders,
        ...options.headers,
      },
      body: hasBody ? JSON.stringify(options.body) : undefined,
      signal: options.signal,
    });

    const payload = await parseBody(response);
    if (!response.ok) {
      throw new HttpError(response.status, response.statusText, payload);
    }
    return payload as T;
  }

  get<T>(path: string, options?: RequestOptions): Promise<T> {
    return this.request<T>(path, { ...options, method: "GET" });
  }

  post<T>(path: string, body: unknown, options?: RequestOptions): Promise<T> {
    return this.request<T>(path, { ...options, method: "POST", body });
  }
}

async function parseBody(response: Response): Promise<unknown> {
  const text = await response.text();
  if (text === "") return null;
  try {
    return JSON.parse(text) as unknown;
  } catch {
    return text;
  }
}

export function createHttpClient(options: HttpClientOptions): HttpClient {
  return new HttpClient(options);
}
