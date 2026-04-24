# ─── Runtime Only ─────────────────────────────────────────────────────────────
# This Dockerfile assumes you have built the application on your host machine.
# To build: dotnet publish Src/Api/AI.Api/AI.Api.csproj -c Release -o ./publish
# Then run: docker compose up --build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Install postgresql-client for pg_isready
RUN apt-get update \
 && apt-get install -y --no-install-recommends postgresql-client \
 && rm -rf /var/lib/apt/lists/*

# Run as non-root for security
RUN groupadd --system appgroup && useradd --system -g appgroup appuser

WORKDIR /app

# Copy published output from the host's 'publish' directory
COPY ./publish .

# Copy entrypoint script
COPY scripts/entrypoint.sh ./scripts/entrypoint.sh
RUN chmod +x ./scripts/entrypoint.sh

# Switch to non-root user
USER appuser

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["./scripts/entrypoint.sh"]
