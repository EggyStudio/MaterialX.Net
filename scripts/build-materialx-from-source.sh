#!/usr/bin/env bash
# Builds the upstream MaterialX C++ project from source and stages the result
# into a fake "SDK" layout (runtimes/MaterialX_SourceBuild_<rid>/{include,lib})
# that mirrors the prebuilt release zips, so native/MaterialXC/CMakeLists.txt
# can consume it via -DMATERIALX_SDK_DIR=... unchanged.
#
# This is used for RIDs where AcademySoftwareFoundation does NOT publish a
# matching prebuilt SDK - currently only osx-x64 (the upstream
# MaterialX_MacOS_Xcode_16_Python313 release ships arm64-only archives).
#
# Usage:
#   scripts/build-materialx-from-source.sh <rid> [version] [cmake-osx-arch]
#
# Examples:
#   scripts/build-materialx-from-source.sh osx-x64 1.39.4 x86_64

set -euo pipefail

RID="${1:?usage: $0 <rid> [version] [cmake-osx-arch]}"
VERSION="${2:-1.39.4}"
OSX_ARCH="${3:-}"

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEST="$REPO_ROOT/runtimes/MaterialX_SourceBuild_${RID}"

if [[ -d "$DEST/lib" && -d "$DEST/include" ]]; then
    echo "MaterialX source build already staged at $DEST"
    exit 0
fi

TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

echo "Cloning MaterialX v${VERSION} source"
git clone --depth 1 --branch "v${VERSION}" \
    https://github.com/AcademySoftwareFoundation/MaterialX.git "$TMP/MaterialX"

CMAKE_ARGS=(
    -S "$TMP/MaterialX"
    -B "$TMP/build"
    -DCMAKE_BUILD_TYPE=Release
    -DCMAKE_INSTALL_PREFIX="$DEST"
    -DMATERIALX_BUILD_SHARED_LIBS=OFF
    -DMATERIALX_BUILD_PYTHON=OFF
    -DMATERIALX_BUILD_TESTS=OFF
    -DMATERIALX_BUILD_VIEWER=OFF
    -DMATERIALX_BUILD_GRAPH_EDITOR=OFF
    -DMATERIALX_BUILD_RENDER=OFF
    -DMATERIALX_BUILD_OIIO=OFF
    -DMATERIALX_INSTALL_PYTHON=OFF
    -DCMAKE_POSITION_INDEPENDENT_CODE=ON
)

if [[ -n "$OSX_ARCH" ]]; then
    CMAKE_ARGS+=(-DCMAKE_OSX_ARCHITECTURES="$OSX_ARCH")
fi

cmake "${CMAKE_ARGS[@]}"
cmake --build "$TMP/build" --config Release -j
cmake --install "$TMP/build" --config Release

# Upstream installs into <prefix>/{include,libraries,...} and on some platforms
# the static archives land under lib64; normalize to lib/ to match the prebuilt
# release zips that native/MaterialXC/CMakeLists.txt expects.
if [[ -d "$DEST/lib64" && ! -d "$DEST/lib" ]]; then
    mv "$DEST/lib64" "$DEST/lib"
fi

echo "MaterialX source build staged at $DEST"

