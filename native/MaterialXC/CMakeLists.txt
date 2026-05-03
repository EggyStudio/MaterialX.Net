# MaterialXC - C ABI shim over MaterialX for MaterialX.Net
#
# Builds a SHARED library (libMaterialXC.so / MaterialXC.dll / libMaterialXC.dylib)
# that statically links the upstream MaterialX archives shipped under
# ../../runtimes/MaterialX_*_Python313/lib/. The resulting binary is self-contained
# and is the only native artifact P/Invoked by MaterialX.Net.
#
# Usage:
#   cmake -S . -B build -DMATERIALX_SDK_DIR=/abs/path/to/runtimes/MaterialX_Linux_GCC_14_Python313 -DCMAKE_BUILD_TYPE=Release
#   cmake --build build --config Release -j
#   # then drop the produced shared lib into ../../runtimes/<rid>/native/
#
# On Windows pass MATERIALX_SDK_DIR pointing at MaterialX_Windows_VS2022_x64_Python313,
# on macOS at MaterialX_MacOS_Xcode_16_Python313.
cmake_minimum_required(VERSION 3.20)
project(MaterialXC CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_VISIBILITY_PRESET hidden)
set(CMAKE_VISIBILITY_INLINES_HIDDEN ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

if(NOT MATERIALX_SDK_DIR)
    message(FATAL_ERROR "Pass -DMATERIALX_SDK_DIR=<path to runtimes/MaterialX_*_Python313>")
endif()

set(MX_INC "${MATERIALX_SDK_DIR}/include")
set(MX_LIB "${MATERIALX_SDK_DIR}/lib")

# Pick up MaterialX's own CMake config so transitive include / link flags
# (e.g. nanogui, OpenImageIO on render libs) are respected if needed later.
list(APPEND CMAKE_PREFIX_PATH "${MX_LIB}/cmake/MaterialX")
find_package(MaterialX QUIET CONFIG)

add_library(MaterialXC SHARED
    src/MaterialXC.cpp
)

target_include_directories(MaterialXC
    PRIVATE
        include
        ${MX_INC}
)

target_compile_definitions(MaterialXC PRIVATE MATERIALXC_BUILDING)

# Link the static MaterialX core + format archives. Name pattern differs across
# platforms (lib*.a vs *.lib); CMake's find_library handles both.
foreach(_mxlib MaterialXFormat MaterialXCore)
    find_library(MX_${_mxlib}_LIB
        NAMES ${_mxlib}
        PATHS ${MX_LIB}
        NO_DEFAULT_PATH
        REQUIRED)
    target_link_libraries(MaterialXC PRIVATE ${MX_${_mxlib}_LIB})
endforeach()

# Output naming: libMaterialXC.so / MaterialXC.dll / libMaterialXC.dylib
set_target_properties(MaterialXC PROPERTIES
    OUTPUT_NAME MaterialXC
    PREFIX "${CMAKE_SHARED_LIBRARY_PREFIX}"
    SUFFIX "${CMAKE_SHARED_LIBRARY_SUFFIX}"
)

if(APPLE)
    set_target_properties(MaterialXC PROPERTIES
        INSTALL_RPATH "@loader_path"
        BUILD_WITH_INSTALL_RPATH ON)
elseif(UNIX)
    set_target_properties(MaterialXC PROPERTIES
        INSTALL_RPATH "$ORIGIN"
        BUILD_WITH_INSTALL_RPATH ON)
endif()

install(TARGETS MaterialXC
    RUNTIME DESTINATION bin
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib)

