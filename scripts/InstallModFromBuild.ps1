Write-Host "Adofai path: $Env:ADOFAI_PATH"
Write-Host "Build path: $Env:BUILD_DIR"

# Ensure ADOFAI is closed
Write-Host "Closing ADOFAI if open"
$adofai = Get-Process -Name "A Dance of Fire and Ice"
Stop-Process -InputObject $adofai
Get-Process | Where-Object {$_.HasExited}

# Create dir if doesn't exist
Write-Host "Creating directory if doesn't exist..."
New-Item -ItemType Directory -Force -Path "$Env:ADOFAI_PATH/Mods/AdofaiWeb"

# Copy items
Write-Host "Copying items to mod folder..."
Copy-Item "$Env:BUILD_DIR/info.json" -Force -Destination "$Env:ADOFAI_PATH/Mods/AdofaiWeb/"
Copy-Item "$Env:BUILD_DIR/AdofaiWeb.dll" -Force -Destination "$Env:ADOFAI_PATH/Mods/AdofaiWeb/"
Copy-Item "$Env:BUILD_DIR/websocket-sharp.dll" -Force -Destination "$Env:ADOFAI_PATH/Mods/AdofaiWeb/"

# Start ADOFAI
Write-Host "Launching ADOFAI"
Start-Process "steam://run/977950"