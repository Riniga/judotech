import { describe, expect, it, vi } from "vitest";
import { HttpClient, HttpError } from "./http-client";

function jsonResponse(body: unknown, init: ResponseInit = {}): Response {
  return new Response(JSON.stringify(body), {
    status: 200,
    headers: { "content-type": "application/json" },
    ...init,
  });
}

describe("HttpClient", () => {
  it("resolves the path against the base URL and parses JSON", async () => {
    const fetchMock = vi.fn().mockResolvedValue(jsonResponse({ id: "u1" }));
    const client = new HttpClient({ baseUrl: "https://api.test/", fetch: fetchMock });

    const result = await client.get<{ id: string }>("/users/u1");

    expect(result).toEqual({ id: "u1" });
    expect(fetchMock).toHaveBeenCalledWith(
      "https://api.test/users/u1",
      expect.objectContaining({ method: "GET" }),
    );
  });

  it("sends a JSON body with a content-type header on post", async () => {
    const fetchMock = vi.fn().mockResolvedValue(jsonResponse({ ok: true }));
    const client = new HttpClient({ baseUrl: "https://api.test", fetch: fetchMock });

    await client.post("/users", { name: "Kano" });

    const [, options] = fetchMock.mock.calls[0];
    expect(options.method).toBe("POST");
    expect(options.body).toBe(JSON.stringify({ name: "Kano" }));
    expect(options.headers["content-type"]).toBe("application/json");
  });

  it("throws HttpError for a non-2xx response", async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValue(jsonResponse({ message: "nope" }, { status: 403, statusText: "Forbidden" }));
    const client = new HttpClient({ baseUrl: "https://api.test", fetch: fetchMock });

    await expect(client.get("/secret")).rejects.toBeInstanceOf(HttpError);
  });
});
