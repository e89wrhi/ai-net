#!/bin/bash
# AI-Net — Apply EF Core migrations for every module DbContext.
# Run from the repository root.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
API_PATH="$ROOT_DIR/Src/Api/AI.Api/AI.Api.csproj"
MODULES_ROOT="$ROOT_DIR/Src/Modules"

echo -e "\033[36m🚀 Starting Auto-Discovery for Database Updates...\033[0m"

SUCCESS_COUNT=0
FAILURE_COUNT=0

# Collect all projects first so the while loop runs in the current shell
# (avoids the classic bash subshell pipe problem where $? / counters are lost)
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
            echo -e "\033[33m📦 Module: $MODULE_NAME\033[0m"
            echo "🆙 Updating: $CONTEXT_NAME"

            if dotnet ef database update \
                --project "$project" \
                --startup-project "$API_PATH" \
                --context "$CONTEXT_NAME"; then
                echo -e "\033[32m✅ Database updated successfully for $CONTEXT_NAME\033[0m"
                SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
            else
                echo -e "\033[31m❌ Error occurred during database update for $CONTEXT_NAME\033[0m"
                FAILURE_COUNT=$((FAILURE_COUNT + 1))
            fi
        done
    fi
done

echo "----------------------------------------------------"
echo -e "\033[36m✨ Done — ✅ $SUCCESS_COUNT succeeded, ❌ $FAILURE_COUNT failed.\033[0m"

# Exit non-zero if any migration failed
[ "$FAILURE_COUNT" -eq 0 ] || exit 1
