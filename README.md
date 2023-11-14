# cgeom
A framework for geometric computation in Grasshopper.

# Getting Started
Clone this repository *recursively* to download all submodules.

```bash
git clone --recursive git@github.com:sesuzuki/cgeom.git
```

Build the C++ code using `cmake` and the GH plugin using Visual Studio.

```bash
cd cgeom
mkdir build && cd build
cmake .. -GNinja
ninja
```
