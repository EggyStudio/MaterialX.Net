using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// A MaterialX <c>NodeGraph</c> - a container of <see cref="Node"/>s with
/// internal connections, exposed via output sockets to its parent document.
/// </summary>
public sealed class NodeGraph : IDisposable
{
    internal readonly NodeGraphHandle Handle;

    internal NodeGraph(NodeGraphHandle handle) { Handle = handle; }

    /// <summary>Adds a node inside this node graph.</summary>
    public Node AddNode(string category, string? name = null, string type = "color3")
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_nodegraph_add_node",
            MaterialXNative.mx_nodegraph_add_node(Handle.Raw, category, name, type));
        var h = new NodeHandle();
        h.SetRaw(raw);
        return new Node(h);
    }

    public void Dispose() => Handle.Dispose();
}

