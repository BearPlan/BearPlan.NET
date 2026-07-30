@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo Starting to convert field names in tsv files...
echo.

REM Use Python script to convert snake_case to PascalCase
python "%~dp0convert_to_pascal_case.py"

echo.
echo All tsv files have been processed.
echo.
echo Press any key to exit...
pause >nul