# Distribution Notes

This repository can likely be packaged into a ready-to-distribute Windows bundle, but distribution is not only a build question. It is also a dependency and license question.

## Recommended Distribution Shape

The most practical deliverable is a folder containing:

- `CGeom.gha`
- `CGeom.dll`
- `cgeom.dll`
- all required native dependency DLLs
- third-party license files

That is usually easier and safer than trying to force every dependency into one monolithic binary.

## GMP

End users do not need to install GMP system-wide if the required GMP runtime DLLs are shipped next to the plugin binaries.

In other words, GMP can be bundled as part of the Windows distribution folder.

## Core Dependency Notes

These are the key third-party dependencies directly visible in this repository's build graph:

| Dependency | Role | Distribution note |
| --- | --- | --- |
| `geometry-central` | geometry processing | MIT license |
| `boundary-first-flattening` | parameterization | MIT license |
| `instant-meshes` | remeshing | BSD-style license |
| `OpenMesh` | mesh data structures | check bundled license files in your final release process |
| `GMP` / `GMPXX` | exact arithmetic for `libdirectional` | can usually be redistributed as runtime DLLs, subject to their license terms |
| `libQEx` | quad extraction | GPLv3 in this repository |

## Important Licensing Constraint

The biggest release constraint in the current repository is `libQEx`, not GMP.

`libQEx` is documented as GPLv3 here:

- [dependencies/libQEx/README.md](/Users/seiichi/Code/cgeom/dependencies/libQEx/README.md)
- [dependencies/libQEx/LICENSE](/Users/seiichi/Code/cgeom/dependencies/libQEx/LICENSE)

Its README also mentions commercial licensing may be available separately.

This means a distributable bundle may be technically straightforward, but you should review your intended release model before shipping binaries publicly.

## Practical Recommendation

For a first Windows release candidate:

1. Build the native C++ outputs.
2. Build the Grasshopper projects.
3. Bundle all runtime DLLs in one plugin folder.
4. Include third-party license files.
5. Test on a clean Windows machine.
6. Review the `libQEx` license position before public distribution.
