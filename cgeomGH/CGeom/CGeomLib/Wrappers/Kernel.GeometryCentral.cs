using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CGeom.Wrappers
{
	public partial class Kernel
	{
        public static class GeometryCentral
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(cgeom_dylib, CallingConvention = CallingConvention.StdCall, EntryPoint = "cgeomGetFlipGeodesics")]
            internal static extern void CgeomGetFlipGeodesics(int numVertices, int numFaces, int numPoints, double[] inCoords, int[] inFaces, int[] inPtsIndices, int[] inPointOffset, out IntPtr outPointCoords, out IntPtr outPointOffsets, out int outNumCoords, out int outNumOffsets);
        }
    }
}

