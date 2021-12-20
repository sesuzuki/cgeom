################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom)

# CMake plugin for vscode
include(CMakeToolsHelpers OPTIONAL)

################################################################################

list(PREPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)

# libigl
option(LIBIGL_WITH_COMISO            "Use CoMiso"         ON)
include(libigl)

include_directories(
    ${CMAKE_SOURCE_DIR}/dependencies/OpenMesh/include
    ${CMAKE_SOURCE_DIR}/dependencies/QEx/include
    ) 
link_directories(
    ${CMAKE_SOURCE_DIR}/dependencies/OpenMesh/lib
    ${CMAKE_SOURCE_DIR}/dependencies/QEx/lib
)

################################################################################
# Library
################################################################################
add_subdirectory(src)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
