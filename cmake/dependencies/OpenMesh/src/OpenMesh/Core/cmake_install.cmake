# Install script for directory: /Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "/usr/local")
endif()
string(REGEX REPLACE "/$" "" CMAKE_INSTALL_PREFIX "${CMAKE_INSTALL_PREFIX}")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE "^[^A-Za-z0-9_]+" ""
           CMAKE_INSTALL_CONFIG_NAME "${BUILD_TYPE}")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME "Debug")
  endif()
  message(STATUS "Install configuration: \"${CMAKE_INSTALL_CONFIG_NAME}\"")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS "Install component: \"${COMPONENT}\"")
    set(CMAKE_INSTALL_COMPONENT "${COMPONENT}")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

# Is this installation the result of a crosscompile?
if(NOT DEFINED CMAKE_CROSSCOMPILING)
  set(CMAKE_CROSSCOMPILING "FALSE")
endif()

# Set default install directory permissions.
if(NOT DEFINED CMAKE_OBJDUMP)
  set(CMAKE_OBJDUMP "/Library/Developer/CommandLineTools/usr/bin/objdump")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/Geometry" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/Config.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/LoopSchemeMaskT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/MathDefs.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/NormalConeT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/NormalConeT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/Plane3d.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/QuadricT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/VectorT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Geometry/VectorT_inc.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/IO" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/BinaryHelper.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/IOInstances.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/IOManager.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/MeshIO.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/OFFFormat.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/OMFormat.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/OMFormat.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/OMFormatT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/Options.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_binary.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_binary_spec.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_binary_vector_of_bool.inl"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_binary_vector_of_fundamentals.inl"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_binary_vector_of_string.inl"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_rbo.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_store.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/SR_types.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/StoreRestore.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/IO/importer" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/importer/BaseImporter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/importer/ImporterT.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/IO/exporter" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/exporter/BaseExporter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/exporter/ExporterT.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/IO/reader" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/BaseReader.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/OBJReader.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/OFFReader.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/OMReader.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/PLYReader.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/reader/STLReader.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/IO/writer" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/BaseWriter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/OBJWriter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/OFFWriter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/OMWriter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/PLYWriter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/IO/writer/STLWriter.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/Mesh" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/ArrayItems.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/ArrayKernel.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/ArrayKernelT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/AttribKernelT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/Attributes.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/BaseKernel.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/BaseMesh.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/Casts.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/CirculatorsT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/FinalMeshItemsT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/Handles.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/IteratorsT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/PolyConnectivity.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/PolyMeshT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/PolyMeshT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/PolyMesh_ArrayKernelT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/Status.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/Traits.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/TriConnectivity.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/TriMeshT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/TriMeshT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/TriMesh_ArrayKernelT.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/Mesh/gen" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/gen/circulators_header.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/gen/circulators_template.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/gen/footer.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/gen/iterators_header.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Mesh/gen/iterators_template.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/System" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/OpenMeshDLLMacros.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/compiler.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/config.h"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/config.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/mostream.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/System/omstream.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Core/Utils" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/AutoPropertyHandleT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/BaseProperty.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/Endian.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/GenProg.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/Noncopyable.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/Property.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/PropertyContainer.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/PropertyManager.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/RandomNumberGenerator.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/SingletonT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/SingletonT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/color_cast.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/vector_cast.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Core/Utils/vector_traits.hh"
    )
endif()

