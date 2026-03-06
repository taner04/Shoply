#!/usr/bin/env python3
# =========================================================================
# Add Migration Script - Creates a new EF Core migration for the Shoply API
# =========================================================================

import subprocess
import shutil
import platform
import argparse
from shared import console_logger, project_root

# =========================================================================
# Configuration
# =========================================================================

PROJECT = project_root / "src" / "Api" / "Api.csproj"
STARTUP_PROJECT = project_root / "tools" / "AppHost" / "AppHost.csproj"
CONTEXT = "Api.Common.Infrastructure.Data.ApplicationDbContext"
OUTPUT_DIR = project_root / "src" / "Api" / "Common" / "Infrastructure" / "Data" / "Migrations"
CONFIGURATION = "Debug"


# =========================================================================
# Utilities
# =========================================================================

def resolve_cmd(cmd: str) -> str:
    found = shutil.which(cmd)
    if found:
        return found

    if platform.system().lower().startswith("win"):
        for ext in (".exe", ".cmd"):
            found = shutil.which(cmd + ext)
            if found:
                return found

    return cmd


# =========================================================================
# Migration Logic
# =========================================================================

def add_migration(name: str):
    dotnet = resolve_cmd("dotnet")

    console_logger.info(f"Creating migration '{name}'...")

    cmd = [
        dotnet, "ef", "migrations", "add", name,
        "--project", str(PROJECT),
        "--startup-project", str(STARTUP_PROJECT),
        "--context", CONTEXT,
        "--output-dir", str(OUTPUT_DIR),
        "--configuration", CONFIGURATION
    ]

    result = subprocess.run(cmd, cwd=project_root, capture_output=True, text=True)

    if result.returncode != 0:
        console_logger.error("Migration failed.")
        console_logger.info(result.stdout)
        console_logger.info(result.stderr)
        return

    console_logger.success(f"Migration '{name}' created successfully.")


# =========================================================================
# Argument Parser
# =========================================================================

def parse_args():
    parser = argparse.ArgumentParser(description="Create a new EF Core migration.")
    parser.add_argument("name", help="Name of the migration to create.")
    return parser.parse_args()


def main():
    args = parse_args()
    add_migration(args.name)


if __name__ == "__main__":
    main()
