namespace MaterialX;

/// <summary>
/// Locates the MaterialX standard <c>libraries/</c> data folder. The package's
/// MSBuild targets stage these files next to the consumer assembly at
/// build/publish time, normally at <c>AppContext.BaseDirectory/libraries</c>.
/// </summary>
public static class LibrarySearch
{
    /// <summary>
    /// Probes well-known locations (in order) for a directory named
    /// <c>libraries</c> containing the MaterialX standard data:
    /// <list type="number">
    ///   <item><c>$(AppContext.BaseDirectory)/libraries</c> (default for an app)</item>
    ///   <item><c>$(AppContext.BaseDirectory)/runtimes/any/native/libraries</c> (raw NuGet payload location)</item>
    ///   <item>Walks up from the executing assembly looking for <c>libraries/stdlib</c>.</item>
    /// </list>
    /// Returns <c>null</c> if not found.
    /// </summary>
    public static string? GetDefaultLibrariesPath()
    {
        var baseDir = AppContext.BaseDirectory;

        var candidates = new[]
        {
            Path.Combine(baseDir, "libraries"),
            Path.Combine(baseDir, "runtimes", "any", "native", "libraries"),
        };

        foreach (var c in candidates)
        {
            if (LooksLikeMaterialXLibraries(c)) return c;
        }

        // Walk up from the host directory (handy for samples in deep build trees).
        var dir = new DirectoryInfo(baseDir);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "libraries");
            if (LooksLikeMaterialXLibraries(candidate)) return candidate;
            dir = dir.Parent;
        }

        return null;
    }

    private static bool LooksLikeMaterialXLibraries(string path)
        => Directory.Exists(path) && Directory.Exists(Path.Combine(path, "stdlib"));
}

