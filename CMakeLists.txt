################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom)


set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/bin/libcgeom)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/bin/libcgeom)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
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
set(GMP_ROOT "" CACHE PATH "Prefix where GMP is installed")
set(GMP_DIR "" CACHE PATH "Alternative prefix where GMP is installed")
set(GMP_INCLUDE_DIR "" CACHE PATH "Directory containing gmp.h")
set(GMP_LIBRARY "" CACHE FILEPATH "Path to the GMP library")
set(GMPXX_LIBRARY "" CACHE FILEPATH "Path to the GMPXX library")
set(GMP_SEARCH_PREFIXES
    ${GMP_ROOT}
    ${GMP_DIR}
    $ENV{GMP_ROOT}
    $ENV{GMP_DIR}
    /opt/local
    /usr/local
    /opt/homebrew
)
if(NOT GMP_INCLUDE_DIR)
    find_path(GMP_INCLUDE_DIR gmp.h
        PATH_SUFFIXES include
        PATHS ${GMP_SEARCH_PREFIXES}
    )
endif()
if(NOT GMP_LIBRARY)
    find_library(GMP_LIBRARY gmp
        PATH_SUFFIXES lib
        PATHS ${GMP_SEARCH_PREFIXES}
    )
endif()
if(NOT GMPXX_LIBRARY)
    find_library(GMPXX_LIBRARY gmpxx
        PATH_SUFFIXES lib
        PATHS ${GMP_SEARCH_PREFIXES}
    )
endif()
if(NOT GMP_INCLUDE_DIR OR NOT GMP_LIBRARY OR NOT GMPXX_LIBRARY)
    message(FATAL_ERROR
        "GMP and GMPXX are required.\n"
        "Set GMP_ROOT or GMP_DIR to the install prefix,\n"
        "or pass GMP_INCLUDE_DIR, GMP_LIBRARY, and GMPXX_LIBRARY explicitly.")
endif()
include_directories(${GMP_INCLUDE_DIR})

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
