using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        public static class BFF
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomOpenSurfaceWithoutHolesToDisk")]
            internal static extern int CgeomOpenSurfaceWithoutHolesToDisk(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int numUV, out IntPtr outUV, out int numUVFaces, out IntPtr outUVFaces, out IntPtr errorMessage);
        }
    }
}

