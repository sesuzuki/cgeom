CGeom Pluging for Grasshopper
===========

# Getting Started

## Compatibility Notice:

The code has been tested on Mac OS environments. Instructions for building the code on Windows will be made available soon.

## System Requirements MacOS:

- **Rhino 8 on Silicon Processors:** This code is compatible with Rhino 8 running on Apple silicon processors.
- **Rhino 7 or 8 on Intel Processors:** Additionally, it is compatible with Rhino 7 or 8 running on Intel processors.

## Building C++ Libraries
The C++ code relies on `libigl` and `SuiteSparse`, which will be downloaded through the `cmake` file.

The code also relies on several dependencies that are included as submodules:
[OpenMesh](https://gitlab.vci.rwth-aachen.de:9000/OpenMesh/OpenMesh.git),
[libQEx](https://github.com/hcebke/libQEx.git),
[BFF](https://github.com/GeometryCollective/boundary-first-flattening.git),

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
