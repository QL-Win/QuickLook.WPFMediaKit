$ErrorActionPreference = 'Stop'

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

$root = Split-Path $PSScriptRoot -Parent

# nuget pack does not build EVR Presenter; it has to be built (Release, Win32 and x64) beforehand.
$nativeDlls = 'EVR Presenter\bin\Release\x86\EVRPresenter32.dll', 'EVR Presenter\bin\Release\x64\EVRPresenter64.dll'
$missing = $nativeDlls | Where-Object { -not (Test-Path (Join-Path $root $_)) }
if ($missing) {
    Write-Error "Build EVR Presenter (Release, Win32 and x64) first. Missing: $($missing -join ', ')"
}

dotnet build (Join-Path $root 'Source\WPF MediaKit.csproj') -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

nuget pack (Join-Path $PSScriptRoot 'Package.nuspec') -OutputDirectory $PSScriptRoot -NonInteractive
exit $LASTEXITCODE
