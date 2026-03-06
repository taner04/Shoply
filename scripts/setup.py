#!/usr/bin/env python3
# =========================================================================
# Shoply Setup Script
# Creates required configuration files if they do not yet exist.
# This script is intended to be run ONCE after cloning the repository.
# =========================================================================

import json
from pathlib import Path

from shared import project_root, console_logger
from create_migration import add_migration

# =========================================================================
# Content Templates
# =========================================================================

LOGGING_TEMPLATE = {
    "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
    }
}

APPSETTINGS_API_TEMPLATE = {
    "Logging": LOGGING_TEMPLATE,
    "AllowedHosts": "*",
    "Auth0Config": {
        "Domain": "your-auth0-domain",
        "Audience": "your-auth0-audience",
        "ClientId": "your-auth0-client-id",
        "ClientSecret": "your-auth0-client-secret",
        "UsePersistentStorage": False
    },
    "ConnectionStrings": {
        "AzureBlobStorage": "your-azure-blob-storage-connection-string"
    }
}

APPSETTINGS_MIGRATION_SERVICE_TEMPLATE = {
    "Logging": LOGGING_TEMPLATE,
    "ConnectionStrings": {
        "DefaultConnection": "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=shoply;"
    }
}

# =========================================================================
# Setup Tasks
# =========================================================================
def create_appsettings(file_path: Path, content: dict) -> None:
    """Create appsettings.json file with given content."""
    if file_path.exists():
        console_logger.success("appsettings.json already exists")
        return

    file_path.parent.mkdir(parents=True, exist_ok=True)
    file_path.write_text(json.dumps(content, indent=2))

    console_logger.success("Created appsettings.json")


def create_appsettings_api() -> None:
    """Create appsettings.json for the API project."""
    create_appsettings(
        project_root / "src" / "Api" / "appsettings.json", APPSETTINGS_API_TEMPLATE
    )


def create_appsettings_migration_service() -> None:
    """Create appsettings.json for the MigrationService project."""
    create_appsettings(
        project_root / "tools" / "MigrationService" / "appsettings.json",
        APPSETTINGS_MIGRATION_SERVICE_TEMPLATE,
    )


# =========================================================================
# Main Entry Point
# =========================================================================


def main() -> None:
    """Initialize all required configuration files."""
    console_logger.info("Initializing Shoply configuration files...")
    create_appsettings_api()
    create_appsettings_migration_service()
    console_logger.success("Initialization completed! You can now configure your values.")

    console_logger.info("Creating initial database migration...")
    add_migration("InitialCreate")

if __name__ == "__main__":
    main()
