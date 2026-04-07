namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        // Let the runtime resolve the platform-specific library filename:
        // cgeom.dll on Windows, libcgeom.dylib on macOS, libcgeom.so on Linux.
        private const string cgeom_dylib = "cgeom";
    }
}
