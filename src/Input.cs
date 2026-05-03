using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// A MaterialX <c>Input</c> port on a node. Carries either a literal value
/// (set via <c>SetValue</c>) or a connection to another node's output.
/// </summary>
public sealed class Input : IDisposable
{
    internal readonly InputHandle Handle;

    internal Input(InputHandle handle) { Handle = handle; }

    public void SetValue(int value)     => MaterialXException.ThrowIfError("mx_input_set_value_int",    MaterialXNative.mx_input_set_value_int(Handle.Raw, value));
    public void SetValue(float value)   => MaterialXException.ThrowIfError("mx_input_set_value_float",  MaterialXNative.mx_input_set_value_float(Handle.Raw, value));
    public void SetValue(bool value)    => MaterialXException.ThrowIfError("mx_input_set_value_bool",   MaterialXNative.mx_input_set_value_bool(Handle.Raw, value ? 1 : 0));
    public void SetValue(string value)  => MaterialXException.ThrowIfError("mx_input_set_value_string", MaterialXNative.mx_input_set_value_string(Handle.Raw, value));
    public void SetValue(Color3 v)      => MaterialXException.ThrowIfError("mx_input_set_value_color3", MaterialXNative.mx_input_set_value_color3(Handle.Raw, v.R, v.G, v.B));
    public void SetValue(Color4 v)      => MaterialXException.ThrowIfError("mx_input_set_value_color4", MaterialXNative.mx_input_set_value_color4(Handle.Raw, v.R, v.G, v.B, v.A));
    public void SetValue(Vector2 v)     => MaterialXException.ThrowIfError("mx_input_set_value_vector2", MaterialXNative.mx_input_set_value_vector2(Handle.Raw, v.X, v.Y));
    public void SetValue(Vector3 v)     => MaterialXException.ThrowIfError("mx_input_set_value_vector3", MaterialXNative.mx_input_set_value_vector3(Handle.Raw, v.X, v.Y, v.Z));
    public void SetValue(Vector4 v)     => MaterialXException.ThrowIfError("mx_input_set_value_vector4", MaterialXNative.mx_input_set_value_vector4(Handle.Raw, v.X, v.Y, v.Z, v.W));

    /// <summary>
    /// Connects this input to <paramref name="node"/>'s primary output, or to a
    /// named secondary output when <paramref name="outputName"/> is supplied.
    /// </summary>
    public void ConnectTo(Node node, string? outputName = null)
        => MaterialXException.ThrowIfError(
            "mx_input_connect_to_node",
            MaterialXNative.mx_input_connect_to_node(Handle.Raw, node.Name, outputName));

    public void Dispose() => Handle.Dispose();
}

