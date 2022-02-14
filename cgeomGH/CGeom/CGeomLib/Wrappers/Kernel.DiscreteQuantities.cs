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
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomPrincipalCurvatures")]
            internal static extern void CgeomPrincipalCurvatures(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr dir1, out IntPtr dir2, out IntPtr val1, out IntPtr val2);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomLaplacianSmoothingForOpenMesh")]
            internal static extern void CgeomLaplacianSmoothingForOpenMesh(int numVertices, int numFaces, int numBoundaries, [In] double[] inCoords, [In] int[] inFaces, [In] int[] inBoundaries, [In] int[] inInteriors, int numIterations, out IntPtr outCoords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomLaplacianSmoothingForCloseMesh")]
            internal static extern void CgeomLaplacianSmoothingForCloseMesh(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, double smoothing, int numIterations, out IntPtr outCoords);

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
        }
    }
}
