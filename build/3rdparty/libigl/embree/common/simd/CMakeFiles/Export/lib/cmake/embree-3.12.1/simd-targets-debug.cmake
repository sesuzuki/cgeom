#----------------------------------------------------------------
# Generated CMake target import file for configuration "Debug".
#----------------------------------------------------------------

# Commands may need to know the format version.
set(CMAKE_IMPORT_FILE_VERSION 1)

# Import target "simd" for configuration "Debug"
set_property(TARGET simd APPEND PROPERTY IMPORTED_CONFIGURATIONS DEBUG)
set_target_properties(simd PROPERTIES
  IMPORTED_LINK_INTERFACE_LANGUAGES_DEBUG "CXX"
  IMPORTED_LOCATION_DEBUG "${_IMPORT_PREFIX}/lib/libsimd.a"
  )

list(APPEND _IMPORT_CHECK_TARGETS simd )
list(APPEND _IMPORT_CHECK_FILES_FOR_simd "${_IMPORT_PREFIX}/lib/libsimd.a" )

# Commands beyond this point should not need to know the version.
set(CMAKE_IMPORT_FILE_VERSION)
