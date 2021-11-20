#----------------------------------------------------------------
# Generated CMake target import file for configuration "Debug".
#----------------------------------------------------------------

# Commands may need to know the format version.
set(CMAKE_IMPORT_FILE_VERSION 1)

# Import target "tasking" for configuration "Debug"
set_property(TARGET tasking APPEND PROPERTY IMPORTED_CONFIGURATIONS DEBUG)
set_target_properties(tasking PROPERTIES
  IMPORTED_LINK_INTERFACE_LANGUAGES_DEBUG "CXX"
  IMPORTED_LOCATION_DEBUG "${_IMPORT_PREFIX}/lib/libtasking.a"
  )

list(APPEND _IMPORT_CHECK_TARGETS tasking )
list(APPEND _IMPORT_CHECK_FILES_FOR_tasking "${_IMPORT_PREFIX}/lib/libtasking.a" )

# Commands beyond this point should not need to know the version.
set(CMAKE_IMPORT_FILE_VERSION)
