#!/usr/bin/env bash
# Fetches an upstream MaterialX prebuilt SDK release into runtimes/<name>/.
#
# Usage:
#   scripts/fetch-materialx-sdk.sh <sdk-name> [version]
#
# Examples:
#   scripts/fetch-materialx-sdk.sh MaterialX_Linux_GCC_14_Python313
#   scripts/fetch-materialx-sdk.sh MaterialX_Windows_VS2022_x64_Python313 1.39.4

set -euo pipefail

SDK_NAME="${1:?usage: $0 <sdk-name> [version]}"
VERSION="${2:-1.39.4}"
REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEST="$REPO_ROOT/runtimes/$SDK_NAME"

if [[ -d "$DEST" && -d "$DEST/lib" ]]; then
    echo "$SDK_NAME already present at $DEST"
    exit 0
fi

URL="https://github.com/AcademySoftwareFoundation/MaterialX/releases/download/v${VERSION}/${SDK_NAME}.zip"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

echo "Downloading $URL"
curl -fL --retry 3 -o "$TMP/sdk.zip" "$URL"

mkdir -p "$DEST"
unzip -q "$TMP/sdk.zip" -d "$TMP/extract"
# Some release zips wrap contents in a top-level folder; flatten if so.
INNER_COUNT="$(ls "$TMP/extract" | wc -l)"
INNER="$(ls "$TMP/extract" | head -1)"
if [[ "$INNER_COUNT" == "1" && -d "$TMP/extract/$INNER" ]]; then
    cp -R "$TMP/extract/$INNER/." "$DEST/"
else
    cp -R "$TMP/extract/." "$DEST/"
fi

echo "Extracted to $DEST"

