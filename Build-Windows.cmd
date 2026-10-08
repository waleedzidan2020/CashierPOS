@echo off
cd /d "%~dp0"
dotnet --version
if errorlevel 1 goto failed
dotnet restore CashierPOS.sln
if errorlevel 1 goto failed
dotnet build CashierPOS.sln -c Release -m:1
if errorlevel 1 goto failed
dotnet test CashierPOS.Tests\CashierPOS.Tests.csproj -c Release -m:1
if errorlevel 1 goto failed
dotnet publish CashierPOS.WPF -c Release -r win-x64 --self-contained true -p:PublishTrimmed=false -o Portable -m:1
if errorlevel 1 goto failed
echo Build and tests completed. Open Start-CashierPOS.cmd.
pause
exit /b 0
:failed
echo Build stopped. Read the error above.
pause
exit /b 1
