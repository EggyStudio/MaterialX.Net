using System.Runtime.InteropServices;

namespace MaterialX;

/// <summary>2-component float vector matching <c>MaterialX::Vector2</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Vector2(float X, float Y);

/// <summary>3-component float vector matching <c>MaterialX::Vector3</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Vector3(float X, float Y, float Z);

/// <summary>4-component float vector matching <c>MaterialX::Vector4</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Vector4(float X, float Y, float Z, float W);

/// <summary>RGB color matching <c>MaterialX::Color3</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Color3(float R, float G, float B);

/// <summary>RGBA color matching <c>MaterialX::Color4</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Color4(float R, float G, float B, float A);

