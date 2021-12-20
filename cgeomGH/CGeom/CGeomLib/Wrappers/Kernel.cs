using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
    public static partial class Kernel
    {
        private const string cgeom_dylib = "libcgeom.dylib";

        public static class DiscreteQuantities
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglPrincipalCurvatures")]
            internal static extern void IglPrincipalCurvatures(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr dir1, out IntPtr dir2, out IntPtr val1, out IntPtr val2);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglLaplacianSmoothing")]
            internal static extern void IglLaplacianSmoothing(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, int numIterations, out IntPtr outCoords);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglNormalsPerVertex")]
            internal static extern void IglNormalsPerVertex(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglNormalsPerFace")]
            internal static extern void IglNormalsPerFace(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglNormalsPerCorner")]
            internal static extern void IglNormalsPerCorner(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, double angle, out IntPtr outNorm);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglGaussianCurvature")]
            internal static extern void IglGaussianCurvature(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outK);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "iglMeanCurvature")]
            internal static extern void IglMeanCurvature(int numVertices, int numFaces, [In] double[] inCoords, [In] int[] inFaces, out IntPtr outH);
        }
    }
}
