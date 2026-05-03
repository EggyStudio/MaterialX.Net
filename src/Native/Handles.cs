using System.Runtime.InteropServices;
using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// SafeHandle base for opaque MaterialXC handles. Each subclass binds the
/// handle kind to its matching <c>mx_*_release</c> function.
/// </summary>
public abstract class MaterialXHandle : SafeHandle
{
    protected MaterialXHandle() : base(IntPtr.Zero, ownsHandle: true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;
    internal IntPtr Raw => handle;

    internal void SetRaw(IntPtr value) => SetHandle(value);
}

public sealed class DocumentHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_document_release(handle); return true; }
}

public sealed class ElementHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_element_release(handle); return true; }
}

public sealed class NodeHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_node_release(handle); return true; }
}

public sealed class NodeGraphHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_nodegraph_release(handle); return true; }
}

public sealed class InputHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_input_release(handle); return true; }
}

public sealed class OutputHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_output_release(handle); return true; }
}

public sealed class ShaderGeneratorHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_shadergen_release(handle); return true; }
}

public sealed class GenContextHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_gencontext_release(handle); return true; }
}

public sealed class ShaderHandle : MaterialXHandle
{
    protected override bool ReleaseHandle() { MaterialXNative.mx_shader_release(handle); return true; }
}

