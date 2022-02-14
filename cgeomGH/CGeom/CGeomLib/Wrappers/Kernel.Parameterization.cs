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
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomSeamlessIntegerGridParameterization")]
            internal static extern void CgeomSeamlessIntegerGridParameterization(int numVertices, int numFaces, double[] inCoords, int[] inFaces, double[] inCoordsX1, double[] inCoordsX2,
                                                                  double gradient_size, double stiffness, bool direct_round, int numIterations, out int outNumUV, out int outNumFUV, out IntPtr outUV, out IntPtr outFUV);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomQuadMeshExtraction")]
            internal static extern void CgeomQuadMeshExtraction(int inVertexCount, int inTriasCount, double[] inCoords, int[] inTrias, double[] inUV, int[] inFUV,
                                                                out int outVertexCount, out int outQuadsCount, out IntPtr outCoords, out IntPtr outQuads);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNRosy")]
            internal static extern void CgeomNRosy(int numVertices, int numFaces, int numConstraints, double[] inCoords, int[] inFaces, int[] inConstrainedFaces, double[] inConstrainedVectorFaces, int degree, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outBarycentersCoords, out IntPtr outSingularities);
        }
    }
}
