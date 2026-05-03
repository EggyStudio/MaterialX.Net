using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// Polymorphic wrapper over any MaterialX <c>Element</c> (Document, Node,
/// NodeGraph, Input, Output, ...). Use the <c>As*</c> methods to attempt a
/// downcast to a more specific kind.
/// </summary>
public class Element : IDisposable
{
    internal readonly ElementHandle Handle;

    internal Element(ElementHandle handle) { Handle = handle; }

    /// <summary>The element's local name.</summary>
    public string Name => MaterialXNative.mx_element_get_name(Handle.Raw);

    /// <summary>The MaterialX category (e.g. <c>"node"</c>, <c>"nodegraph"</c>, <c>"standard_surface"</c>).</summary>
    public string Category => MaterialXNative.mx_element_get_category(Handle.Raw);

    /// <summary>The element namespace prefix, or empty string.</summary>
    public string Namespace => MaterialXNative.mx_element_get_namespace(Handle.Raw);

    /// <summary>Try-cast to <see cref="Node"/>; returns <c>null</c> if not a Node.</summary>
    public Node? AsNode()
    {
        var raw = MaterialXNative.mx_element_as_node(Handle.Raw);
        if (raw == IntPtr.Zero) return null;
        var h = new NodeHandle();
        h.SetRaw(raw);
        return new Node(h);
    }

    /// <summary>Try-cast to <see cref="NodeGraph"/>; returns <c>null</c> if not a NodeGraph.</summary>
    public NodeGraph? AsNodeGraph()
    {
        var raw = MaterialXNative.mx_element_as_nodegraph(Handle.Raw);
        if (raw == IntPtr.Zero) return null;
        var h = new NodeGraphHandle();
        h.SetRaw(raw);
        return new NodeGraph(h);
    }

    /// <summary>Try-cast to <see cref="Input"/>; returns <c>null</c> if not an Input.</summary>
    public Input? AsInput()
    {
        var raw = MaterialXNative.mx_element_as_input(Handle.Raw);
        if (raw == IntPtr.Zero) return null;
        var h = new InputHandle();
        h.SetRaw(raw);
        return new Input(h);
    }

    public void Dispose() => Handle.Dispose();
}

