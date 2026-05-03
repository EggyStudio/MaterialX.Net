// MaterialXC - implementation of the flat C ABI declared in MaterialXC.h.
// Each opaque mx_*_t handle is a heap-allocated MaterialX::*Ptr (shared_ptr)
// so lifetime follows refcounted semantics across the FFI boundary. Releasing
// a handle drops one reference; the underlying C++ object survives as long as
// MaterialX itself holds another reference (e.g. via parent->getChild()).

#define MATERIALXC_BUILDING

#include "MaterialXC.h"

#include <MaterialXCore/Document.h>
#include <MaterialXCore/Node.h>
#include <MaterialXCore/Element.h>
#include <MaterialXCore/Types.h>
#include <MaterialXFormat/XmlIo.h>
#include <MaterialXFormat/File.h>
#include <MaterialXFormat/Util.h>

#include <exception>
#include <string>
#include <cstring>

namespace mx = MaterialX;

// ---------- per-thread error / string buffers ----------

namespace {
    thread_local std::string g_lastError;
    thread_local std::string g_stringBuf;

    const char* setStr(const std::string& s) {
        g_stringBuf = s;
        return g_stringBuf.c_str();
    }
    const char* setStr(std::string&& s) {
        g_stringBuf = std::move(s);
        return g_stringBuf.c_str();
    }

    mx_status reportException(const std::exception& e, mx_status code = MX_ERR_INTERNAL) {
        g_lastError = e.what();
        return code;
    }
    mx_status reportError(const char* msg, mx_status code = MX_ERR_INTERNAL) {
        g_lastError = msg ? msg : "";
        return code;
    }

    // Box helpers: every public handle is a heap-allocated shared_ptr.
    template <typename T>
    inline T* box(std::shared_ptr<typename T::element_type> ptr) {
        if (!ptr) return nullptr;
        return new T(std::move(ptr));
    }

    template <typename Handle>
    inline auto unbox(Handle h) -> typename std::remove_pointer<Handle>::type& {
        return *reinterpret_cast<typename std::remove_pointer<Handle>::type*>(h);
    }
}

// Concrete "handle" types are aliases to the corresponding MaterialX shared_ptrs.
struct mx_document_s   : mx::DocumentPtr  { using mx::DocumentPtr::DocumentPtr;  mx_document_s(mx::DocumentPtr p)  : mx::DocumentPtr(std::move(p))  {} };
struct mx_element_s    : mx::ElementPtr   { using mx::ElementPtr::ElementPtr;   mx_element_s(mx::ElementPtr p)    : mx::ElementPtr(std::move(p))   {} };
struct mx_node_s       : mx::NodePtr      { using mx::NodePtr::NodePtr;         mx_node_s(mx::NodePtr p)          : mx::NodePtr(std::move(p))      {} };
struct mx_nodegraph_s  : mx::NodeGraphPtr { using mx::NodeGraphPtr::NodeGraphPtr; mx_nodegraph_s(mx::NodeGraphPtr p) : mx::NodeGraphPtr(std::move(p)) {} };
struct mx_input_s      : mx::InputPtr     { using mx::InputPtr::InputPtr;       mx_input_s(mx::InputPtr p)        : mx::InputPtr(std::move(p))     {} };
struct mx_output_s     : mx::OutputPtr    { using mx::OutputPtr::OutputPtr;     mx_output_s(mx::OutputPtr p)      : mx::OutputPtr(std::move(p))    {} };

// ---------- diagnostics ----------

extern "C" MX_API const char* mx_last_error(void) {
    return g_lastError.c_str();
}

extern "C" MX_API const char* mx_version(void) {
    static std::string v = std::to_string(MATERIALX_MAJOR_VERSION) + "." +
                           std::to_string(MATERIALX_MINOR_VERSION) + "." +
                           std::to_string(MATERIALX_BUILD_VERSION);
    return v.c_str();
}

// ---------- release ----------

extern "C" MX_API void mx_document_release(mx_document_t h)   { delete h; }
extern "C" MX_API void mx_element_release(mx_element_t h)     { delete h; }
extern "C" MX_API void mx_node_release(mx_node_t h)           { delete h; }
extern "C" MX_API void mx_nodegraph_release(mx_nodegraph_t h) { delete h; }
extern "C" MX_API void mx_input_release(mx_input_t h)         { delete h; }
extern "C" MX_API void mx_output_release(mx_output_t h)       { delete h; }

// ---------- Document ----------

extern "C" MX_API mx_document_t mx_document_create(void) {
    try { return new mx_document_s(mx::createDocument()); }
    catch (const std::exception& e) { reportException(e); return nullptr; }
}

extern "C" MX_API mx_status mx_document_load_libraries(mx_document_t doc, const char* libraries_root) {
    if (!doc) return reportError("doc is null", MX_ERR_INVALID_ARG);
    try {
        mx::FileSearchPath searchPath;
        mx::FilePath root = libraries_root ? mx::FilePath(libraries_root) : mx::FilePath("libraries");
        searchPath.append(root.getParentPath());
        mx::StringSet xincludeFiles = mx::loadLibraries({ root.getBaseName() }, searchPath, *doc);
        (void)xincludeFiles;
        return MX_OK;
    } catch (const std::exception& e) { return reportException(e, MX_ERR_IO); }
}

extern "C" MX_API mx_status mx_document_read_from_xml_file(mx_document_t doc, const char* path, const char* search_path) {
    if (!doc || !path) return reportError("doc/path is null", MX_ERR_INVALID_ARG);
    try {
        mx::FileSearchPath sp = search_path ? mx::FileSearchPath(search_path) : mx::FileSearchPath();
        mx::readFromXmlFile(*doc, path, sp);
        return MX_OK;
    } catch (const std::exception& e) { return reportException(e, MX_ERR_PARSE); }
}

extern "C" MX_API mx_status mx_document_read_from_xml_string(mx_document_t doc, const char* xml, const char* search_path) {
    if (!doc || !xml) return reportError("doc/xml is null", MX_ERR_INVALID_ARG);
    try {
        mx::FileSearchPath sp = search_path ? mx::FileSearchPath(search_path) : mx::FileSearchPath();
        mx::readFromXmlString(*doc, xml, sp);
        return MX_OK;
    } catch (const std::exception& e) { return reportException(e, MX_ERR_PARSE); }
}

extern "C" MX_API mx_status mx_document_write_to_xml_file(mx_document_t doc, const char* path) {
    if (!doc || !path) return reportError("doc/path is null", MX_ERR_INVALID_ARG);
    try { mx::writeToXmlFile(*doc, path); return MX_OK; }
    catch (const std::exception& e) { return reportException(e, MX_ERR_IO); }
}

extern "C" MX_API const char* mx_document_write_to_xml_string(mx_document_t doc) {
    if (!doc) { reportError("doc is null", MX_ERR_INVALID_ARG); return ""; }
    try { return setStr(mx::writeToXmlString(*doc)); }
    catch (const std::exception& e) { reportException(e); return ""; }
}

extern "C" MX_API int32_t mx_document_validate(mx_document_t doc, const char** out_message) {
    if (!doc) { reportError("doc is null", MX_ERR_INVALID_ARG); return 0; }
    try {
        std::string msg;
        bool ok = (*doc)->validate(&msg);
        if (out_message) *out_message = setStr(msg);
        return ok ? 1 : 0;
    } catch (const std::exception& e) { reportException(e); return 0; }
}

// ---------- Element traversal ----------

extern "C" MX_API int32_t mx_element_child_count(mx_element_t parent) {
    if (!parent) return 0;
    try { return static_cast<int32_t>((*parent)->getChildren().size()); }
    catch (const std::exception& e) { reportException(e); return 0; }
}

extern "C" MX_API mx_element_t mx_element_child_at(mx_element_t parent, int32_t index) {
    if (!parent) return nullptr;
    try {
        auto kids = (*parent)->getChildren();
        if (index < 0 || index >= (int32_t)kids.size()) return nullptr;
        return new mx_element_s(kids[index]);
    } catch (const std::exception& e) { reportException(e); return nullptr; }
}

extern "C" MX_API const char* mx_element_get_name(mx_element_t e) {
    if (!e) return "";
    try { return setStr((*e)->getName()); }
    catch (const std::exception& ex) { reportException(ex); return ""; }
}

extern "C" MX_API const char* mx_element_get_category(mx_element_t e) {
    if (!e) return "";
    try { return setStr((*e)->getCategory()); }
    catch (const std::exception& ex) { reportException(ex); return ""; }
}

extern "C" MX_API const char* mx_element_get_namespace(mx_element_t e) {
    if (!e) return "";
    try { return setStr((*e)->getNamespace()); }
    catch (const std::exception& ex) { reportException(ex); return ""; }
}

extern "C" MX_API mx_node_t      mx_element_as_node(mx_element_t e)       { if (!e) return nullptr; auto p = (*e)->asA<mx::Node>();      return p ? new mx_node_s(p)      : nullptr; }
extern "C" MX_API mx_nodegraph_t mx_element_as_nodegraph(mx_element_t e)  { if (!e) return nullptr; auto p = (*e)->asA<mx::NodeGraph>(); return p ? new mx_nodegraph_s(p) : nullptr; }
extern "C" MX_API mx_input_t     mx_element_as_input(mx_element_t e)      { if (!e) return nullptr; auto p = (*e)->asA<mx::Input>();     return p ? new mx_input_s(p)     : nullptr; }
extern "C" MX_API mx_output_t    mx_element_as_output(mx_element_t e)     { if (!e) return nullptr; auto p = (*e)->asA<mx::Output>();    return p ? new mx_output_s(p)    : nullptr; }

// ---------- Node / NodeGraph ----------

extern "C" MX_API mx_node_t mx_document_add_node(mx_document_t doc, const char* category, const char* name, const char* type) {
    if (!doc || !category) { reportError("doc/category is null", MX_ERR_INVALID_ARG); return nullptr; }
    try { return new mx_node_s((*doc)->addNode(category, name ? name : mx::EMPTY_STRING, type ? type : mx::DEFAULT_TYPE_STRING)); }
    catch (const std::exception& e) { reportException(e); return nullptr; }
}

extern "C" MX_API mx_nodegraph_t mx_document_add_nodegraph(mx_document_t doc, const char* name) {
    if (!doc) { reportError("doc is null", MX_ERR_INVALID_ARG); return nullptr; }
    try { return new mx_nodegraph_s((*doc)->addNodeGraph(name ? name : mx::EMPTY_STRING)); }
    catch (const std::exception& e) { reportException(e); return nullptr; }
}

extern "C" MX_API mx_node_t mx_nodegraph_add_node(mx_nodegraph_t g, const char* category, const char* name, const char* type) {
    if (!g || !category) { reportError("graph/category is null", MX_ERR_INVALID_ARG); return nullptr; }
    try { return new mx_node_s((*g)->addNode(category, name ? name : mx::EMPTY_STRING, type ? type : mx::DEFAULT_TYPE_STRING)); }
    catch (const std::exception& e) { reportException(e); return nullptr; }
}

extern "C" MX_API const char* mx_node_get_name(mx_node_t n)     { if (!n) return ""; try { return setStr((*n)->getName()); }     catch (const std::exception& e) { reportException(e); return ""; } }
extern "C" MX_API const char* mx_node_get_category(mx_node_t n) { if (!n) return ""; try { return setStr((*n)->getCategory()); } catch (const std::exception& e) { reportException(e); return ""; } }
extern "C" MX_API const char* mx_node_get_type(mx_node_t n)     { if (!n) return ""; try { return setStr((*n)->getType()); }     catch (const std::exception& e) { reportException(e); return ""; } }

extern "C" MX_API mx_input_t mx_node_get_or_add_input(mx_node_t n, const char* name, const char* type) {
    if (!n || !name) { reportError("node/name is null", MX_ERR_INVALID_ARG); return nullptr; }
    try {
        mx::InputPtr in = (*n)->getInput(name);
        if (!in) in = (*n)->addInput(name, type ? type : mx::DEFAULT_TYPE_STRING);
        return new mx_input_s(in);
    } catch (const std::exception& e) { reportException(e); return nullptr; }
}

// ---------- Input setters ----------

#define MX_INPUT_SETTER(NAME, EXPR) \
    if (!in) return reportError("input is null", MX_ERR_INVALID_ARG); \
    try { (*in)->setValue(EXPR); return MX_OK; } \
    catch (const std::exception& e) { return reportException(e); }

extern "C" MX_API mx_status mx_input_set_value_int(mx_input_t in, int32_t v)    { MX_INPUT_SETTER("integer", (int)v) }
extern "C" MX_API mx_status mx_input_set_value_float(mx_input_t in, float v)    { MX_INPUT_SETTER("float", v) }
extern "C" MX_API mx_status mx_input_set_value_bool(mx_input_t in, int32_t v)   { MX_INPUT_SETTER("boolean", (bool)(v != 0)) }
extern "C" MX_API mx_status mx_input_set_value_string(mx_input_t in, const char* v) {
    if (!in) return reportError("input is null", MX_ERR_INVALID_ARG);
    try { (*in)->setValue<std::string>(v ? v : ""); return MX_OK; }
    catch (const std::exception& e) { return reportException(e); }
}
extern "C" MX_API mx_status mx_input_set_value_color3(mx_input_t in, float r, float g, float b)            { MX_INPUT_SETTER("color3",  mx::Color3(r,g,b)) }
extern "C" MX_API mx_status mx_input_set_value_color4(mx_input_t in, float r, float g, float b, float a)   { MX_INPUT_SETTER("color4",  mx::Color4(r,g,b,a)) }
extern "C" MX_API mx_status mx_input_set_value_vector2(mx_input_t in, float x, float y)                    { MX_INPUT_SETTER("vector2", mx::Vector2(x,y)) }
extern "C" MX_API mx_status mx_input_set_value_vector3(mx_input_t in, float x, float y, float z)           { MX_INPUT_SETTER("vector3", mx::Vector3(x,y,z)) }
extern "C" MX_API mx_status mx_input_set_value_vector4(mx_input_t in, float x, float y, float z, float w)  { MX_INPUT_SETTER("vector4", mx::Vector4(x,y,z,w)) }

#undef MX_INPUT_SETTER

extern "C" MX_API mx_status mx_input_connect_to_node(mx_input_t in, const char* node_name, const char* output_name) {
    if (!in) return reportError("input is null", MX_ERR_INVALID_ARG);
    try {
        if (node_name && *node_name) (*in)->setNodeName(node_name);
        if (output_name && *output_name) (*in)->setOutputString(output_name);
        return MX_OK;
    } catch (const std::exception& e) { return reportException(e); }
}

