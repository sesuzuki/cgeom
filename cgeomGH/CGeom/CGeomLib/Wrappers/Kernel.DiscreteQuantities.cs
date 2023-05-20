using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        public static class DiscreteQuantities
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomBarycenters")]
            internal static extern void CgeomBarycenters(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outCoordsCount, out IntPtr outCoords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPerFaceAsymptoticDirections")]
            internal static extern int CgeomPerFaceAsymptoticDirections(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outX1CoordsCount, out int outX2CoordsCount, out int outVanishingCount, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outVanishingIndex, out IntPtr error);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPerFacePrincipalCurvatures")]
            internal static extern void CgeomPerFacePrincipalCurvatures(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outX1CoordsCount, out int outX2CoordsCount, out int outK1Count, out int outK2Count, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outK1, out IntPtr outK2);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPerVertexAsymptoticDirections")]
            internal static extern int CgeomPerVertexAsymptoticDirections(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outX1CoordsCount, out int outX2CoordsCount, out int outVanishingCount, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outVanishingIndex, out IntPtr error);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPerVertexPrincipalCurvatures")]
            internal static extern void CgeomPerVertexPrincipalCurvatures(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outX1CoordsCount, out int outX2CoordsCount, out int outK1Count, out int outK2Count, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outK1, out IntPtr outK2);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomLaplacianSmoothingForOpenMesh")]
            internal static extern void CgeomLaplacianSmoothingForOpenMesh(int numVertices, int numFaces, int numBoundaries, [In] double[] inCoords, [In] int[] inFaces, [In] int[] inBoundaries, [In] int[] inInteriors, int numIterations, out int outCoordsCount, out IntPtr outCoords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomLaplacianSmoothingForCloseMesh")]
            internal static extern void CgeomLaplacianSmoothingForCloseMesh(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, double smoothing, int numIterations, out int outCoordsCount, out IntPtr outCoords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNormalsPerVertex")]
            internal static extern void CgeomNormalsPerVertex(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNormalsPerFace")]
            internal static extern void CgeomNormalsPerFace(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomNormalsPerCorner")]
            internal static extern void CgeomNormalsPerCorner(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, double angle, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomGaussianCurvature")]
            internal static extern void CgeomGaussianCurvature(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outK);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomMeanCurvature")]
            internal static extern void CgeomMeanCurvature(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outH);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomLocalBasis")]
            internal static extern void CgeomLocalBasis(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out int outCount, out IntPtr outX1Coords, out IntPtr outX2Coords, out IntPtr outX3Coords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomExactDiscreteGeodesicDistances")]
            internal static extern int CgeomExactDiscreteGeodesicDistances(int numVertices, int numFaces, int numSourceVertices, int numSourceFaces, int numTargetVertices, int numTargetFaces, [In] double[] inCoords, [In] int[] inFaces, [In] int[] inSourceVertices, [In] int[] inSourceFaces, [In] int[] inTargetVertices, [In] int[] inTargetFaces, out int outDistancesCount, out IntPtr outDistances, out IntPtr error);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomHeatGeodesics")]
            internal static extern int CgeomHeatGeodesics(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, double t, int inSourceVertex, bool useIntrinsicDelaunay, out int outDistancesCount, out IntPtr outDistances, out IntPtr error);
        }
    }
}
