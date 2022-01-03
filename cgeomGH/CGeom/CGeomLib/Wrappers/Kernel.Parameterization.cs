using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        public static class Parameterization
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglSeamlessIntegerGridParameterization")]
            internal static extern void IglSeamlessIntegerGridParameterization(int numVertices, int numFaces, double[] inCoords, int[] inFaces,
                                                                               out int outVertexCount, out int outFaceCount, out IntPtr outVertexCoords, out IntPtr outFaceIndexes);
        }
    }
}
