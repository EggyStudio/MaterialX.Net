using System.Runtime.InteropServices;
namespace MaterialX.Native;
/// <summary>
/// P/Invoke surface for the <c>MaterialXC</c> native shim. Internal; the
/// public API in <see cref="MaterialX"/> is built on top of these calls.
/// </summary>
internal static partial class MaterialXNative
{
    internal const string LibName = "MaterialXC";
    /// <summary>
    /// Helper for native functions returning a UTF-8 string owned by the shim
    /// (a pointer into a thread-local std::string). The marshaller MUST NOT
    /// free this pointer, so all such functions are declared with an IntPtr
    /// return type and copied through this helper.
    /// </summary>
    internal static string ReadUtf8(IntPtr p)
        => p == IntPtr.Zero ? string.Empty : (Marshal.PtrToStringUTF8(p) ?? string.Empty);
    // ------------- diagnostics -------------
    [LibraryImport(LibName, EntryPoint = "mx_last_error")]
    internal static partial IntPtr mx_last_error_ptr();
    internal static string mx_last_error() => ReadUtf8(mx_last_error_ptr());
    [LibraryImport(LibName, EntryPoint = "mx_version")]
    internal static partial IntPtr mx_version_ptr();
    internal static string mx_version() => ReadUtf8(mx_version_ptr());
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
    [LibraryImport(LibName, EntryPoint = "mx_document_write_to_xml_string")]
    internal static partial IntPtr mx_document_write_to_xml_string_ptr(IntPtr doc);
    internal static string mx_document_write_to_xml_string(IntPtr doc) => ReadUtf8(mx_document_write_to_xml_string_ptr(doc));
    [LibraryImport(LibName, EntryPoint = "mx_document_validate")]
    internal static partial int mx_document_validate(IntPtr doc, out IntPtr outMessage);
    // ------------- Element -------------
    [LibraryImport(LibName, EntryPoint = "mx_element_child_count")]
    internal static partial int mx_element_child_count(IntPtr parent);
    [LibraryImport(LibName, EntryPoint = "mx_element_child_at")]
    internal static partial IntPtr mx_element_child_at(IntPtr parent, int index);
    [LibraryImport(LibName, EntryPoint = "mx_element_get_name")]
    internal static partial IntPtr mx_element_get_name_ptr(IntPtr e);
    internal static string mx_element_get_name(IntPtr e) => ReadUtf8(mx_element_get_name_ptr(e));
    [LibraryImport(LibName, EntryPoint = "mx_element_get_category")]
    internal static partial IntPtr mx_element_get_category_ptr(IntPtr e);
    internal static string mx_element_get_category(IntPtr e) => ReadUtf8(mx_element_get_category_ptr(e));
    [LibraryImport(LibName, EntryPoint = "mx_element_get_namespace")]
    internal static partial IntPtr mx_element_get_namespace_ptr(IntPtr e);
    internal static string mx_element_get_namespace(IntPtr e) => ReadUtf8(mx_element_get_namespace_ptr(e));
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
    [LibraryImport(LibName, EntryPoint = "mx_node_get_name")]
    internal static partial IntPtr mx_node_get_name_ptr(IntPtr n);
    internal static string mx_node_get_name(IntPtr n) => ReadUtf8(mx_node_get_name_ptr(n));
    [LibraryImport(LibName, EntryPoint = "mx_node_get_category")]
    internal static partial IntPtr mx_node_get_category_ptr(IntPtr n);
    internal static string mx_node_get_category(IntPtr n) => ReadUtf8(mx_node_get_category_ptr(n));
    [LibraryImport(LibName, EntryPoint = "mx_node_get_type")]
    internal static partial IntPtr mx_node_get_type_ptr(IntPtr n);
    internal static string mx_node_get_type(IntPtr n) => ReadUtf8(mx_node_get_type_ptr(n));
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
    // ------------- Renderable discovery -------------
    [LibraryImport(LibName, EntryPoint = "mx_document_renderable_count")]
    internal static partial int mx_document_renderable_count(IntPtr doc);
    [LibraryImport(LibName, EntryPoint = "mx_document_renderable_at")]
    internal static partial IntPtr mx_document_renderable_at(IntPtr doc, int index);
    // ------------- Shader code generation -------------
    [LibraryImport(LibName, EntryPoint = "mx_shadergen_create")]
    internal static partial IntPtr mx_shadergen_create(int target);
    [LibraryImport(LibName, EntryPoint = "mx_shadergen_release")]
    internal static partial void mx_shadergen_release(IntPtr g);
    [LibraryImport(LibName, EntryPoint = "mx_shadergen_target_name")]
    internal static partial IntPtr mx_shadergen_target_name_ptr(IntPtr g);
    internal static string mx_shadergen_target_name(IntPtr g) => ReadUtf8(mx_shadergen_target_name_ptr(g));
    [LibraryImport(LibName, EntryPoint = "mx_gencontext_create")]
    internal static partial IntPtr mx_gencontext_create(IntPtr g);
    [LibraryImport(LibName, EntryPoint = "mx_gencontext_release")]
    internal static partial void mx_gencontext_release(IntPtr c);
    [LibraryImport(LibName, EntryPoint = "mx_gencontext_add_source_search_path", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial MxStatus mx_gencontext_add_source_search_path(IntPtr c, string path);
    [LibraryImport(LibName, EntryPoint = "mx_shadergen_generate", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_shadergen_generate(IntPtr g, string? name, IntPtr element, IntPtr ctx);
    [LibraryImport(LibName, EntryPoint = "mx_shader_release")]
    internal static partial void mx_shader_release(IntPtr s);
    [LibraryImport(LibName, EntryPoint = "mx_shader_stage_count")]
    internal static partial int mx_shader_stage_count(IntPtr s);
    [LibraryImport(LibName, EntryPoint = "mx_shader_stage_name_at")]
    internal static partial IntPtr mx_shader_stage_name_at_ptr(IntPtr s, int index);
    internal static string mx_shader_stage_name_at(IntPtr s, int index) => ReadUtf8(mx_shader_stage_name_at_ptr(s, index));
    [LibraryImport(LibName, EntryPoint = "mx_shader_get_source_code", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr mx_shader_get_source_code_ptr(IntPtr s, string stageName);
    internal static string mx_shader_get_source_code(IntPtr s, string stageName) => ReadUtf8(mx_shader_get_source_code_ptr(s, stageName));
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
