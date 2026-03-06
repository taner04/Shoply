#!/usr/bin/env python3
# =========================================================================
# Shoply Build Script (Cross-Platform)
# =========================================================================

import subprocess
import shutil
import platform
import argparse
from pathlib import Path

from shared import console_logger, project_root

# =========================================================================
# Command Resolver (npm on Windows = npm.cmd)
# =========================================================================

def resolve_cmd(cmd: str) -> str:
    found = shutil.which(cmd)
    if found:
        return found

    # Windows compatibility
    if platform.system().lower().startswith("win"):
        for ext in (".cmd", ".exe", ".bat"):
            found = shutil.which(cmd + ext)
            if found:
                return found

    return cmd


# =========================================================================
# Requirements
# =========================================================================

def is_docker_running() -> bool:
    docker = resolve_cmd("docker")
    try:
        subprocess.check_output([docker, "info"], stderr=subprocess.STDOUT)
        return True
    except Exception:
        return False


def does_migration_exist() -> bool:
    migrations_folder = project_root / "src" / "Api" / "Common" / "Infrastructure" / "Data" / "Migrations"
    return migrations_folder.exists()

def does_appsettings_api_exist() -> bool:
    appsettings_file = project_root / "src" / "Api" / "appsettings.json"
    return appsettings_file.exists()

def does_appsettings_migration_service_exist() -> bool:
    appsettings_file = project_root / "tools" / "MigrationService" / "appsettings.json"
    return appsettings_file.exists()


def check_requirements(docker_required: bool) -> bool:
    missing = []

    checks = [
        (
            "Migrations",
            not does_migration_exist(),
            "Migrations folder missing! Run the create_migration.py script or via dotnet ef tools."
        ),
        (
            "appsettings.json (API)",
            not does_appsettings_api_exist(),
            "appsettings.json file is missing! Run setup.py"
        ),
        (
            "appsettings.json (MigrationService)",
            not does_appsettings_migration_service_exist(),
            "appsettings.json file is missing for MigrationService! Run setup.py"
        ),
    ]

    for name, condition, message in checks:
        if condition:
            missing.append((name, message))

    if not missing:
        return True

    console_logger.error("Missing requirements:\n")

    header = f"{'Requirement'.ljust(20)} | Message"
    separator = "-" * 100
    print(header)
    print(separator)

    for name, message in missing:
        print(f"{name.ljust(20)} | {message}")

    return False

# =========================================================================
# .NET
# =========================================================================

def dotnet_restore():
    console_logger.info("Restoring .NET dependencies...")
    subprocess.run(["dotnet", "restore"], cwd=project_root, check=True)


def dotnet_build_project(project_path: str) -> None:
    subprocess.run(
        [
            "dotnet", "build", project_path,
            "-c", "Debug", "--no-restore",
            "--verbosity", "quiet", "/m"
        ],
        cwd=project_root,
        check=True
    )


def build_dotnet_projects() -> None:
    console_logger.info("Building .NET projects...")

    dotnet_build_project("src/Api/Api.csproj")
    dotnet_build_project("tests/UnitTests/UnitTests.csproj")
    dotnet_build_project("tools/AppHost/AppHost.csproj")
    dotnet_build_project("tools/MigrationService/MigrationService.csproj")
    dotnet_build_project("tools/ServiceDefaults/ServiceDefaults.csproj")

    console_logger.success("All .NET projects built successfully.")


def run_dotnet_test_project(project: Path) -> bool:
    console_logger.info(f"Running tests: {project.name}")

    result = subprocess.run(
        ["dotnet", "test", str(project), "--no-build", "--verbosity", "quiet", "/m"],
        cwd=project.parent,
        capture_output=True,
        text=True
    )

    if result.returncode == 0:
        console_logger.success(f"Tests passed: {project.name}")
        return True

    console_logger.error(f"Tests failed: {project.name}")
    console_logger.info(result.stdout)
    console_logger.info(result.stderr)
    return False


def find_test_projects() -> list[Path]:
    return list((project_root / "tests").rglob("*Tests.csproj"))


def run_all_tests() -> bool:
    console_logger.info("Running all .NET tests...")

    test_projects = find_test_projects()
    if not test_projects:
        console_logger.error("No test projects found.")
        return False

    all_success = True
    for project in test_projects:
        if not run_dotnet_test_project(project):
            all_success = False

    return all_success




# =========================================================================
# Argument Parser
# =========================================================================

def parse_args():
    parser = argparse.ArgumentParser(description="Taskly Build Script")

    parser.add_argument(
        "--dotnet",
        action="store_true",
        help="Build .NET projects (default)"
    )

    parser.add_argument(
        "--test",
        action="store_true",
        help="Run tests after building"
    )

    return parser.parse_args()


# =========================================================================
# Main
# =========================================================================

def main() -> None:
    args = parse_args()

    run_tests = args.test

    if not check_requirements(docker_required=False):
        print()
        console_logger.error("Build aborted due to unmet requirements.")
        return

    dotnet_restore()
    build_dotnet_projects()

    if run_tests and not run_all_tests():
        print()
        console_logger.error("Tests failed — stopping build.")
        return

    print()
    console_logger.success("Build completed successfully.")


if __name__ == "__main__":
    main()
