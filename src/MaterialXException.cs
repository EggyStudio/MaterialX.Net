using MaterialX.Native;

namespace MaterialX;

/// <summary>
/// Exception type raised by MaterialX.Net wrappers. Carries the
/// <see cref="MxStatus"/> code reported by the native shim plus the
/// thread-local last-error message (<c>mx_last_error</c>).
/// </summary>
public sealed class MaterialXException : Exception
{
    /// <summary>Native status code from the failing call.</summary>
    public int StatusCode { get; }

    internal MaterialXException(string operation, MxStatus status)
        : base($"{operation} failed ({status}): {MaterialXNative.mx_last_error()}")
    {
        StatusCode = (int)status;
    }

    internal MaterialXException(string operation)
        : base($"{operation} failed: {MaterialXNative.mx_last_error()}")
    { }

    internal static void ThrowIfError(string operation, MxStatus status)
    {
        if (status != MxStatus.Ok) throw new MaterialXException(operation, status);
    }

    internal static IntPtr ThrowIfNull(string operation, IntPtr handle)
    {
        if (handle == IntPtr.Zero) throw new MaterialXException(operation);
        return handle;
    }
}

