################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom VERSION 0.1.0)

# CMake plugin for vscode
include(CMakeToolsHelpers OPTIONAL)

################################################################################

set(THIRD_PARTY_DIR ${CMAKE_CURRENT_SOURCE_DIR}/3rdparty)
list(APPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)

# libigl
option(LIBIGL_WITH_OPENGL            "Use OpenGL"         OFF)
option(LIBIGL_WITH_OPENGL_GLFW       "Use GLFW"           OFF)
option(LIBIGL_WITH_OPENGL_GLFW_IMGUI "Use IMGUI"          OFF)
option(LIBIGL_WITH_VIEWER            "Use OpenGL viewer"  OFF)
option(LIBIGL_USE_STATIC_LIBRARY     "Use static library" ON)

find_package(LIBIGL QUIET)

################################################################################
# Library
################################################################################
add_library(cgeom SHARED cgeom.h cgeom.cc)
target_link_libraries(cgeom igl::core)
set_target_properties(cgeom PROPERTIES CXX_STANDARD 14)
set_target_properties(cgeom PROPERTIES CXX_STANDARD_REQUIRED ON)
set_target_properties(cgeom PROPERTIES LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/lib)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
