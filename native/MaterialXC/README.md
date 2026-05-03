# MaterialXC
Native C ABI shim over MaterialX. Built as a shared library
(`libMaterialXC.so` / `MaterialXC.dll` / `libMaterialXC.dylib`) that statically
links the upstream MaterialX archives. The resulting binary is the only
native artifact P/Invoked by `MaterialX.Net`.
The upstream MaterialX SDK is not committed; it is fetched from the official
GitHub release on demand by `scripts/fetch-materialx-sdk.sh`. All shell
commands below are run from this folder (`<repo>/native/MaterialXC`).
## linux-x64 (build on Linux)
```sh
../../scripts/fetch-materialx-sdk.sh MaterialX_Linux_GCC_14_Python313
cmake -S . -B build/linux-x64 \
  -DMATERIALX_SDK_DIR="$PWD/../../runtimes/MaterialX_Linux_GCC_14_Python313" \
  -DCMAKE_BUILD_TYPE=Release
cmake --build build/linux-x64 --config Release -j
cp build/linux-x64/libMaterialXC.so ../../runtimes/linux-x64/native/
```
Toolchain: `gcc-c++` (or `clang`), CMake >= 3.20, `make`, `curl`, `tar`.
## All three RIDs via GitHub Actions (recommended for win-x64 and osx-x64)
`.github/workflows/build-natives.yml` builds each RID on its native runner
(`ubuntu-latest`, `windows-latest`, `macos-latest`), fetches the matching
MaterialX SDK release, and uploads each binary as a workflow artifact.
Trigger the workflow:
```sh
git push                      # auto-runs on changes under native/MaterialXC/**
# or, manually, from the repository's Actions tab via "Run workflow"
```
Download the produced binaries into the package layout:
```sh
gh run download --name MaterialXC-linux-x64 --dir runtimes/linux-x64/native
gh run download --name MaterialXC-win-x64   --dir runtimes/win-x64/native
gh run download --name MaterialXC-osx-x64   --dir runtimes/osx-x64/native
```
(`gh` is the GitHub CLI: `sudo dnf install -y gh && gh auth login`.)
## Integration
After a binary lands in `runtimes/<rid>/native/`, `dotnet build` and
`dotnet pack` against `MaterialX.Net.csproj` pick it up automatically. RIDs
without a binary are skipped by the csproj's `Condition="Exists(...)"` guards.
## Surface
The shim exposes `extern "C"` opaque-handle functions for Document creation,
XML read/write, `loadLibraries`, Element traversal, Node and NodeGraph
creation, Input value setters, and node connections. The full surface lives
in [`include/MaterialXC.h`](include/MaterialXC.h). To extend the binding, add
a function there and in [`src/MaterialXC.cpp`](src/MaterialXC.cpp), then
mirror it in [`src/Native/MaterialXNative.cs`](../../src/Native/MaterialXNative.cs).
