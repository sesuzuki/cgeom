using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        public static class Processing
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
            internal static extern void CgeomNRosy(int numVertices, int numFaces, int numConstraints,double[] inCoords, int[] inFaces,
                                                   int[] inB, double[] inFF1, int degree, 
                                                   out int outX1CoordsCount, out int outX2CoordsCount, out int outSingularitiesCount,
                                                   out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outSingularities);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNRosyFromFrameField")]
            internal static extern void CgeomNRosyFromFrameField(int numVertices, int numFaces, int numConstraints, double[] inCoords, int[] inFaces,
                                       int[] inB, double[] inFF1, double[] inFF2, int degree,
                                       out int outX1CoordsCount, out int outX2CoordsCount, out int outSingularitiesCount,
                                       out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outSingularities);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPlanarization")]
            internal static extern void CgeomPlanarization(int inVertexCount, int inQuadsCount, [In] double[] inCoords, [In] int[] inQuads, int iterations, double threshold, out int outVertexCount, out int outPlanarityCount, out IntPtr outCoords, out IntPtr outPlanarity);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomRotateVectors")]
            internal static extern void CgeomRotateVectors(int numVectors, [In] double[] inX1Coords, [In] double[] inB1Coords, [In] double[] inB2Coords, [In] double[] inAngle, out int outCount, out IntPtr outX1Coords);


            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomParallelTransport")]
            internal static extern int CgeomParallelTransport(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, int inSourceVertex, double initialPara, double initialPerp, out int outCount, out IntPtr outCoords, out IntPtr outVecCoords, out IntPtr error);


            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomEdgeVectors")]
            internal static extern int CgeomEdgeVectors(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outCount, out IntPtr outEdgeMidCoords, out IntPtr outParCoords, out IntPtr outPerpCoords, out IntPtr error);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomRemeshAlongIsoline")]
            internal static extern int CgeomRemeshAlongIsoline(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, [In] double[] inScalarField, double inIsoValue, out int outVertexCount, out int outFaceCount, out IntPtr outCoords, out IntPtr outFaces, out IntPtr error);
        }
    }
}
