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

Build a material and write it to disk:

```csharp
using MaterialX;

using var doc = Document.Create();
doc.LoadStandardLibraries(); // resolves the bundled libraries/ folder

using var surface = doc.AddNode("standard_surface", "shader1", "surfaceshader");
using var baseColor = surface.GetOrAddInput("base_color", "color3");
baseColor.SetValue(new Color3(0.8f, 0.2f, 0.1f));

doc.WriteToXmlFile("out.mtlx");
```

### Generating Vulkan GLSL (or desktop GLSL / ESSL / WGSL)

Once a document has at least one renderable element (a `surfacematerial`,
`standard_surface`, etc.), you can generate real shader source straight from
the managed API:

```csharp
using MaterialX;
using System.Linq;

const string Mtlx = """
<?xml version="1.0"?>
<materialx version="1.39">
  <standard_surface name="SR1" type="surfaceshader">
    <input name="base_color" type="color3" value="0.8, 0.2, 0.2" />
  </standard_surface>
  <surfacematerial name="M1" type="material">
    <input name="surfaceshader" type="surfaceshader" nodename="SR1" />
  </surfacematerial>
</materialx>
""";

using var doc = Document.ReadFromXmlString(Mtlx);
doc.LoadStandardLibraries();

// Pick a backend: Vulkan | Glsl400 | Essl300 | Wgsl
using var gen = ShaderGenerator.Create(ShaderTarget.Vulkan);
using var ctx = GenContext.Create(gen);
ctx.AddStandardLibrarySearchPath(); // resolves "libraries/..." includes

var renderable = doc.Renderables.First();
using var shader = gen.Generate(renderable, ctx, "MyShader");

string vertexSrc = shader.GetSourceCode(ShaderStage.Vertex);
string pixelSrc  = shader.GetSourceCode(ShaderStage.Pixel);

Console.WriteLine($"target={gen.Target} vertex={vertexSrc.Length} pixel={pixelSrc.Length}");
```

The pixel stage is the full MaterialX `standard_surface` implementation as
Vulkan-flavored GLSL (`#version 450` + `layout(set=, binding=)` decorations on
every UBO and sampler) - i.e. source you can feed straight into
`glslangValidator -V` / `shaderc` to produce SPIR-V for `vkCreateShaderModule`.
Switching `ShaderTarget` swaps backends with no other changes required:
`Glsl400` for desktop OpenGL, `Essl300` for GLES 3 / WebGL 2, `Wgsl` for
WebGPU.

## Building the native shim

The package ships pre-built shims for `win-x64`, `linux-x64`, and `osx-arm64`
(Apple Silicon; Intel Macs are not supported because the upstream MaterialX
SDK we statically link against ships arm64-only archives). To
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

