using System;
using CGeom.Wrappers;
using Rhino.Geometry;
using System.Linq;
using System.Runtime.InteropServices;

namespace CGeom.Tools
{
    public static class DiscreteOperators
    {
        public static void LaplacianSmoothing(int numIterations, ref Mesh mesh)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices,numFaces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr pos;
            Kernel.DiscreteQuantities.IglLaplacianSmoothing(numVertices, numFaces, coords, faces, numIterations, out pos);

            // Parse new vertex positions
            double[] outPos = new double[numVertices * 3];
            Marshal.Copy(pos, outPos, 0, numVertices * 3);
            Marshal.FreeCoTaskMem(pos);

            // Update Mesh
            for (int i = 0; i < numVertices; i++)
            {
                mesh.Vertices.SetVertex(i, new Point3d(outPos[i * 3], outPos[i * 3 + 1], outPos[i * 3 + 2]));
            }
        }
    }
}
