# 🛡️ Identity Extraction & Security Hardening

## Overview
Currently, the **AI-Net** system houses `Duende IdentityServer` directly inside the AI Monolithic API as just another module (`Src/Modules/Identity`). While this is convenient for local development, it carries significant security and operational risks.

## The Core Problem
Identity Server is a heavily privileged component. It possesses the RSA Token Signing Keys needed to generate valid JWTs. 
By running it inside the AI API host:
1. **Shared Memory Risk**: Any vulnerability (like an RCE or memory leak) in an external API integration, image-processing library, or code-generation sandbox could allow attackers to dump the process memory and steal Identity's private signing keys. With those, they can forge admin tokens forever.
2. **Coupled Scaling**: Identity logic is mostly CPU-bound (hashing, signing), while AI orchestration logic is heavily I/O-bound (waiting on LLMs). They should be scaled on completely different metrics.

## The Solution: A Dedicated Identity Service

Identity must be treated as Tier-0 Infrastructure. It needs to be moved entirely out of the Monolith into its own dedicated host.

### Implementation Steps

#### 1. Extract the Module
1. Create a brand new .NET API Project called `AI.IdentityService`.
2. Move everything from `Src/Modules/Identity` into this new project.
3. Configure this new project to host Duende IdentityServer, managing its own specialized configuration and EF Core context.

#### 2. Update .NET Aspire Orchestrator
In your `AppHost`, spin up the Identity project separately from the API.

```csharp
// In AppHost.cs
var identityDb = postgres.AddDatabase("identitydb");

var identity = builder.AddProject<Projects.AI_IdentityService>("identity")
                      .WithReference(identityDb);

var api = builder.AddProject<Projects.AI_Api>("ai-api")
                 .WithReference(postgres) // API talks to module DBs
                 .WithReference(identity); // API depends on Identity being alive
```

#### 3. Switch the API to 'Resource' Mode
The monolithic API's `Program.cs` currently acts as an Identity Provider. It should be downgraded to an API Resource that blindly trusts tokens issued by the new service.

Remove IdentityServer from the API and configure standard JWT Bearer Auth:

```csharp
// Inside AI.Api/Program.cs Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Point to the dedicated Identity Container
        options.Authority = builder.Configuration["IdentityService:Url"]; 
        options.Audience = "ai_api_v1";
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });
```

#### 4. Proxy the Network
Using Docker/Kubernetes network rules, ensure the Identity Service is not directly accessible to the internet unless explicitly routed (e.g., exposing only `/connect/token` via your API Gateway). 

## Outcome
The core monolith is now fundamentally stateless regarding authentication. If an attacker exploits an AI module payload, the JWT signing keys remain completely physically isolated in a secure, hardened microservice container.
