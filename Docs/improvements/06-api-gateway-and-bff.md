# 🚪 API Gateway & Traffic Routing (YARP & BFF)

## Overview
Currently, the monolithic `AI.Api` functions as the sole entry point for the internet. It maps everything directly using Minimal APIs.

## The Core Problem
1. **Lack of Tenant Shaping**: If a single angry tenant or bad bot floods your chat endpoints, the main `.NET` thread pool exhausts, bringing the entire ecosystem down.
2. **Fat UI Clients**: The front-end application holds raw JWT access tokens and hits API endpoints directly, risking token theft (XSS).
3. **No Domain Layering**: Mixing edge-routing, bot-protection logic, and core AI processing in the single host creates heavy code coupling.

## The Solution: A Two-Stage Front Door

### 1. Introduce YARP as the Ingress Edge
**YARP (Yet Another Reverse Proxy)** is Microsoft's high-performance proxy built on .NET.

1. **Create a `AI.Gateway` Project**: Spin up a minimal project running purely YARP.
2. **Move Rate Limiting to the Edge**: Configure YARP to enforce **Token Bucket** rate limiting per IP address or Tenant ID *before* the request even touches the main AI API.

```json
// YARP appsettings.json snippet
"ReverseProxy": {
  "Routes": {
    "codegen-route": {
      "ClusterId": "ai-api-cluster",
      "Match": { "Path": "/api/codegen/{**catch-all}" },
      "RateLimiterPolicy": "StrictLlmPolicy" // Blocks abuse early!
    }
  },
  "Clusters": {
    "ai-api-cluster": {
      "Destinations": {
        "node1": { "Address": "http://ai-api:8080" }
      }
    }
  }
}
```

### 2. Implement the Backend-For-Frontend (BFF)
Instead of forcing the Vue/React client to negotiate tokens:

1. **Create `AI.BFF`**: This serves as a lightweight shim situated closely to the UI.
2. **Cookie Auth**: The BFF negotiates the complicated OpenID Connect ping-pong with `IdentityServer`.
3. **Secure Cookies**: The BFF issues an encrypted, HTTP-Only `SameSite=Strict` session cookie back to the browser.
4. **Proxy Calls**: When the browser fetches `/api/codegen`, the BFF catches it, extracts the raw JWT from its secure vault, attaches it as a `Bearer` header, and passes it securely to YARP -> Monolith.

## Outcome
The core web API becomes fully sealed off from the wild internet. It processes clean, rate-limited, appropriately authenticated workloads. XSS attacks on the client side can't steal Identity Tokens, and bots are dropped by YARP without impacting AI inference speeds.
