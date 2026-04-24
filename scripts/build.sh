#!/bin/bash
# AI-Net Build Script
# Builds the entire solution in Release configuration.
# Run from the repository root or from any directory.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_ROOT="$(dirname "$SCRIPT_DIR")"

echo "🔨 Building AI-Net solution..."
dotnet restore "$SOLUTION_ROOT/AI.sln"
dotnet build "$SOLUTION_ROOT/AI.sln" -c Release --no-restore
echo "✅ Build succeeded."
