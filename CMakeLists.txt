################################################################################
# General Informations
################################################################################

cmake_minimum_required(VERSION 3.0.0)
project(cgeom VERSION 0.1.0)

################################################################################

set(THIRD_PARTY_DIR ${CMAKE_CURRENT_SOURCE_DIR}/3rdparty)
list(APPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)


add_subdirectory(${THIRD_PARTY_DIR}/libigl)

################################################################################
# Library
################################################################################
add_library(cgeom cgeom.cc)
set_target_properties(cgeom PROPERTIES CXX_STANDARD 14)
set_target_properties(cgeom PROPERTIES CXX_STANDARD_REQUIRED ON)
target_include_directories(cgeom SYSTEM PUBLIC .)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
