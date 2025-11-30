@echo off
echo Stopping any running test processes...
taskkill /F /IM dotnet.exe /FI "WINDOWTITLE eq *test*" 2>nul
taskkill /F /IM Lifx.Api.Test.exe 2>nul
timeout /t 2 /nobreak >nul
echo.
echo Cleaning build output...
dotnet clean
timeout /t 1 /nobreak >nul
echo.
echo Building solution...
dotnet build
echo.
echo Done! You can now run: dotnet test --filter "FullyQualifiedName~Lan"
