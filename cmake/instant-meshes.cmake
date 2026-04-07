if(TARGET instant_meshes_lib)
    return()
endif()

set(IM_DIR ${CMAKE_CURRENT_SOURCE_DIR}/dependencies/instant-meshes)

# Build bundled TBB as a static library
set(TBB_BUILD_SHARED          OFF CACHE BOOL " " FORCE)
set(TBB_BUILD_STATIC          ON  CACHE BOOL " " FORCE)
set(TBB_BUILD_TBBMALLOC       OFF CACHE BOOL " " FORCE)
set(TBB_BUILD_TBBMALLOC_PROXY OFF CACHE BOOL " " FORCE)
set(TBB_BUILD_TESTS           OFF CACHE BOOL " " FORCE)
add_subdirectory(${IM_DIR}/ext/tbb ext_build/im_tbb)

# Instant Meshes non-GUI sources only (no viewer/widgets/glutil/main)
add_library(instant_meshes_lib STATIC
    ${IM_DIR}/src/batch.cpp
    ${IM_DIR}/src/meshio.cpp
    ${IM_DIR}/src/normal.cpp
    ${IM_DIR}/src/adjacency.cpp
    ${IM_DIR}/src/meshstats.cpp
    ${IM_DIR}/src/hierarchy.cpp
    ${IM_DIR}/src/extract.cpp
    ${IM_DIR}/src/field.cpp
    ${IM_DIR}/src/bvh.cpp
    ${IM_DIR}/src/subdivide.cpp
    ${IM_DIR}/src/reorder.cpp
    ${IM_DIR}/src/serializer.cpp
    ${IM_DIR}/src/smoothcurve.cpp
    ${IM_DIR}/src/cleanup.cpp
    ${IM_DIR}/src/dedge.cpp
    ${IM_DIR}/ext/rply/rply.c
)

target_include_directories(instant_meshes_lib PUBLIC
    ${IM_DIR}/src
    ${IM_DIR}/ext/tbb/include
    ${IM_DIR}/ext/half
    ${IM_DIR}/ext/pcg32
    ${IM_DIR}/ext/dset
    ${IM_DIR}/ext/pss
    ${IM_DIR}/ext/rply
    ${CMAKE_CURRENT_SOURCE_DIR}/build/_deps/eigen-src  # Eigen from libigl fetch
)

target_link_libraries(instant_meshes_lib tbb_static)
target_compile_definitions(instant_meshes_lib PRIVATE SINGLE_PRECISION)
# C++14: bundled TBB uses std::binary_function which was removed in C++17
set_target_properties(instant_meshes_lib PROPERTIES CXX_STANDARD 14 CXX_STANDARD_REQUIRED ON)
set_target_properties(tbb_static PROPERTIES CXX_STANDARD 14 CXX_STANDARD_REQUIRED ON)
# Suppress warnings from instant-meshes sources
target_compile_options(instant_meshes_lib PRIVATE
    $<$<CXX_COMPILER_ID:Clang,AppleClang,GNU>:-Wno-unused-parameter>
    $<$<CXX_COMPILER_ID:Clang,AppleClang,GNU>:-Wno-sign-compare>
    $<$<CXX_COMPILER_ID:Clang,AppleClang,GNU>:-Wno-deprecated-declarations>)
