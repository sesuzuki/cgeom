using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using CGeom.Wrappers;
using System.Runtime.InteropServices;

namespace CGeom.Tools
{
    public static class Analysis
    {
        public static void PrincipalCurvatures(IEnumerable<Point3d> vertices, IEnumerable<int[]> faces, out double[] outVal1, out double[] outVal2, out Vector3d[] outVec1, out Vector3d[] outVec2)
        {
            int numVertices = vertices.Count();
            int numFaces = faces.Count();

            double[] inCoords = new double[numVertices*3];
            for(int i=0; i<numVertices; i++)
            {
                // Column-major storage for Eigen::MatrixXd
                Point3d p = vertices.ElementAt(i);
                inCoords[i] = p.X;
                inCoords[numVertices + i] = p.Y;
                inCoords[numVertices * 2 + i] = p.Z;
            }

            int[] inFaces = new int[numFaces * 3];
            for (int i = 0; i < numFaces; i++)
            {
                // Column-major storage for Eigen::MatrixXd
                int[] f = faces.ElementAt(i);
                inFaces[i] = f[0];
                inFaces[numFaces + i] = f[1];
                inFaces[numFaces * 2 + i] = f[2];
            }

            IntPtr dir1, dir2, val1, val2;
            Kernel.Curvature.IglPrincipalCurvatures(numVertices, numFaces, inCoords, inFaces, out dir1, out dir2, out val1, out val2);

            // Parse principal curvatures
            outVal1 = new double[numVertices];
            outVal2 = new double[numVertices];
            Marshal.Copy(val1, outVal1, 0, numVertices);
            Marshal.Copy(val2, outVal2, 0, numVertices);
            Marshal.FreeCoTaskMem(val1);
            Marshal.FreeCoTaskMem(val2);

            // Parse principal curvature directions
            double[] outDir1 = new double[numVertices*3];
            double[] outDir2 = new double[numVertices*3];
            Marshal.Copy(dir1, outDir1, 0, numVertices * 3);
            Marshal.Copy(dir2, outDir2, 0, numVertices * 3);
            Marshal.FreeCoTaskMem(dir1);
            Marshal.FreeCoTaskMem(dir2);

            outVec1 = new Vector3d[numVertices];
            outVec2 = new Vector3d[numVertices];
            for(int i=0; i<numVertices; i++)
            {
                outVec1[i] = new Vector3d(outDir1[i*3], outDir1[i*3+1], outDir1[i*3+2]);
                outVec2[i] = new Vector3d(outDir2[i * 3], outDir2[i * 3 + 1], outDir2[i * 3 + 2]);
            }
        }
    }
}
