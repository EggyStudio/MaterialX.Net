using System.Runtime.InteropServices;

namespace MaterialX.Native;

/// <summary>
/// P/Invoke surface for the <c>MaterialXC</c> native shim. Internal; the
/// public API in <see cref="MaterialX"/> is built on top of these calls.
/// </summary>
internal static partial class MaterialXNative
{
    internal const string LibName = "MaterialXC";

    // ------------- diagnostics -------------

    [LibraryImport(LibName, EntryPoint = "mx_last_error", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_last_error();

    [LibraryImport(LibName, EntryPoint = "mx_version", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_version();

    // ------------- release -------------

    [LibraryImport(LibName, EntryPoint = "mx_document_release")]   internal static partial void mx_document_release(IntPtr h);
    [LibraryImport(LibName, EntryPoint = "mx_element_release")]    internal static partial void mx_element_release(IntPtr h);
    [LibraryImport(LibName, EntryPoint = "mx_node_release")]       internal static partial void mx_node_release(IntPtr h);
    [LibraryImport(LibName, EntryPoint = "mx_nodegraph_release")]  internal static partial void mx_nodegraph_release(IntPtr h);
    [LibraryImport(LibName, EntryPoint = "mx_input_release")]      internal static partial void mx_input_release(IntPtr h);
    [LibraryImport(LibName, EntryPoint = "mx_output_release")]     internal static partial void mx_output_release(IntPtr h);

    // ------------- Document -------------

    [LibraryImport(LibName, EntryPoint = "mx_document_create")]
    internal static partial IntPtr mx_document_create();

    [LibraryImport(LibName, EntryPoint = "mx_document_load_libraries", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_document_load_libraries(IntPtr doc, string? librariesRoot);

    [LibraryImport(LibName, EntryPoint = "mx_document_read_from_xml_file", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_document_read_from_xml_file(IntPtr doc, string path, string? searchPath);

    [LibraryImport(LibName, EntryPoint = "mx_document_read_from_xml_string", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_document_read_from_xml_string(IntPtr doc, string xml, string? searchPath);

    [LibraryImport(LibName, EntryPoint = "mx_document_write_to_xml_file", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_document_write_to_xml_file(IntPtr doc, string path);

    [LibraryImport(LibName, EntryPoint = "mx_document_write_to_xml_string", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_document_write_to_xml_string(IntPtr doc);

    [LibraryImport(LibName, EntryPoint = "mx_document_validate", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int mx_document_validate(IntPtr doc, out IntPtr outMessage);

    // ------------- Element -------------

    [LibraryImport(LibName, EntryPoint = "mx_element_child_count")]
    internal static partial int mx_element_child_count(IntPtr parent);

    [LibraryImport(LibName, EntryPoint = "mx_element_child_at")]
    internal static partial IntPtr mx_element_child_at(IntPtr parent, int index);

    [LibraryImport(LibName, EntryPoint = "mx_element_get_name", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_element_get_name(IntPtr e);

    [LibraryImport(LibName, EntryPoint = "mx_element_get_category", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_element_get_category(IntPtr e);

    [LibraryImport(LibName, EntryPoint = "mx_element_get_namespace", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_element_get_namespace(IntPtr e);

    [LibraryImport(LibName, EntryPoint = "mx_element_as_node")]      internal static partial IntPtr mx_element_as_node(IntPtr e);
    [LibraryImport(LibName, EntryPoint = "mx_element_as_nodegraph")] internal static partial IntPtr mx_element_as_nodegraph(IntPtr e);
    [LibraryImport(LibName, EntryPoint = "mx_element_as_input")]     internal static partial IntPtr mx_element_as_input(IntPtr e);
    [LibraryImport(LibName, EntryPoint = "mx_element_as_output")]    internal static partial IntPtr mx_element_as_output(IntPtr e);

    // ------------- Node / NodeGraph -------------

    [LibraryImport(LibName, EntryPoint = "mx_document_add_node", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_document_add_node(IntPtr doc, string category, string? name, string? type);

    [LibraryImport(LibName, EntryPoint = "mx_document_add_nodegraph", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_document_add_nodegraph(IntPtr doc, string? name);

    [LibraryImport(LibName, EntryPoint = "mx_nodegraph_add_node", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_nodegraph_add_node(IntPtr g, string category, string? name, string? type);

    [LibraryImport(LibName, EntryPoint = "mx_node_get_name", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_node_get_name(IntPtr n);

    [LibraryImport(LibName, EntryPoint = "mx_node_get_category", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_node_get_category(IntPtr n);

    [LibraryImport(LibName, EntryPoint = "mx_node_get_type", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial string mx_node_get_type(IntPtr n);

    [LibraryImport(LibName, EntryPoint = "mx_node_get_or_add_input", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_node_get_or_add_input(IntPtr n, string name, string? type);

    // ------------- Input setters -------------

    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_int")]    internal static partial MxStatus mx_input_set_value_int(IntPtr i, int v);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_float")]  internal static partial MxStatus mx_input_set_value_float(IntPtr i, float v);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_bool")]   internal static partial MxStatus mx_input_set_value_bool(IntPtr i, int v);

    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_string", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_input_set_value_string(IntPtr i, string v);

    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_color3")]  internal static partial MxStatus mx_input_set_value_color3(IntPtr i, float r, float g, float b);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_color4")]  internal static partial MxStatus mx_input_set_value_color4(IntPtr i, float r, float g, float b, float a);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_vector2")] internal static partial MxStatus mx_input_set_value_vector2(IntPtr i, float x, float y);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_vector3")] internal static partial MxStatus mx_input_set_value_vector3(IntPtr i, float x, float y, float z);
    [LibraryImport(LibName, EntryPoint = "mx_input_set_value_vector4")] internal static partial MxStatus mx_input_set_value_vector4(IntPtr i, float x, float y, float z, float w);

    [LibraryImport(LibName, EntryPoint = "mx_input_connect_to_node", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_input_connect_to_node(IntPtr i, string? nodeName, string? outputName);
}

/// <summary>Mirrors the <c>mx_status</c> enum in MaterialXC.h.</summary>
internal enum MxStatus
{
    Ok = 0,
    InvalidArg = 1,
    Io = 2,
    Parse = 3,
    NotFound = 4,
    Internal = 5,
}

