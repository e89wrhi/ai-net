#!/bin/bash
# AI-Net Migration Generation Script
# Generates EF Core migrations for all module DbContexts via auto-discovery.
# Run from the repository root.

set -euo pipefail

MIGRATION_NAME=${1:-}
if [ -z "$MIGRATION_NAME" ]; then
    read -rp "Enter migration name (e.g. Initial): " MIGRATION_NAME
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
API_PATH="$ROOT_DIR/Src/Api/AI.Api/AI.Api.csproj"
MODULES_ROOT="$ROOT_DIR/Src/Modules"

echo -e "\033[36m🚀 Starting EF Core Migration Auto-Discovery...\033[0m"

# Verify dotnet-ef is available
if ! dotnet tool list 2>/dev/null | grep -q "dotnet-ef"; then
    echo "dotnet-ef tool not found. Restoring..."
    dotnet tool restore
fi

SUCCESS_COUNT=0
FAILURE_COUNT=0

# Use mapfile+for to keep counters in the current shell (avoids pipe subshell)
mapfile -t PROJECTS < <(find "$MODULES_ROOT" -name "*.csproj")

for project in "${PROJECTS[@]}"; do
    PROJECT_DIR=$(dirname "$project")
    DATA_DIR="$PROJECT_DIR/Data"

    if [ -d "$DATA_DIR" ]; then
        mapfile -t CONTEXT_FILES < <(find "$DATA_DIR" -maxdepth 1 -name "*Context.cs")

        for contextFile in "${CONTEXT_FILES[@]}"; do
            CONTEXT_NAME=$(basename "$contextFile" .cs)
            MODULE_NAME=$(basename "$PROJECT_DIR")

            echo "----------------------------------------------------"
            echo -e "\033[33m📦 Module: $MODULE_NAME\033[0m | Context: $CONTEXT_NAME"

            if dotnet ef migrations add "$MIGRATION_NAME" \
                --project "$project" \
                --startup-project "$API_PATH" \
                --context "$CONTEXT_NAME" \
                --output-dir Data/Migrations; then
                echo -e "\033[32m✅ Success\033[0m"
                SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
            else
                echo -e "\033[33m⚠️  No changes or error — skipping $CONTEXT_NAME\033[0m"
                FAILURE_COUNT=$((FAILURE_COUNT + 1))
            fi
        done
    fi
done

echo "----------------------------------------------------"
echo -e "\033[36m✨ Migration process finished — ✅ $SUCCESS_COUNT succeeded, ⚠️  $FAILURE_COUNT skipped/failed.\033[0m"
