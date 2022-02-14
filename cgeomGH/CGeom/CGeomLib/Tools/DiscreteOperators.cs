using System;
using CGeom.Wrappers;
using Rhino.Geometry;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CGeom.Tools;
using static CGeom.Tools.Utils;

namespace CGeom.Tools
{
    public static class DiscreteOperators
    {
        public static void LaplacianSmoothingForOpenMesh(int numIterations, ref Mesh mesh, IEnumerable<Point3d> anchors, double tolerance = 1e-3)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            List<int> indexes = Enumerable.Range(0, numVertices).ToList();
            HashSet<int> inBoundaries = new HashSet<int>();

            PointCloud cloud = new PointCloud(mesh.Vertices.ToPoint3dArray());

            for (int i = 0; i < anchors.Count(); i++)
            {
                var p1 = anchors.ElementAt(i);
                int idx = cloud.ClosestPoint(p1);
                var p2 = cloud[idx].Location;

                if (p1.DistanceTo(p2) <= tolerance) inBoundaries.Add(idx);
            }

            int[] inInteriors = indexes.Except(inBoundaries).ToArray();

            IntPtr ptrCoords;
            Kernel.DiscreteQuantities.CgeomLaplacianSmoothingForOpenMesh(numVertices, numFaces, inBoundaries.Count, coords, faces, inBoundaries.ToArray(), inInteriors, numIterations, out ptrCoords);

            // Parse new vertex positions
            ParsePointerToMeshVertices(ptrCoords, numVertices * 3, ref mesh);
        }

        public static void LaplacianSmoothingForCloseMesh(int numIterations, ref Mesh mesh, double smoothing, double tolerance = 1e-3)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);


            IntPtr ptrCoords;
            Kernel.DiscreteQuantities.CgeomLaplacianSmoothingForCloseMesh(numVertices, numFaces, coords, faces, smoothing, numIterations, out ptrCoords);

            // Parse new vertex positions
            ParsePointerToMeshVertices(ptrCoords, numVertices * 3, ref mesh);
        }
    }
}
