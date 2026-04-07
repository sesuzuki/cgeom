param(
    [string]$BuildDir = "build-win",
    [string]$ManagedOutputDir = "bin/cgeomGH",
    [string]$NativeOutputDir = "bin/libcgeom",
    [string]$DistDir = "dist/windows/cgeom"
)

$ErrorActionPreference = "Stop"

function Resolve-RepoPath {
    param([string]$PathValue)

    if ([System.IO.Path]::IsPathRooted($PathValue)) {
        return $PathValue
    }

    return Join-Path $PSScriptRoot ".." $PathValue
}

function Ensure-Directory {
    param([string]$PathValue)

    if (-not (Test-Path $PathValue)) {
        New-Item -ItemType Directory -Path $PathValue | Out-Null
    }
}

function Copy-IfExists {
    param(
        [string]$Source,
        [string]$Destination
    )

    if (Test-Path $Source) {
        Copy-Item -Path $Source -Destination $Destination -Force
        return $true
    }

    return $false
}

$buildDirAbs = Resolve-RepoPath $BuildDir
$managedOutputAbs = Resolve-RepoPath $ManagedOutputDir
$nativeOutputAbs = Resolve-RepoPath $NativeOutputDir
$distAbs = Resolve-RepoPath $DistDir

$pluginDir = Join-Path $distAbs "plugin"
$licensesDir = Join-Path $distAbs "licenses"
$nativeDir = Join-Path $distAbs "native"

Ensure-Directory $distAbs
Ensure-Directory $pluginDir
Ensure-Directory $licensesDir
Ensure-Directory $nativeDir

$managedFiles = @(
    "CGeomGH.gha",
    "CGeom.gha",
    "CGeom.dll"
)

foreach ($fileName in $managedFiles) {
    $sourcePath = Join-Path $managedOutputAbs $fileName
    if (-not (Copy-IfExists -Source $sourcePath -Destination $pluginDir)) {
        Write-Warning "Managed output not found: $sourcePath"
    }
}

$nativeSearchPatterns = @(
    "cgeom.dll",
    "QEx.dll",
    "OpenMeshCore*.dll",
    "OpenMeshTools*.dll",
    "gmp*.dll",
    "tbb*.dll"
)

foreach ($pattern in $nativeSearchPatterns) {
    $matches = @()

    if (Test-Path $nativeOutputAbs) {
        $matches += Get-ChildItem -Path $nativeOutputAbs -File -Filter $pattern -ErrorAction SilentlyContinue
    }

    if (Test-Path $buildDirAbs) {
        $matches += Get-ChildItem -Path $buildDirAbs -Recurse -File -Filter $pattern -ErrorAction SilentlyContinue
    }

    $matches = $matches | Sort-Object FullName -Unique

    foreach ($match in $matches) {
        Copy-Item -Path $match.FullName -Destination $pluginDir -Force
        Copy-Item -Path $match.FullName -Destination $nativeDir -Force
    }
}

$licenseFiles = @(
    @{ Source = "LICENSE"; Destination = "cgeom-LICENSE" },
    @{ Source = "dependencies/geometry-central/LICENSE"; Destination = "geometry-central-LICENSE" },
    @{ Source = "dependencies/libQEx/LICENSE"; Destination = "libQEx-GPLv3-LICENSE" },
    @{ Source = "dependencies/boundary-first-flattening/LICENSE"; Destination = "bff-MIT-LICENSE" },
    @{ Source = "dependencies/instant-meshes/LICENSE.txt"; Destination = "instant-meshes-LICENSE" }
)

foreach ($entry in $licenseFiles) {
    $sourcePath = Resolve-RepoPath $entry.Source
    $destinationPath = Join-Path $licensesDir $entry.Destination
    if (-not (Copy-IfExists -Source $sourcePath -Destination $destinationPath)) {
        Write-Warning "License file not found: $sourcePath"
    }
}

$distributionNotes = Resolve-RepoPath "DISTRIBUTION_NOTES.md"
Copy-IfExists -Source $distributionNotes -Destination $distAbs | Out-Null

Write-Host ""
Write-Host "Windows distribution bundle staged in:"
Write-Host "  $distAbs"
Write-Host ""
Write-Host "Plugin folder:"
Write-Host "  $pluginDir"
Write-Host ""
Write-Host "Next checks:"
Write-Host "  1. Verify CGeom.gha and CGeom.dll are present."
Write-Host "  2. Verify cgeom.dll and all required native DLLs are present."
Write-Host "  3. Test loading the plugin in Rhino/Grasshopper on a clean Windows machine."
