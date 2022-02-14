using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using CGeom.Wrappers;
using System.Runtime.InteropServices;
using CGeom.Tools;

namespace CGeom.Tools
{
    public static class DiscreteQuantities
    {
        public static void PerVertexAsymptoticDirections(Mesh mesh, out Vector3d[] outVec1, out Vector3d[] outVec2)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords;
            int outX1CoordsCount, outX2CoordsCount;
            Kernel.DiscreteQuantities.CgeomPerVertexAsymptoticDirections(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outX1Coords, out outX2Coords);

            // Parse asymptotic directions
            outVec1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
            outVec2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
        }

        public static void PerFaceAsymptoticDirections(Mesh mesh, out Vector3d[] outVec1, out Vector3d[] outVec2)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords;
            int outX1CoordsCount, outX2CoordsCount;
            Kernel.DiscreteQuantities.CgeomPerFaceAsymptoticDirections(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outX1Coords, out outX2Coords);

            // Parse asymptotic directions
            outVec1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
            outVec2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
        }

        public static void PerVertexPrincipalCurvatures(Mesh mesh, out double[] K1, out double[] K2, out Vector3d[] X1, out Vector3d[] X2)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords, outK1, outK2;
            int outX1CoordsCount, outX2CoordsCount, outK1Count, outK2Count; 
            Kernel.DiscreteQuantities.CgeomPerVertexPrincipalCurvatures(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outK1Count, out outK2Count, out outX1Coords, out outX2Coords, out outK1, out outK2);

            // Parse principal curvatures
            K1 = Utils.ParsePointerToDoubleArr(outK1, outK1Count);
            K2 = Utils.ParsePointerToDoubleArr(outK2, outK2Count);
            X1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
            X2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
        }

        public static void PerFacePrincipalCurvatures(Mesh mesh, out double[] K1, out double[] K2, out Vector3d[] X1, out Vector3d[] X2)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords, outK1, outK2;
            int outX1CoordsCount, outX2CoordsCount, outK1Count, outK2Count;
            Kernel.DiscreteQuantities.CgeomPerFacePrincipalCurvatures(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outK1Count, out outK2Count, out outX1Coords, out outX2Coords, out outK1, out outK2);

            // Parse principal curvatures
            K1 = Utils.ParsePointerToDoubleArr(outK1, outK1Count);
            K2 = Utils.ParsePointerToDoubleArr(outK2, outK2Count);
            X1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
            X2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
        }

        public static Vector3d[] PerVertexNormals(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

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
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

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
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

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
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

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
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr h;
            Kernel.DiscreteQuantities.CgeomMeanCurvature(numVertices, numFaces, coords, faces, out h);

            double[] H = new double[numVertices];
            Marshal.Copy(h, H, 0, numVertices);
            Marshal.FreeCoTaskMem(h);

            return H;
        }
    }
}