using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        private const string cgeom_dylib = "libcgeom.dylib";

        public static class Curvature
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglPrincipalCurvatures")]
            internal static extern double IglPrincipalCurvatures(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr dir1, out IntPtr dir2, out IntPtr val1, out IntPtr val2);
        }
    }
}
