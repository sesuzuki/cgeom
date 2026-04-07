CGeom Pluging for Grasshopper
===========

# Getting Started

## Compatibility Notice:

The code has been tested primarily on macOS environments.

Windows support is partially wired up in the build files, but still depends on having a Windows-compatible toolchain and third-party dependencies installed, especially GMP. The native library and Grasshopper wrapper are no longer hardcoded to macOS artifacts, but the curvature-aligned remeshing path still uses a reduced safety fallback on Windows because the POSIX `sigsetjmp` recovery path is unavailable there.

For a dedicated Windows walkthrough, see [BUILDING_WINDOWS.md](/Users/seiichi/Code/cgeom/BUILDING_WINDOWS.md).

## System Requirements MacOS:

- **Rhino 8 on Silicon Processors:** This code is compatible with Rhino 8 running on Apple silicon processors.
- **Rhino 7 or 8 on Intel Processors:** Additionally, it is compatible with Rhino 7 or 8 running on Intel processors.

## Building C++ Libraries
The C++ code relies on `libigl` (v2.5.0) and `SuiteSparse`, which will be downloaded through the `cmake` file. Eigen 3.4 is fetched automatically as part of libigl.

The code also relies on several dependencies that are included as submodules:
[OpenMesh](https://gitlab.vci.rwth-aachen.de:9000/OpenMesh/OpenMesh.git),
[libQEx](https://github.com/hcebke/libQEx.git),
[BFF](https://github.com/GeometryCollective/boundary-first-flattening.git),
[geometry-central](https://github.com/nmwsharp/geometry-central.git),
[libdirectional](https://github.com/avaxman/libdirectional.git).

### Additional System Dependency: GMP

[libdirectional](https://github.com/avaxman/libdirectional.git) requires [GMP](https://gmplib.org) (GNU Multiple Precision Arithmetic Library) for exact arithmetic. Install it before building:

**macOS (MacPorts):**
```bash
sudo port install gmp
```

**macOS (Homebrew):**
```bash
brew install gmp
```

**Windows:**
Install GMP and point CMake at it with `GMP_ROOT` or `GMP_DIR`, for example:

```powershell
cmake -S . -B build -G "Visual Studio 17 2022" -A x64 -DGMP_ROOT=C:\path\to\gmp
```

## Obtaining and Building

Clone this repository *recursively* so that its submodules are also downloaded:

```bash
git clone --recursive git@github.com:sesuzuki/cgeom.git
```

Build the C++ code using `cmake` and your favorite build system. For example, with [`ninja`](https://ninja-build.org):

```bash
cd cgeom
mkdir build && cd build
cmake .. -GNinja
ninja
```

## Building C# Plugin MacOs

### Visual Studio for Mac (if not installed)
Download and install [Visual Studio 2022](https://visualstudio.microsoft.com/vs/mac/)

Check if `Mono` is installed with Visual Studio.
Open Visual Studio for Mac and click on 'Visual Studio' in the top menu bar. 
Select 'About Visual Studio' from the dropdown menu and, in the dialog that opens, you should see version information and installed components.
If `Mono` is not listed, download and install [Mono](https://www.mono-project.com/download/stable/)

Download the latest [RhinoVisualStudioExtensions](https://github.com/mcneel/RhinoCommonXamarinStudioAddin/releases).
Launch Visual Studio => Navigate to Visual Studio>Extensions.. => Click "Install from file" => Select the .mpack file.

Quit and Restart Visual Studio => Navigate to Extensions Studio>Add-ins..>Installed tab => Verify that RhinoCommon Plugin Support exists under the Debugging category.

## Building 
Open .sln project from `cgeomGH/CGeom/` in Visual Studio and build it. This will copy all the .dll and .gha files (plugin files) in `cgeom/bin/cgeomGH`.

If the 'bin' folder is not referenced in Grasshopper, open Rhino, enter `GrasshopperDeveloperSettings` into the Command console, and add the path to the 'bin' folder to the Library Folders. Restart Rhino
