#!/usr/bin/env python3
# =========================================================================
# Shared Utilities Module
# Common logging functions and utilities for Shoply scripts
# =========================================================================

from pathlib import Path

# =========================================================================
# Get the project root directory (parent of scripts folder)
# =========================================================================
def __get_project_root() -> Path:
    return Path(__file__).parent.parent

class ConsoleLogger:
    _RED = '\033[31m'
    _GREEN = '\033[32m'
    _YELLOW = '\033[33m'
    _BLUE = '\033[34m'
    _RESET = '\033[0m'

    def success(self, message: str) -> None:
        print(f"{self._GREEN}[SUCCESS]{self._RESET} {message}")

    def error(self, message: str) -> None:
        print(f"{self._RED}[ERROR]{self._RESET} {message}")

    def info(self, message: str) -> None:
        print(f"{self._BLUE}[INFO]{self._RESET} {message}")

    def warning(self, message: str) -> None:
        print(f"{self._YELLOW}[WARNING]{self._RESET} {message}")


console_logger = ConsoleLogger()
project_root = __get_project_root()
