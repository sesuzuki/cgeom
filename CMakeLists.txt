################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom)

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/bin)

# CMake plugin for vscode
include(CMakeToolsHelpers OPTIONAL)

################################################################################
list(PREPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)

# libigl
option(LIBIGL_WITH_COMISO            "Use CoMiso"         ON)
include(libigl)

# Open Mesh 3.0 required for libQEx
add_subdirectory(dependencies/OpenMesh)
include_directories(${CMAKE_SOURCE_DIR}/dependencies/OpenMesh/src)

# libQEx
find_package(OpenMesh REQUIRED)
add_subdirectory(dependencies/libQEx)
include_directories(${CMAKE_SOURCE_DIR}/dependencies/libQEx/interfaces/c)


################################################################################
# Library
################################################################################
link_directories(${CMAKE_SOURCE_DIR}/bin)

add_subdirectory(src)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
