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
        public static void PerVertexAsymptoticDirections(Mesh mesh, out Vector3d[] outVec1, out Vector3d[] outVec2, out int[] outIndexes)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords, outVanishingIdx, outErrorMsg;
            int outX1CoordsCount, outX2CoordsCount, outVanishingCount;
            int errorCode = Kernel.DiscreteQuantities.CgeomPerVertexAsymptoticDirections(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outVanishingCount, out outX1Coords, out outX2Coords, out outVanishingIdx, out outErrorMsg);

            if (errorCode == 0)
            {
                // Parse asymptotic directions
                outVec1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
                outVec2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
                outIndexes = Utils.ParsePointerToIntArr(outVanishingIdx, outVanishingCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }

        public static void PerFaceAsymptoticDirections(Mesh mesh, out Vector3d[] outVec1, out Vector3d[] outVec2, out int[] outIndexes)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outX1Coords, outX2Coords, outVanishingIdx, outErrorMsg;
            int outX1CoordsCount, outX2CoordsCount, outVanishingCount;
            int errorCode = Kernel.DiscreteQuantities.CgeomPerFaceAsymptoticDirections(numVertices, numFaces, coords, faces, out outX1CoordsCount, out outX2CoordsCount, out outVanishingCount, out outX1Coords, out outX2Coords, out outVanishingIdx, out outErrorMsg);

            if (errorCode == 0)
            {
                // Parse asymptotic directions
                outVec1 = Utils.ParsePointerToVectorArr(outX1Coords, outX1CoordsCount);
                outVec2 = Utils.ParsePointerToVectorArr(outX2Coords, outX2CoordsCount);
                outIndexes = Utils.ParsePointerToIntArr(outVanishingIdx, outVanishingCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
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

        public static Point3d[] Barycenters(Mesh mesh)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr outCoords;
            int outCoordsCount;
            Kernel.DiscreteQuantities.CgeomBarycenters(numVertices, numFaces, coords, faces, out outCoordsCount, out outCoords);

            return Utils.ParsePointerToPointArr(outCoords, outCoordsCount);
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

        public static void LocalBasis(Mesh mesh, out Vector3d[] B1, out Vector3d[] B2, out Vector3d[] B3)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            IntPtr ptrX1, ptrX2, ptrX3;
            int outCount;
            Kernel.DiscreteQuantities.CgeomLocalBasis(numVertices, numFaces, coords, faces, out outCount, out ptrX1, out ptrX2, out ptrX3);

            B1 = Utils.ParsePointerToVectorArr(ptrX1, outCount);
            B2 = Utils.ParsePointerToVectorArr(ptrX2, outCount);
            B3 = Utils.ParsePointerToVectorArr(ptrX3, outCount);
        }

        public static double[] ExactGeodesicDistances(Mesh mesh, List<Point3d> sourcePoints, List<Point3d> targetPoints, bool useFaces, double maximumDistance=0.1)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            // Find mesh closest source points
            HashSet<int> inSourceVertices = new HashSet<int>();
            HashSet<int> inSourceFaces = new HashSet<int>();
            for (int i=0; i<sourcePoints.Count; i++)
            {
                var cmp = mesh.ClosestMeshPoint(sourcePoints[i], maximumDistance);
                inSourceVertices.Add(cmp.ComponentIndex.Index);
                inSourceFaces.Add(cmp.FaceIndex);
            }

            // Find mesh closest target points
            HashSet<int> inTargetVertices = new HashSet<int>();
            HashSet<int> inTargetFaces = new HashSet<int>();
            for (int i = 0; i < targetPoints.Count; i++)
            {
                var cmp = mesh.ClosestMeshPoint(targetPoints[i], maximumDistance);
                inTargetVertices.Add(cmp.ComponentIndex.Index);
                inTargetFaces.Add(cmp.FaceIndex);
            }

            int[] sourceVertices, targetVertices, sourceFaces, targetFaces;
            if (useFaces)
            {
                sourceVertices = inSourceVertices.ToArray();
                targetVertices = inTargetVertices.ToArray();

                sourceFaces = new int[0];
                targetFaces = new int[0];
            }
            else
            {
                sourceVertices = new int[0];
                targetVertices = new int[0];

                sourceFaces = inSourceFaces.ToArray();
                targetFaces = inTargetFaces.ToArray();
            }

            int outDistancesCount;
            IntPtr outDistances, outErrorMsg;
            int errorCode = Kernel.DiscreteQuantities.CgeomExactDiscreteGeodesicDistances(numVertices, numFaces, sourceVertices.Length, sourceFaces.Length, targetVertices.Length, targetFaces.Length, coords, faces, sourceVertices, sourceFaces, targetVertices, targetFaces, out outDistancesCount, out outDistances, out outErrorMsg);

            if (errorCode == 0)
            {
                return Utils.ParsePointerToDoubleArr(outDistances, outDistancesCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }

        /// <summary>
        /// Compute fast approximate geodesic distances using precomputed data from a set of selected source vertices
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="t"> t “heat” parameter (smaller → more accurate, less stable) </param>
        /// <returns></returns>
        public static double[] HeatGeodesics(Mesh mesh, double t, Point3d sourcePoint, bool useIntrinsicDelaunay=true, double maximumDistance = 0.1)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            var cmp = mesh.ClosestMeshPoint(sourcePoint, maximumDistance);
            if (cmp == null) throw new Exception("Invalid source point.");

            int sourceVertex = cmp.ComponentIndex.Index;

            int outDistancesCount;
            IntPtr outDistances, outErrorMsg;
            int errorCode = Kernel.DiscreteQuantities.CgeomHeatGeodesics(numVertices, numFaces, coords, faces, t, sourceVertex, useIntrinsicDelaunay, out outDistancesCount, out outDistances, out outErrorMsg);

            if (errorCode == 0)
            {
                return Utils.ParsePointerToDoubleArr(outDistances, outDistancesCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }

        public static List<Polyline> ExactGeodesicPaths(Mesh mesh, Point3d sourcePoint, List<Point3d> targetPoints, double maximumDistance = 0.1)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces, false);

            // Find mesh closest source points
            int inSourceVertex = mesh.ClosestMeshPoint(sourcePoint, maximumDistance).ComponentIndex.Index;

            // Find mesh closest target points
            HashSet<int> inTargetVertices = new HashSet<int>();
            for (int i = 0; i < targetPoints.Count; i++)
            {
                inTargetVertices.Add(mesh.ClosestMeshPoint(targetPoints[i], maximumDistance).ComponentIndex.Index);
            }

            if (!inTargetVertices.Contains(inSourceVertex)) throw new Exception("Source and Target vertices are the same.");

            int[] sourceVertices, targetVertices;
            sourceVertices = new int[] { inSourceVertex };
            targetVertices = inTargetVertices.ToArray();
            int numSources = sourceVertices.Length;
            int numTargets = targetVertices.Length;

            int outCount;
            IntPtr outCoords, outCoordsSize, outErrorMsg;
            int errorCode = Kernel.DiscreteQuantities.CgeomGeodesicPaths(numVertices, numFaces, numSources, numTargets, coords, faces, sourceVertices, targetVertices, out outCount, out outCoords, out outCoordsSize, out outErrorMsg);

            if (errorCode == 0)
            {
                Point3d[] pts = Utils.ParsePointerToPointArr(outCoords, outCount, Utils.StorageOrder.RowMajor);
                int[] sizes = Utils.ParsePointerToIntArr(outCoordsSize, numTargets);

                List<Polyline> paths = new List<Polyline>();
                int idx = 0;
                for (int i=0; i<numTargets; i++)
                {
                    int count = sizes[i];
                    Point3d[] pp = new Point3d[count];
                    for (int j=0; j<count; j++)
                    {
                        pp[j] = pts[idx + j];
                    }
                    paths.Add(new Polyline(pp));
                    idx += count;
                }

                return paths;
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }
    }
}