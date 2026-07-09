# QueryMaker Proxy — Auth

QueryMaker sits in front of the mobile app's legacy `WebService.asmx` backend. It resolves the
real target per-request from the caller's `Referer`/`Origin` header (see
[middleware/proxy.js](middleware/proxy.js)), so the same server works across environments
(localhost, test, prod) without per-environment config.

This document covers the token layer added on top of that proxy: why it exists, how it works,
and how a client is supposed to talk to it.

## Why

Without it, anyone who discovers the proxy URL (or the WebService target it forwards to) could
call it directly — the proxy would happily relay their requests. The token proves a request came
through a successful login before `/generalProxy` will forward it anywhere.

## How it works

```
Client                          QueryMaker                         WebService.asmx
  │                                  │                                    │
  │ POST /LoginMobileApp/Login       │                                    │
  │ (username/password)              │                                    │
  ├─────────────────────────────────>│                                    │
  │                                  │  forwards to WebService login ────>│
  │                                  │<───────────────────────────────────┤
  │                                  │ on success: signs a JWT,           │
  │                                  │ injects it as `queryMakerKey`      │
  │<─────────────────────────────────┤ in the JSON response               │
  │  { ...loginData,                 │                                    │
  │    queryMakerKey: "<jwt>" }      │                                    │
  │                                  │                                    │
  │ POST /GetSomeData                │                                    │
  │ header: x-querymaker-token       │                                    │
  ├─────────────────────────────────>│                                    │
  │                     verify token, check origin binding                │
  │                     if OK: forward + reissue a renewed token ────────>│
  │<─────────────────────────────────┤<───────────────────────────────────┤
  │  header: x-querymaker-token      │                                    │
  │  (renewed — always overwrite     │                                    │
  │   your stored copy with this)    │                                    │
```

- **Token = JWT**, signed with `JWT_SECRET`, containing the caller's validated `origin` as a claim.
- **Issued** by `loginMobileAppProxy` ([middleware/proxy.js](middleware/proxy.js)) only on a
  successful login response, replacing the `queryMakerKey` field.
- **Required** by `requireQueryMakerToken` ([middleware/auth.js](middleware/auth.js)), which gates
  everything behind `generalProxy` in [server.js](server.js). `/LoginMobileApp`, `/test`,
  `/generate`, and `/deliver` are not gated by this token.
- **Origin-bound**: the token only works from the same origin (`Referer`/`Origin`) it was issued
  for. A token stolen from one environment/app instance can't be replayed against another.
- **Sliding expiration, 7-day window**: every request with a valid token gets a freshly-signed
  token back in the `x-querymaker-token` response header. As long as the client always overwrites
  its stored token with that renewed one, an actively-used session never expires — only a user who
  doesn't open the app for a full 7 days in a row will be asked to log in again. This trades off a
  small "leaked token stays alive if used regularly" risk for near-invisible re-logins, which
  matters for this app's 60+ user base.

## Environment variables

See [.env.example](.env.example):

| Var | Purpose |
|---|---|
| `JWT_SECRET` | Signs/verifies tokens. **Required** — server refuses to start without it. Generate with `node -e "console.log(require('crypto').randomBytes(48).toString('hex'))"`. Use a different value per environment; never commit it. |
| `JWT_TTL` | Sliding expiration window, default `7d`. |
| `PORT` | HTTP port, default `3000`. |

## Client integration

The page must be served from a URL containing `/pages/` (see `PAGES_SEGMENT` in
[middleware/proxy.js](middleware/proxy.js)) so the server can validate the `Referer` and resolve
a target.

**1. Login — obtains the first token**

```js
async function login(username, password) {
  const res = await fetch("/LoginMobileApp/Login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });

  if (!res.ok) throw new Error("Login failed");

  const data = await res.json();
  // data.queryMakerKey is the JWT issued on success
  localStorage.setItem("queryMakerToken", data.queryMakerKey);
  return data;
}
```

**2. Every other proxied call — sends the token, absorbs the renewed one**

```js
async function queryMakerFetch(path, options = {}) {
  const token = localStorage.getItem("queryMakerToken");

  const res = await fetch(path, {
    ...options,
    headers: {
      ...options.headers,
      "x-querymaker-token": token,
    },
  });

  // Sliding expiration: always overwrite the stored token with the renewed
  // one so the session keeps sliding forward.
  const refreshed = res.headers.get("x-querymaker-token");
  if (refreshed) localStorage.setItem("queryMakerToken", refreshed);

  if (res.status === 401) {
    localStorage.removeItem("queryMakerToken");
    // redirect to login / show "session expired"
  }

  return res;
}

// Example usage:
const res = await queryMakerFetch("/GetSomeData", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ someParam: 1 }),
});
const result = await res.json();
```

Notes:

- The path segment after `/LoginMobileApp` (or after the root, for `generalProxy`) is forwarded
  onto `WebService.asmx/`, e.g. `/LoginMobileApp/Login` → `WebService.asmx/Login`,
  `/GetSomeData` → `WebService.asmx/GetSomeData`.
- `localStorage` is the natural spot for this token in a webview-based app; swap in whatever
  secure storage your app already uses if it's a native wrapper instead.

## Troubleshooting 401s

`requireQueryMakerToken` returns one of three error bodies — useful for telling client bugs apart:

| Response | Cause |
|---|---|
| `{"error":"Missing QueryMaker token"}` | No `x-querymaker-token` header sent. |
| `{"error":"Invalid or expired QueryMaker token"}` | Token malformed, signed with a different secret, or past its TTL (no activity for the full window). |
| `{"error":"Token not valid for this origin"}` | Token's `origin` claim doesn't match the current request's validated `Referer`/`Origin` — e.g. token issued on prod, used against test. |

A `502 {"error":"Unable to resolve proxy target"}` is unrelated to auth — it means the request
passed the token check but the `Referer`/`Origin` couldn't be resolved to a WebService target
(missing header, disallowed origin, or no `/pages/` segment in the path).
