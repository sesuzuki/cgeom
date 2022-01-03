using System;
using System.Runtime.InteropServices;
using CGeom.Wrappers;
using Rhino.Geometry;

namespace CGeom.Tools
{
    public static class Parameterizations
    {
        /// <summary>
        /// Seamless Integer-Grid Quadrangulation (MIQ)
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh MIQ(Mesh mesh)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            int numCoords, numQuads;
            IntPtr cPtr, qPtr;
            Kernel.Parameterization.IglSeamlessIntegerGridParameterization(numVertices, numFaces, coords, faces, out numCoords, out numQuads, out cPtr, out qPtr);

            double[] outCoords = new double[numCoords];
            int[] outQuads = new int[numQuads];
            Marshal.Copy(cPtr, outCoords, 0, numCoords);
            Marshal.Copy(qPtr, outQuads, 0, numQuads);
            Marshal.FreeCoTaskMem(cPtr);
            Marshal.FreeCoTaskMem(qPtr);

            return Utils.GetQuadMesh(outCoords, outQuads);
        }
    }
}
