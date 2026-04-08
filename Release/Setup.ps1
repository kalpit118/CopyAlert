<#
.SYNOPSIS
    Installer script for CopyAlert.
#>

$AppName = "CopyAlert"
$ExeName = "CopyAlert.exe"
$InstallDir = Join-Path $env:LOCALAPPDATA $AppName
$ShortcutName = "$AppName.lnk"

# 1. Create Install Directory
if (-not (Test-Path $InstallDir)) {
    New-Item -Path $InstallDir -ItemType Directory -Force
}

# 2. Copy Files
Write-Host "Installing $AppName to $InstallDir..." -ForegroundColor Cyan
Copy-Item -Path ".\$ExeName" -Destination "$InstallDir\$ExeName" -Force
if (Test-Path ".\CopyAlert_Logo.png") {
    Copy-Item -Path ".\CopyAlert_Logo.png" -Destination "$InstallDir\CopyAlert_Logo.png" -Force
}
if (Test-Path ".\LICENSE") {
    Copy-Item -Path ".\LICENSE" -Destination "$InstallDir\LICENSE" -Force
}

# 3. Create Shortcuts
$WshShell = New-Object -ComObject WScript.Shell

# Desktop Shortcut
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$DesktopShortcut = $WshShell.CreateShortcut(Join-Path $DesktopPath $ShortcutName)
$DesktopShortcut.TargetPath = Join-Path $InstallDir $ExeName
$DesktopShortcut.WorkingDirectory = $InstallDir
$DesktopShortcut.IconLocation = Join-Path $InstallDir $ExeName # Uses embedded icon
$DesktopShortcut.Save()

# Start Menu Shortcut
$StartMenuPath = [Environment]::GetFolderPath("StartMenu")
$ProgramsPath = Join-Path $StartMenuPath "Programs"
$StartMenuShortcut = $WshShell.CreateShortcut(Join-Path $ProgramsPath $ShortcutName)
$StartMenuShortcut.TargetPath = Join-Path $InstallDir $ExeName
$StartMenuShortcut.WorkingDirectory = $InstallDir
$StartMenuShortcut.Save()

Write-Host "$AppName installed successfully!" -ForegroundColor Green
Write-Host "Shortcuts created on Desktop and Start Menu." -ForegroundColor Gray
