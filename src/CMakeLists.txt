add_library(cgeom SHARED discrete_quantities.h discrete_quantities.cc processing.h processing.cc bff.h bff.cc instant_meshes_wrapper.h instant_meshes_wrapper.cc)
# instant_meshes_wrapper.cc includes TBB headers that use std::binary_function
# (removed in C++17).  Override to C++14 for this single file only.
set_source_files_properties(instant_meshes_wrapper.cc PROPERTIES CXX_STANDARD 14 CXX_STANDARD_REQUIRED ON)
target_link_libraries(cgeom igl::core igl_copyleft::comiso OpenMeshCored OpenMeshToolsd QEx bff geometry-central ${GMP_LIBRARY} ${GMPXX_LIBRARY} instant_meshes_lib)
target_include_directories(cgeom PUBLIC ${BFF_INCLUDE_DIRS} ${CMAKE_SOURCE_DIR}/dependencies/libdirectional/include ${CMAKE_SOURCE_DIR}/dependencies/instant-meshes/src)
target_compile_definitions(cgeom PRIVATE USE_GMP_ENABLED)
target_compile_definitions(cgeom PRIVATE
    $<$<PLATFORM_ID:Windows>:CGEOM_PARAM_DLL>
    $<$<PLATFORM_ID:Windows>:CGEOM_PARAM_DLL_EXPORT>
    $<$<PLATFORM_ID:Windows>:CGEOM_BFF_DLL>
    $<$<PLATFORM_ID:Windows>:CGEOM_BFFDLL_EXPORT>
    $<$<PLATFORM_ID:Windows>:CGEOM_QUANT_DLL>
    $<$<PLATFORM_ID:Windows>:CGEOM_QUANT_DLL_EXPORT>
)
target_compile_options(cgeom PRIVATE
    $<$<CXX_COMPILER_ID:Clang,AppleClang,GNU>:-Wno-register>
)
set_target_properties(cgeom PROPERTIES CXX_STANDARD 20)
set_target_properties(cgeom PROPERTIES CXX_STANDARD_REQUIRED ON)
set_target_properties(cgeom PROPERTIES LIBRARY_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin/libcgeom)
set_target_properties(cgeom PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin/libcgeom)
