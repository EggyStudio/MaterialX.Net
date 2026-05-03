/*
 * MaterialXC - flat C ABI over the MaterialX C++ API.
 *
 * Built as a SHARED library that statically links the upstream MaterialX
 * archives (libMaterialXCore.a / .lib, libMaterialXFormat.a / .lib).
 *
 * Conventions:
 *   - All handles are opaque pointers. NULL = null/error.
 *   - Strings are UTF-8 zero-terminated. Returned strings are owned by an
 *     internal thread-local buffer and remain valid until the next call
 *     returning a string on the same thread.
 *   - Every _release function is null-safe.
 *   - Non-zero return codes are mx_status values; the corresponding text is
 *     available via mx_last_error().
 */
#ifndef MATERIALXC_H
#define MATERIALXC_H

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32)
  #if defined(MATERIALXC_BUILDING)
    #define MX_API __declspec(dllexport)
  #else
    #define MX_API __declspec(dllimport)
  #endif
#else
  #define MX_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* ---------- status / diagnostics ---------- */

typedef enum mx_status {
    MX_OK = 0,
    MX_ERR_INVALID_ARG = 1,
    MX_ERR_IO = 2,
    MX_ERR_PARSE = 3,
    MX_ERR_NOT_FOUND = 4,
    MX_ERR_INTERNAL = 5
} mx_status;

/* Returns the last error message produced on the calling thread (UTF-8),
 * or an empty string if no error has been recorded. */
MX_API const char* mx_last_error(void);

/* Returns the linked MaterialX library version, e.g. "1.39.3". */
MX_API const char* mx_version(void);

/* ---------- opaque handles ---------- */

typedef struct mx_document_s* mx_document_t;
typedef struct mx_element_s*  mx_element_t;   /* polymorphic - any Element */
typedef struct mx_node_s*     mx_node_t;
typedef struct mx_nodegraph_s* mx_nodegraph_t;
typedef struct mx_input_s*    mx_input_t;
typedef struct mx_output_s*   mx_output_t;

/* Every handle is owned. Release through the matching destructor. All
 * destructors are null-safe and idempotent. */
MX_API void mx_document_release(mx_document_t h);
MX_API void mx_element_release(mx_element_t h);
MX_API void mx_node_release(mx_node_t h);
MX_API void mx_nodegraph_release(mx_nodegraph_t h);
MX_API void mx_input_release(mx_input_t h);
MX_API void mx_output_release(mx_output_t h);

/* ---------- Document ---------- */

MX_API mx_document_t mx_document_create(void);

/* Loads MaterialX documents from a search path (OS-native separator-joined,
 * ';' on Windows, ':' elsewhere). Pass NULL for libraries_root to use the
 * default ("libraries" next to the host binary). Returns MX_OK or an error. */
MX_API mx_status mx_document_load_libraries(mx_document_t doc,
                                            const char* libraries_root);

MX_API mx_status mx_document_read_from_xml_file(mx_document_t doc,
                                                const char* path,
                                                const char* search_path);

MX_API mx_status mx_document_read_from_xml_string(mx_document_t doc,
                                                  const char* xml,
                                                  const char* search_path);

MX_API mx_status mx_document_write_to_xml_file(mx_document_t doc,
                                               const char* path);

/* Returned pointer is owned by an internal per-thread buffer; copy if you
 * need to retain it past the next MaterialXC string-returning call. */
MX_API const char* mx_document_write_to_xml_string(mx_document_t doc);

MX_API int32_t mx_document_validate(mx_document_t doc, const char** out_message);

/* ---------- Element traversal (Document/NodeGraph share Element base) ---------- */

MX_API int32_t mx_element_child_count(mx_element_t parent);
/* Returns a new owned handle (release with mx_element_release). NULL on OOB. */
MX_API mx_element_t mx_element_child_at(mx_element_t parent, int32_t index);

MX_API const char* mx_element_get_name(mx_element_t e);
MX_API const char* mx_element_get_category(mx_element_t e);
MX_API const char* mx_element_get_namespace(mx_element_t e);

/* Try-cast helpers. Return a new owned handle of the requested kind, or NULL
 * if the element is not of that kind. The original element handle remains
 * valid and must still be released independently. */
MX_API mx_node_t      mx_element_as_node(mx_element_t e);
MX_API mx_nodegraph_t mx_element_as_nodegraph(mx_element_t e);
MX_API mx_input_t     mx_element_as_input(mx_element_t e);
MX_API mx_output_t    mx_element_as_output(mx_element_t e);

/* ---------- Node / NodeGraph ---------- */

/* Add a node directly under the document. Returns NULL on failure. */
MX_API mx_node_t mx_document_add_node(mx_document_t doc,
                                      const char* category,
                                      const char* name,
                                      const char* type);

MX_API mx_nodegraph_t mx_document_add_nodegraph(mx_document_t doc, const char* name);

MX_API mx_node_t mx_nodegraph_add_node(mx_nodegraph_t g,
                                       const char* category,
                                       const char* name,
                                       const char* type);

MX_API const char* mx_node_get_name(mx_node_t n);
MX_API const char* mx_node_get_category(mx_node_t n);
MX_API const char* mx_node_get_type(mx_node_t n);

/* Add or fetch an Input on a node. Returns a new owned handle. */
MX_API mx_input_t mx_node_get_or_add_input(mx_node_t n, const char* name, const char* type);

/* ---------- Input / Output value setters ---------- */

MX_API mx_status mx_input_set_value_int(mx_input_t in, int32_t value);
MX_API mx_status mx_input_set_value_float(mx_input_t in, float value);
MX_API mx_status mx_input_set_value_bool(mx_input_t in, int32_t value);
MX_API mx_status mx_input_set_value_string(mx_input_t in, const char* value);
MX_API mx_status mx_input_set_value_color3(mx_input_t in, float r, float g, float b);
MX_API mx_status mx_input_set_value_color4(mx_input_t in, float r, float g, float b, float a);
MX_API mx_status mx_input_set_value_vector2(mx_input_t in, float x, float y);
MX_API mx_status mx_input_set_value_vector3(mx_input_t in, float x, float y, float z);
MX_API mx_status mx_input_set_value_vector4(mx_input_t in, float x, float y, float z, float w);

/* Connect this input to an output of another node by name (or to a top-level
 * output element if 'node_name' is NULL). */
MX_API mx_status mx_input_connect_to_node(mx_input_t in, const char* node_name, const char* output_name);

#ifdef __cplusplus
}
#endif

#endif /* MATERIALXC_H */

