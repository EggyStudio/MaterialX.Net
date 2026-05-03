using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// A MaterialX <c>Node</c> - an instance of a node definition with typed inputs
/// and a single primary output type. Wraps a native <c>mx_node_t</c>.
/// </summary>
public sealed class Node : IDisposable
{
    internal readonly NodeHandle Handle;

    internal Node(NodeHandle handle) { Handle = handle; }

    public string Name     => MaterialXNative.mx_node_get_name(Handle.Raw);
    public string Category => MaterialXNative.mx_node_get_category(Handle.Raw);
    public string Type     => MaterialXNative.mx_node_get_type(Handle.Raw);

    /// <summary>
    /// Returns the existing <c>Input</c> with the given name, or creates a new
    /// one with the supplied <paramref name="type"/> if it doesn't exist.
    /// </summary>
    public Input GetOrAddInput(string name, string type)
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_node_get_or_add_input",
            MaterialXNative.mx_node_get_or_add_input(Handle.Raw, name, type));
        var h = new InputHandle();
        h.SetRaw(raw);
        return new Input(h);
    }

    public void Dispose() => Handle.Dispose();
}

