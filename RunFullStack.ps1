# Execute o projeto API
Start-Process -FilePath "cmd.exe" -ArgumentList "/k cd src\FleetMGMT.API && dotnet run"

# Execute o projeto UI
Start-Process -FilePath "cmd.exe" -ArgumentList "/k cd src\FleetMGMT.UI && dotnet run"
