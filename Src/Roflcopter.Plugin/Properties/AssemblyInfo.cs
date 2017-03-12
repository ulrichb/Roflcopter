using System.Reflection;

[assembly: AssemblyTitle(AssemblyConsts.Title)]
[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]

// ReSharper disable once CheckNamespace
internal static class AssemblyConsts
{
    public const string Title =
            "Roflcopter ReSharper Plugin"
#if DEBUG
            + " (Debug Build)"
#endif
        ;
}
