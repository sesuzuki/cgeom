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
            internal static extern void CgeomQuadMeshExtraction(int inVertexCount, int inTriasCount, int inUVCount, int inFUVCount, double[] inCoords, int[] inTrias, double[] inUV, int[] inFUV,
                                                                out int outVertexCount, out int outQuadsCount, out IntPtr outCoords, out IntPtr outQuads);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNRosy")]
            internal static extern void CgeomNRosy(int numVertices, int numFaces, int numConstraints, double[] inCoords, int[] inFaces, int[] inConstrainedFaces, double[] inConstrainedVectorFaces, int degree,
                                                   out int outX1CoordsCount, out int outX2CoordsCount, out int outBarycentersCoordsCount, out int outSingularitiesCount, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outBarycentersCoords, out IntPtr outSingularities);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPlanarization")]
            internal static extern void CgeomPlanarization(int inVertexCount, int inQuadsCount, [In] double[] inCoords, [In] int[] inQuads, int iterations, double threshold, out int outVertexCount, out IntPtr outCoords);
        }
    }
}
