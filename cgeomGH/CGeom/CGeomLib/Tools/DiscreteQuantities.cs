using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using CGeom.Wrappers;
using System.Runtime.InteropServices;

namespace CGeom.Tools
{
    public static class DiscreteQuantities
    {
        public static void PerVertexPrincipalCurvatures(Mesh mesh, out double[] outVal1, out double[] outVal2, out Vector3d[] outVec1, out Vector3d[] outVec2)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr dir1, dir2, val1, val2;
            Kernel.DiscreteQuantities.CgeomPrincipalCurvatures(numVertices, numFaces, coords, faces, out dir1, out dir2, out val1, out val2);

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

        public static void PerFacePrincipalCurvatures(Mesh mesh, out double[] outVal1, out double[] outVal2, out Vector3d[] outVec1, out Vector3d[] outVec2)
        {
            double[] vval1, vval2;
            Vector3d[] vdir1, vdir2;
            DiscreteQuantities.PerVertexPrincipalCurvatures(mesh, out vval1, out vval2, out vdir1, out vdir2);

            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            outVal1 = new double[numFaces];
            outVal2 = new double[numFaces];
            outVec1 = new Vector3d[numFaces];
            outVec2 = new Vector3d[numFaces];

            for(int i=0; i<numFaces; i++)
            {
                outVec1[i] = new Vector3d();
                outVec2[i] = new Vector3d();
                outVal1[i] = 0;
                outVal2[i] = 0;

                for(int j=0; j<3; j++)
                {
                    int idx = faces[i*3+j];
                    outVec1[i] += vdir1[idx];
                    outVec2[i] += vdir2[idx];
                    outVal1[i] += vval1[idx];
                    outVal2[i] += vval2[idx];
                }

                outVal1[i] /= 3;
                outVal2[i] /= 3;
                outVec1[i] /= 3;
                outVec2[i] /= 3;

                outVec1[i].Unitize();
                outVec2[i].Unitize();
            }
        }

        public static Vector3d[] PerVertexNormals(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr n;
            Kernel.DiscreteQuantities.CgeomNormalsPerVertex(numVertices, numFaces, coords, faces, out n);

            double[] outNorm = new double[numVertices * 3];
            Marshal.Copy(n, outNorm, 0, numVertices * 3);
            Marshal.FreeCoTaskMem(n);

            Vector3d[] N = new Vector3d[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                N[i] = new Vector3d(outNorm[i * 3], outNorm[i * 3 + 1], outNorm[i * 3 + 2]);
            }

            return N;
        }

        public static Vector3d[] PerFaceNormals(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr n;
            Kernel.DiscreteQuantities.CgeomNormalsPerFace(numVertices, numFaces, coords, faces, out n);

            double[] outNorm = new double[numFaces * 3];
            Marshal.Copy(n, outNorm, 0, numFaces * 3);
            Marshal.FreeCoTaskMem(n);

            Vector3d[] N = new Vector3d[numFaces];
            for (int i = 0; i < numFaces; i++)
            {
                N[i] = new Vector3d(outNorm[i * 3], outNorm[i * 3 + 1], outNorm[i * 3 + 2]);
            }

            return N;
        }

        /// <summary>
        /// Corner normals as averages of normals of faces incident on the corresponding vertex
        /// which do not deviate by more than a specified dihedral angle (in degrees)
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="angle"> Angle in degrees. </param>
        /// <returns></returns>
        public static Vector3d[] PerCornerNormals(Mesh mesh, double angle=20)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr n;
            Kernel.DiscreteQuantities.CgeomNormalsPerCorner(numVertices, numFaces, coords, faces, angle, out n);

            double[] outNorm = new double[numVertices * 3];
            Marshal.Copy(n, outNorm, 0, numVertices * 3);
            Marshal.FreeCoTaskMem(n);

            Vector3d[] N = new Vector3d[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                N[i] = new Vector3d(outNorm[i * 3], outNorm[i * 3 + 1], outNorm[i * 3 + 2]);
            }

            return N;
        }

        /// <summary>
        /// Gaussian curvature defined as the product of the principal curvatures: kG=k1k2.
        /// Gaussian curvature tells how locally spherical or elliptic the surface is (kG>0),
        /// how locally saddle-shaped or hyperbolic the surface is (kG<0 ),
        /// or how locally cylindrical or parabolic (kG=0 ) the surface is.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double[] GaussianCurvature(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr k;
            Kernel.DiscreteQuantities.CgeomGaussianCurvature(numVertices, numFaces, coords, faces, out k);

            double[] K = new double[numVertices];
            Marshal.Copy(k, K, 0, numVertices);
            Marshal.FreeCoTaskMem(k);

            return K;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double[] MeanCurvature(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr h;
            Kernel.DiscreteQuantities.CgeomMeanCurvature(numVertices, numFaces, coords, faces, out h);

            double[] H = new double[numVertices];
            Marshal.Copy(h, H, 0, numVertices);
            Marshal.FreeCoTaskMem(h);

            return H;
        }
    }
}