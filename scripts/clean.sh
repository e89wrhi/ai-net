#!/bin/bash
# AI-Net Clean Script — removes build artifacts.
# Run from anywhere; resolves the solution root automatically.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_ROOT="$(dirname "$SCRIPT_DIR")"

echo "🧹 Cleaning AI-Net build artifacts..."
dotnet clean "$SOLUTION_ROOT/AI.sln" -c Release

# Deep clean: remove all bin/ and obj/ directories left behind by dotnet clean.
# Uncomment if you encounter stale artifact issues after changing project types.
# echo "🗑️  Deep cleaning bin/ and obj/ directories..."
# find "$SOLUTION_ROOT/Src" -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

echo "✅ Clean complete."
