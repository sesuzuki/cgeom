# Install script for directory: /Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools

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
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Decimater" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/BaseDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/BaseDecimaterT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/CollapseInfoT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/DecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/DecimaterT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/McDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/McDecimaterT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/MixedDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/MixedDecimaterT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModAspectRatioT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModAspectRatioT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModBaseT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModEdgeLengthT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModEdgeLengthT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModHausdorffT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModHausdorffT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModIndependentSetsT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModNormalDeviationT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModNormalFlippingT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModProgMeshT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModProgMeshT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModQuadricT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModQuadricT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModRoundnessT.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Dualizer" TYPE FILE FILES "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Dualizer/meshDualT.hh")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Kernel_OSG" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/ArrayKernelT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/AttribKernelT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/PropertyKernel.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/PropertyT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/Traits.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/TriMesh_OSGArrayKernelT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/VectorAdapter.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/bindT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Kernel_OSG/color_cast.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Smoother" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/BaseDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/DecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/McDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/MixedDecimaterT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModAspectRatioT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModEdgeLengthT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModHausdorffT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModProgMeshT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Decimater/ModQuadricT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Smoother/JacobiLaplaceSmootherT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Smoother/LaplaceSmootherT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Smoother/SmootherT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Smoother/smooth_mesh.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Subdivider/Adaptive/Composite" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/CompositeT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/CompositeT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/CompositeTraits.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/RuleInterfaceT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/RulesT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/RulesT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Adaptive/Composite/Traits.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Subdivider/Uniform" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/CatmullClarkT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/CatmullClarkT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/CompositeLoopT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/CompositeSqrt3T.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/LongestEdgeT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/LoopT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/ModifiedButterFlyT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/Sqrt3InterpolatingSubdividerLabsikGreinerT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/Sqrt3T.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/SubdividerT.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Subdivider/Uniform/Composite" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/Composite/CompositeT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/Composite/CompositeT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Subdivider/Uniform/Composite/CompositeTraits.hh"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/Utils" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/Config.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/GLConstAsString.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/Gnuplot.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/Gnuplot.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/HeapT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/MeshCheckerT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/MeshCheckerT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/NumLimitsT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/StripifierT.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/StripifierT.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/TestingFramework.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/Timer.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/conio.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/Utils/getopt.h"
    )
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/OpenMesh/Tools/VDPM" TYPE FILE FILES
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/MeshTraits.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/StreamingDef.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VFront.cc"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VFront.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VHierarchy.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VHierarchyNode.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VHierarchyNodeIndex.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/VHierarchyWindow.hh"
    "/Users/suzuki/Code/cgeom/dependencies/OpenMesh/src/OpenMesh/Tools/VDPM/ViewingParameters.hh"
    )
endif()

