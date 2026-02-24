#!/bin/bash

ROOT_DIR=$(pwd)
API_PATH="$ROOT_DIR/Src/Api/AI.Api/AI.Api.csproj"
MODULES_ROOT="$ROOT_DIR/Src/Modules"

echo -e "\033[36m🚀 Starting Auto-Discovery for Database Updates...\033[0m"

# Find all csproj files in Modules
find "$MODULES_ROOT" -name "*.csproj" | while read -r project; do
    PROJECT_DIR=$(dirname "$project")
    DATA_DIR="$PROJECT_DIR/Data"
    
    if [ -d "$DATA_DIR" ]; then
        # Find context files
        find "$DATA_DIR" -name "*Context.cs" | while read -r contextFile; do
            CONTEXT_NAME=$(basename "$contextFile" .cs)
            MODULE_NAME=$(basename "$PROJECT_DIR")
            
            echo "----------------------------------------------------"
            echo -e "\033[33m📦 Module: $MODULE_NAME\033[0m"
            echo "🆙 Updating: $CONTEXT_NAME"
            
            dotnet ef database update \
                --project "$project" \
                --startup-project "$API_PATH" \
                --context "$CONTEXT_NAME"
            
            if [ $? -eq 0 ]; then
                echo -e "\033[32m✅ Database updated successfully for $CONTEXT_NAME\033[0m"
            else
                echo -e "\033[31m❌ Error occurred during database update for $CONTEXT_NAME\033[0m"
            fi
        done
    fi
done

echo "----------------------------------------------------"
echo -e "\033[36m✨ All databases processed!\033[0m"
