# Building on Windows

This document describes the current best-effort workflow for building `cgeom` on Windows.

Windows support in this repository is partially wired up, but it has not yet been fully validated end-to-end on a clean Windows machine. The native build is the critical step. Once that succeeds, the Grasshopper projects are much more straightforward.

## Recommended Setup

- Windows 10 or Windows 11
- Visual Studio 2022
- Desktop development with C++ workload
- MSVC x64 toolchain
- CMake
- Git
- Rhino 8 for Windows if you want to load the Grasshopper plugin
- GMP installed in a known prefix such as `C:\deps\gmp`

## Clone the Repository

Clone recursively so all submodules are available:

```powershell
git clone --recursive <repo-url> cgeom
cd cgeom
```

If you already cloned without submodules:

```powershell
git submodule update --init --recursive
```

## Install GMP

`libdirectional` requires GMP and GMPXX for exact arithmetic.

The CMake files in this repo first try to find GMP under a prefix provided by `GMP_ROOT` or `GMP_DIR`. The expected layout is:

```text
C:\deps\gmp\
  include\
    gmp.h
  lib\
    gmp.lib
    gmpxx.lib
  bin\
    gmp.dll
    gmpxx.dll
```

If your GMP package uses different filenames or a different layout, you can also pass the paths explicitly:

```powershell
cmake -S . -B build-win -G "Visual Studio 17 2022" -A x64 `
  -DGMP_INCLUDE_DIR=C:\deps\gmp\include `
  -DGMP_LIBRARY=C:\deps\gmp\lib\gmp.lib `
  -DGMPXX_LIBRARY=C:\deps\gmp\lib\gmpxx.lib
```

At the moment, this repository does not automatically download and build GMP. That is possible in CMake, but GMP is not as straightforward as a header-only dependency, especially on Windows/MSVC. An unconditional auto-download would be fairly brittle unless we standardize on a package manager workflow such as `vcpkg`.

## Configure the Native Library

From the repository root:

```powershell
cmake -S . -B build-win -G "Visual Studio 17 2022" -A x64 -DGMP_ROOT=C:\deps\gmp
```

Notes:

- `-A x64` is important. Build the native library as 64-bit.
- If you prefer a different install prefix, change `C:\deps\gmp`.
- The first configure may download dependencies such as `libigl`, so internet access is required unless those dependencies are already cached.

## Build the Native Library

```powershell
cmake --build build-win --config Release
```

Expected output:

- the main native library should be placed under `bin\libcgeom`
- on Windows, the main library name should be `cgeom.dll`

Depending on how third-party libraries are built, additional DLLs may also be produced there.

## Build the Grasshopper Plugin

After the native build succeeds:

1. Open [cgeomGH/CGeom/CGeom.sln](/Users/seiichi/Code/cgeom/cgeomGH/CGeom/CGeom.sln) in Visual Studio 2022.
2. Restore NuGet packages if Visual Studio prompts for it.
3. Build the solution in `Release | Any CPU`.

The release outputs are expected under:

```text
bin\cgeomGH
```

The managed wrapper is configured to load the native library by its generic base name, so .NET should resolve `cgeom.dll` on Windows as long as it is copied into that output folder or otherwise available on the DLL search path.

## Create a Distributable Bundle

After building both the native library and the Grasshopper solution, you can stage a distributable folder with:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\package-windows-dist.ps1
```

By default, the script collects files into:

```text
dist\windows\cgeom
```

The staged bundle includes:

- a `plugin` folder with managed and native runtime files
- a `native` folder with the copied native DLLs
- a `licenses` folder with key third-party license files
- [DISTRIBUTION_NOTES.md](/Users/seiichi/Code/cgeom/DISTRIBUTION_NOTES.md)

You can override the default paths, for example:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\package-windows-dist.ps1 `
  -BuildDir build-win `
  -ManagedOutputDir bin\cgeomGH `
  -NativeOutputDir bin\libcgeom `
  -DistDir dist\windows\cgeom
```

## Expected Plugin Outputs

After a successful build, check for these in the output folder:

- `CGeom.gha`
- `CGeomGH.gha`
- `CGeom.dll`
- `cgeom.dll`
- any native dependency DLLs required by `cgeom.dll`

If the Grasshopper project builds but Rhino cannot load the plugin, the usual cause is that one or more native DLLs were not copied alongside the managed assemblies.

## Rhino / Grasshopper Loading

If the plugin output folder is not already in Grasshopper's library search paths:

1. Open Rhino.
2. Run `GrasshopperDeveloperSettings`.
3. Add the plugin output directory to the library folders.
4. Restart Rhino and Grasshopper.

## Troubleshooting

### CMake cannot find GMP

Check that your GMP prefix contains:

- `include\gmp.h`
- `lib\gmp.lib`
- `lib\gmpxx.lib`

Then re-run configure with:

```powershell
cmake -S . -B build-win -G "Visual Studio 17 2022" -A x64 -DGMP_ROOT=C:\deps\gmp
```

If prefix-based lookup still fails, pass the three paths explicitly:

```powershell
cmake -S . -B build-win -G "Visual Studio 17 2022" -A x64 `
  -DGMP_INCLUDE_DIR=C:\deps\gmp\include `
  -DGMP_LIBRARY=C:\deps\gmp\lib\gmp.lib `
  -DGMPXX_LIBRARY=C:\deps\gmp\lib\gmpxx.lib
```

### CMake fails while downloading dependencies

This repo fetches some dependencies during configure. Make sure:

- the machine has internet access
- Git is installed and available on `PATH`
- any firewall or proxy settings allow GitHub access

### MSVC compile errors in third-party code

The repository has been adjusted to remove the most obvious macOS-only assumptions, but some dependencies may still need Windows-specific fixes. If that happens, the first places to inspect are:

- [CMakeLists.txt](/Users/seiichi/Code/cgeom/CMakeLists.txt)
- [src/CMakeLists.txt](/Users/seiichi/Code/cgeom/src/CMakeLists.txt)
- [src/processing.cc](/Users/seiichi/Code/cgeom/src/processing.cc)

### Curvature-aligned remeshing behaves differently on Windows

The file [src/processing.cc](/Users/seiichi/Code/cgeom/src/processing.cc) uses a POSIX `sigsetjmp` recovery path on non-Windows platforms to recover from certain aborts inside `libdirectional`. That path is currently disabled on Windows, so failures in that part of the pipeline may be harsher there until a Windows-specific replacement is implemented.

### Distribution and licensing

For binary distribution planning, read [DISTRIBUTION_NOTES.md](/Users/seiichi/Code/cgeom/DISTRIBUTION_NOTES.md).

The short version is:

- GMP can usually be bundled as runtime DLLs, so end users do not need a separate GMP install.
- The more important release constraint in the current repository is `libQEx`, which is GPLv3 in this checkout.

## Current Recommendation

For the first successful Windows bring-up:

1. Get the native CMake build working first.
2. Verify that `cgeom.dll` is produced.
3. Only then build the `CGeom` / `CGeomGH` solution.
4. If plugin loading fails, inspect missing native DLLs before changing managed code.

This keeps the debugging surface small and makes Windows issues much easier to isolate.
