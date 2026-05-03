using MaterialX.Native;

namespace MaterialX;

/// <summary>Code-generation backends supported by the bundled MaterialXC shim.</summary>
public enum ShaderTarget
{
    /// <summary>Desktop OpenGL / Vulkan-style GLSL.</summary>
    Glsl400 = 0,
    /// <summary>OpenGL ES 3 / WebGL 2 (ESSL 3.0).</summary>
    Essl300 = 1,
    /// <summary>SPIR-V-friendly GLSL with Vulkan binding decorations.</summary>
    Vulkan = 2,
    /// <summary>WebGPU shading language.</summary>
    Wgsl = 3,
}

/// <summary>
/// Standard MaterialX shader stage names. Pass to
/// <see cref="Shader.GetSourceCode(string)"/> to retrieve generated source.
/// </summary>
public static class ShaderStage
{
    public const string Vertex = "vertex";
    public const string Pixel  = "pixel";
}

/// <summary>
/// A MaterialX shader code generator targeting one of the GLSL-family backends.
/// Pair with a <see cref="GenContext"/> and call <see cref="Generate"/> on a
/// renderable <see cref="Element"/> (typically obtained via
/// <see cref="Document.Renderables"/>).
/// </summary>
public sealed class ShaderGenerator : IDisposable
{
    internal readonly ShaderGeneratorHandle Handle;

    private ShaderGenerator(ShaderGeneratorHandle h) { Handle = h; }

    /// <summary>Creates a generator for the requested target.</summary>
    public static ShaderGenerator Create(ShaderTarget target)
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_shadergen_create",
            MaterialXNative.mx_shadergen_create((int)target));
        var h = new ShaderGeneratorHandle();
        h.SetRaw(raw);
        return new ShaderGenerator(h);
    }

    /// <summary>The generator's MaterialX target identifier (e.g. "genglsl").</summary>
    public string Target => MaterialXNative.mx_shadergen_target_name(Handle.Raw);

    /// <summary>Generates a <see cref="Shader"/> from a renderable element.</summary>
    public Shader Generate(Element element, GenContext context, string? name = null)
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_shadergen_generate",
            MaterialXNative.mx_shadergen_generate(Handle.Raw, name, element.Handle.Raw, context.Handle.Raw));
        var h = new ShaderHandle();
        h.SetRaw(raw);
        return new Shader(h);
    }

    public void Dispose() => Handle.Dispose();
}

/// <summary>
/// Generation context (search paths, options) consumed by
/// <see cref="ShaderGenerator.Generate"/>.
/// </summary>
public sealed class GenContext : IDisposable
{
    internal readonly GenContextHandle Handle;

    private GenContext(GenContextHandle h) { Handle = h; }

    /// <summary>Creates a context bound to the given generator.</summary>
    public static GenContext Create(ShaderGenerator generator)
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_gencontext_create",
            MaterialXNative.mx_gencontext_create(generator.Handle.Raw));
        var h = new GenContextHandle();
        h.SetRaw(raw);
        return new GenContext(h);
    }

    /// <summary>
    /// Adds a search path used to locate per-target shader source includes.
    /// MaterialX's bundled GLSL/MSL/etc. sources reference includes qualified
    /// with a <c>libraries/</c> prefix (e.g. <c>libraries/stdlib/genglsl/lib/mx_math.glsl</c>),
    /// so the path you register must be the <i>parent</i> of the
    /// <c>libraries</c> folder.
    /// </summary>
    public void AddSourceSearchPath(string path)
        => MaterialXException.ThrowIfError(
            "mx_gencontext_add_source_search_path",
            MaterialXNative.mx_gencontext_add_source_search_path(Handle.Raw, path));

    /// <summary>
    /// Convenience: register the source-code search path for the bundled
    /// MaterialX standard libraries (resolved via
    /// <see cref="LibrarySearch.GetDefaultLibrariesPath"/>). Equivalent to
    /// <see cref="AddSourceSearchPath"/> with that folder's parent directory.
    /// </summary>
    public void AddStandardLibrarySearchPath()
    {
        var libs = LibrarySearch.GetDefaultLibrariesPath()
            ?? throw new MaterialXException("AddStandardLibrarySearchPath: bundled MaterialX libraries/ folder not found");
        var parent = Path.GetDirectoryName(libs.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            ?? throw new MaterialXException("AddStandardLibrarySearchPath: cannot resolve parent of " + libs);
        AddSourceSearchPath(parent);
    }

    public void Dispose() => Handle.Dispose();
}

/// <summary>
/// A generated shader object, owning the produced source code for one or more
/// stages (vertex, pixel, ...). Query stages via <see cref="StageNames"/> and
/// fetch source with <see cref="GetSourceCode"/>.
/// </summary>
public sealed class Shader : IDisposable
{
    internal readonly ShaderHandle Handle;

    internal Shader(ShaderHandle h) { Handle = h; }

    /// <summary>Number of stages (e.g. 2 for GLSL: vertex + pixel).</summary>
    public int StageCount => MaterialXNative.mx_shader_stage_count(Handle.Raw);

    /// <summary>Names of the stages produced by the generator, in index order.</summary>
    public IEnumerable<string> StageNames
    {
        get
        {
            var n = StageCount;
            for (var i = 0; i < n; i++)
                yield return MaterialXNative.mx_shader_stage_name_at(Handle.Raw, i);
        }
    }

    /// <summary>
    /// Returns the generated source code for the named stage, or an empty
    /// string if the stage does not exist on this shader.
    /// </summary>
    public string GetSourceCode(string stageName)
        => MaterialXNative.mx_shader_get_source_code(Handle.Raw, stageName);

    public void Dispose() => Handle.Dispose();
}

