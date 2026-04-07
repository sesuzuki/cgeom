################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom)


set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/bin/libcgeom)
################################################################################
set(THIRD_PARTY_DIR ${CMAKE_CURRENT_SOURCE_DIR}/dependencies)

# CMake plugin for vscode
include(CMakeToolsHelpers OPTIONAL)

# Add math defintions
add_definitions(-D_USE_MATH_DEFINES)

################################################################################
list(PREPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)

# libigl
option(LIBIGL_COPYLEFT_COMISO "Use CoMISo" ON)
include(libigl)
igl_include(copyleft comiso)

# Open Mesh 3.0 required for libQEx
add_subdirectory(${THIRD_PARTY_DIR}/OpenMesh)
include_directories(${THIRD_PARTY_DIR}/OpenMesh/src)

# libQEx
find_package(OpenMesh REQUIRED)
add_subdirectory(${THIRD_PARTY_DIR}/libQEx)
include_directories(${THIRD_PARTY_DIR}/libQEx/interfaces/c)

# BFF
option(BFF_BUILD_CLI  "Build BFF command line" OFF)
option(BFF_BUILD_GUI  "Build BFF GUI"          OFF)
add_subdirectory(${THIRD_PARTY_DIR}/boundary-first-flattening)
include_directories(${THIRD_PARTY_DIR}/boundary-first-flattening/include)

# Geometry central
add_subdirectory(${THIRD_PARTY_DIR}/geometry-central)
include_directories(${THIRD_PARTY_DIR}/geometry-central/include)

# libdirectional (header-only, with GMP for exact arithmetic)
include_directories(${THIRD_PARTY_DIR}/libdirectional/include)
find_library(GMP_LIBRARY gmp PATHS /opt/local/lib REQUIRED)
find_library(GMPXX_LIBRARY gmpxx PATHS /opt/local/lib REQUIRED)
include_directories(/opt/local/include)

# Instant Meshes (field-aligned remeshing, non-GUI batch library)
include(instant-meshes)

################################################################################
# Library
################################################################################
link_directories(${CMAKE_SOURCE_DIR}/bin/libcgeom)

add_subdirectory(src)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
