# 🧠 Caching & Vector DB Integration

## Overview
A mature AI project requires exceptional state preservation. Currently, the system leverages PostgreSQL and EventStoreDB. However, as the system grows into features like conversational history (RAG) and repeated Code-Gen tasks, typical relational databases break down.

## The Core Problem
1. **Redundant LLM Invocations**: Generating code or generic summaries using the exact same prompt (`"Write a python script for factorial"`) re-hits OpenAI repeatedly, wasting time and money.
2. **Context Amnesia**: PostgreSQL `LIKE` queries cannot find "semantically similar" messages. This prevents the `ChatBot` or `CodeGen` assistants from looking backward contextually.

## The Solution

### 1. Hard Distributed Caching (Redis / Garnet)
**Microsoft Garnet** is heavily recommended for .NET stacks as an incredibly fast drop-in replacement for Redis.

1. **Add to Aspire**: In your AppHost, provision the cache.
2. **Semantic Caching**: Before passing a prompt to the `AiOrchestration` layer, hash the prompt text + the LLM parameters. Check Garnet for a hit. 
3. **Response Caching Middleware**: For endpoints like `GET /api/v1/models/list`, use built-in ASP.NET Output Caching configured against Garnet.

### 2. Vector Database Intregration (RAG - Retrieval Augmented Generation)

To allow the AI to "remember" past contexts or search documentation implicitly, we need a Vector store.

**Choosing the Store:**
- If you want to remain tightly coupled within PostgreSQL, simply enable the **`pgvector`** extension in your current Postgres container.
- For high enterprise scale, deploy **Qdrant** or **Milvus** alongside your docker-compose.

**Implementation Flow:**
1. **Embeddings**: When a user uploads a PDF or writes a long prompt to `SimpleMD` or `ChatBot`, a background MassTransit consumer fires an event requesting an `Embedding`.
2. **Storage**: The orchestrator converts the text into a float array (e.g., `[0.012, -0.45..., 0.81]`) and saves it to the Vector database.
3. **Retrieval**: When the user asks "How do I fix the error with the database?", the system vectorizes that question, does a `Cosine-Similarity` search against the Vector DB, pulls the top 5 most relevant past context files, injects them into the system prompt invisibly, and sends it to the LLM.

## Outcome
Integrating Garnet caching reduces redundant token costs dramatically. By hooking `pgvector` or Qdrant directly into your MassTransit message flow, you elevate the platform from a "simple prompt router" into an aware, RAG-enabled Enterprise AI brain.
