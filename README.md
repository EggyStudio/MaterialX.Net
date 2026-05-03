# MaterialX.Net

A single-package .NET binding for [MaterialX](https://materialx.org/) - the open
standard for representing rich material and look-development content in
computer graphics. Targets **.NET 10** and uses `LibraryImport` source-generated
P/Invoke.

The NuGet ships:

- `MaterialX.Net.dll` - managed wrapper API (`MaterialX.Document`, `Node`,
  `NodeGraph`, `Input`, value types, ...).
- `runtimes/<rid>/native/MaterialXC.{dll,so,dylib}` - a tiny `extern "C"`
  shared library that statically links the upstream MaterialX archives. This
  is the only native artifact you need to ship; MaterialX itself is C++ and
  cannot be P/Invoked directly.
- `runtimes/any/native/libraries/` - the MaterialX standard data libraries
  (`stdlib`, `pbrlib`, `bxdf`, `lights`, `cmlib`, `nprlib`, `mdl`, `targets`).
  The bundled MSBuild targets stage them next to your assembly as `libraries/`
  at build/publish time, where `Document.LoadStandardLibraries()` picks them up
  automatically.

## Quick start

```csharp
using MaterialX;

using var doc = Document.Create();
doc.LoadStandardLibraries(); // resolves the bundled libraries/ folder

using var surface = doc.AddNode("standard_surface", "shader1", "surfaceshader");
using var baseColor = surface.GetOrAddInput("base_color", "color3");
baseColor.SetValue(new Color3(0.8f, 0.2f, 0.1f));

doc.WriteToXmlFile("out.mtlx");
```

## Building the native shim

The package ships pre-built shims for `win-x64`, `linux-x64`, and `osx-x64`. To
rebuild them from source against the upstream MaterialX SDKs (checked in under
`runtimes/MaterialX_*_Python313/`), see [`native/MaterialXC/README.md`](native/MaterialXC/README.md).

## Why a C-shim?

MaterialX is a C++ API (templates, `std::shared_ptr`, namespaces). The upstream
distribution ships only static archives (`.a` / `.lib`) plus pybind11 Python
modules - nothing that can be `LibraryImport`'d directly from .NET. The
`MaterialXC` shim under `native/MaterialXC/` exposes the subset of MaterialX
needed by the managed API as a flat `extern "C"` surface using opaque handles
(`mx_document_t`, `mx_node_t`, ...). Extending the binding = adding a function
to `MaterialXC.h/.cpp` and mirroring it in `src/Native/MaterialXNative.cs`.

## License

`MaterialX.Net` is licensed under [Apache-2.0](LICENSE), matching upstream
MaterialX. The bundled native binaries and data libraries are © Academy
Software Foundation / contributors and redistributed under the same license.

