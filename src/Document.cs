using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// A MaterialX <c>Document</c> - the root container for materials, node graphs,
/// and other elements. Owns a native <c>mx_document_t</c>; dispose to release.
/// </summary>
public sealed class Document : IDisposable
{
    internal readonly DocumentHandle Handle;

    private Document(DocumentHandle handle) { Handle = handle; }

    /// <summary>Creates an empty MaterialX document.</summary>
    public static Document Create()
    {
        var raw = MaterialXException.ThrowIfNull("mx_document_create", MaterialXNative.mx_document_create());
        var h = new DocumentHandle();
        h.SetRaw(raw);
        return new Document(h);
    }

    /// <summary>
    /// Loads MaterialX into the document from an <c>.mtlx</c> file on disk.
    /// </summary>
    public static Document ReadFromXmlFile(string path, string? searchPath = null)
    {
        var doc = Create();
        try
        {
            MaterialXException.ThrowIfError(
                "mx_document_read_from_xml_file",
                MaterialXNative.mx_document_read_from_xml_file(doc.Handle.Raw, path, searchPath));
            return doc;
        }
        catch
        {
            doc.Dispose();
            throw;
        }
    }

    /// <summary>Loads a MaterialX document from an in-memory XML string.</summary>
    public static Document ReadFromXmlString(string xml, string? searchPath = null)
    {
        var doc = Create();
        try
        {
            MaterialXException.ThrowIfError(
                "mx_document_read_from_xml_string",
                MaterialXNative.mx_document_read_from_xml_string(doc.Handle.Raw, xml, searchPath));
            return doc;
        }
        catch
        {
            doc.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Imports the MaterialX standard libraries (stdlib, pbrlib, bxdf, lights, ...)
    /// into this document. When <paramref name="librariesRoot"/> is <c>null</c>,
    /// the path returned by <see cref="LibrarySearch.GetDefaultLibrariesPath"/>
    /// is used.
    /// </summary>
    public void LoadStandardLibraries(string? librariesRoot = null)
    {
        var root = librariesRoot ?? LibrarySearch.GetDefaultLibrariesPath()
            ?? throw new MaterialXException("LoadStandardLibraries: bundled MaterialX libraries/ folder not found");
        MaterialXException.ThrowIfError(
            "mx_document_load_libraries",
            MaterialXNative.mx_document_load_libraries(Handle.Raw, root));
    }

    /// <summary>Writes this document to an <c>.mtlx</c> file on disk.</summary>
    public void WriteToXmlFile(string path)
    {
        MaterialXException.ThrowIfError(
            "mx_document_write_to_xml_file",
            MaterialXNative.mx_document_write_to_xml_file(Handle.Raw, path));
    }

    /// <summary>Serializes this document to an XML string.</summary>
    public string WriteToXmlString() => MaterialXNative.mx_document_write_to_xml_string(Handle.Raw);

    /// <summary>
    /// Validates the document. Returns <c>true</c> if valid; otherwise the
    /// validation report is returned via <paramref name="message"/>.
    /// </summary>
    public bool Validate(out string message)
    {
        var ok = MaterialXNative.mx_document_validate(Handle.Raw, out var msgPtr);
        message = msgPtr == IntPtr.Zero
            ? string.Empty
            : System.Runtime.InteropServices.Marshal.PtrToStringUTF8(msgPtr) ?? string.Empty;
        return ok != 0;
    }

    /// <summary>Adds a top-level node to the document.</summary>
    public Node AddNode(string category, string? name = null, string type = "color3")
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_document_add_node",
            MaterialXNative.mx_document_add_node(Handle.Raw, category, name, type));
        var h = new NodeHandle();
        h.SetRaw(raw);
        return new Node(h);
    }

    /// <summary>Adds a top-level node graph to the document.</summary>
    public NodeGraph AddNodeGraph(string? name = null)
    {
        var raw = MaterialXException.ThrowIfNull(
            "mx_document_add_nodegraph",
            MaterialXNative.mx_document_add_nodegraph(Handle.Raw, name));
        var h = new NodeGraphHandle();
        h.SetRaw(raw);
        return new NodeGraph(h);
    }

    /// <summary>Enumerates direct children of the document as polymorphic <see cref="Element"/>s.</summary>
    public IEnumerable<Element> Children
    {
        get
        {
            var count = MaterialXNative.mx_element_child_count(Handle.Raw);
            for (var i = 0; i < count; i++)
            {
                var raw = MaterialXNative.mx_element_child_at(Handle.Raw, i);
                if (raw == IntPtr.Zero) continue;
                var h = new ElementHandle();
                h.SetRaw(raw);
                yield return new Element(h);
            }
        }
    }

    public void Dispose() => Handle.Dispose();
}

