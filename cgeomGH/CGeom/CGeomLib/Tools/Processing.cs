using System;
using CGeom.Wrappers;
using Rhino.Geometry;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CGeom.Tools
{
    public static class Processing
    {
        public static void LaplacianSmoothingForOpenMesh(int numIterations, ref Mesh mesh, IEnumerable<Point3d> anchors, double tolerance = 1e-3)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Mesh copy = mesh.DuplicateMesh();
            Utils.ParseTriangleRhinoMesh(copy, out coords, out faces, out numVertices, out numFaces);

            List<int> indexes = Enumerable.Range(0, numVertices).ToList();
            HashSet<int> inBoundaries = new HashSet<int>();

            PointCloud cloud = new PointCloud(copy.Vertices.ToPoint3dArray());

            for (int i = 0; i < anchors.Count(); i++)
            {
                var p1 = anchors.ElementAt(i);
                int idx = cloud.ClosestPoint(p1);
                var p2 = cloud[idx].Location;

                if (p1.DistanceTo(p2) <= tolerance) inBoundaries.Add(idx);
            }

            int[] inInteriors = indexes.Except(inBoundaries).ToArray();

            IntPtr outCoords;
            int outCoordsCount;
            Kernel.DiscreteQuantities.CgeomLaplacianSmoothingForOpenMesh(numVertices, numFaces, inBoundaries.Count, coords, faces, inBoundaries.ToArray(), inInteriors, numIterations, out outCoordsCount, out outCoords);

            // Parse new vertex positions
            Utils.ParsePointerToMeshVertices(outCoords, outCoordsCount, ref mesh);
        }

        public static void LaplacianSmoothingForCloseMesh(int numIterations, ref Mesh mesh, double smoothing, double tolerance = 1e-3)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Mesh copy = mesh.DuplicateMesh();
            Utils.ParseTriangleRhinoMesh(copy, out coords, out faces, out numVertices, out numFaces);


            IntPtr outCoords;
            int outCoordsCount;
            Kernel.DiscreteQuantities.CgeomLaplacianSmoothingForCloseMesh(numVertices, numFaces, coords, faces, smoothing, numIterations, out outCoordsCount, out outCoords);

            // Parse new vertex positions
            Utils.ParsePointerToMeshVertices(outCoords, outCoordsCount, ref mesh);
        }

        public static void RotateVectors(IEnumerable<Vector3d> X1, IEnumerable<double> angles, IEnumerable<Vector3d> B1, IEnumerable<Vector3d> B2, out Vector3d[] X2)
        {
            double[] inX1Coords = Utils.FlattenVector3dData(X1);
            double[] inB1Coords = Utils.FlattenVector3dData(B1);
            double[] inB2Coords = Utils.FlattenVector3dData(B2);
            int numVectors = X1.Count();

            double[] inAngle = angles.ToArray();

            IntPtr ptrX1;
            int outCount;
            Kernel.Processing.CgeomRotateVectors(numVectors, inX1Coords, inB1Coords, inB2Coords, inAngle, out outCount, out ptrX1);

            X2 = Utils.ParsePointerToVectorArr(ptrX1, outCount);
        }

        public static void ParallelTransport(Mesh mesh, int sourceIndex, double parallelParam, double perpendicualrParam, out Point3d[] points, out Vector3d[] vectors)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            int outCount;
            IntPtr outVecCoords, outCoords, outErrorMsg;
            int errorCode = Kernel.Processing.CgeomParallelTransport(numVertices, numFaces, coords, faces, sourceIndex, parallelParam, perpendicualrParam, out outCount, out outCoords, out outVecCoords, out outErrorMsg);

            if (errorCode == 0)
            {
                vectors = Utils.ParsePointerToVectorArr(outVecCoords, outCount);
                points = Utils.ParsePointerToPoint3DArr(outCoords, outCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }

        public static void EdgeVectors(Mesh mesh, out Point3d[] midPoints, out Vector3d[] parallelVectors, out Vector3d[] perpendicularVectors)
        {
            int numVertices, numFaces;
            double[] coords;
            int[] faces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            int outCount;
            IntPtr outEdgeMidCoords, outParCoords, outPerpCoords,outErrorMsg;
            int errorCode = Kernel.Processing.CgeomEdgeVectors(numVertices, numFaces, coords, faces, out outCount, out outEdgeMidCoords, out outParCoords, out outPerpCoords, out outErrorMsg);

            if (errorCode == 0)
            {
                midPoints = Utils.ParsePointerToPoint3DArr(outEdgeMidCoords, outCount);
                parallelVectors = Utils.ParsePointerToVectorArr(outParCoords, outCount);
                perpendicularVectors = Utils.ParsePointerToVectorArr(outPerpCoords, outCount);
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }

        public static Mesh RemeshAlongIsoline(Mesh mesh, double[] scalarField, double isoValue)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Utils.ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            int outNumVertices, outNumFaces;
            IntPtr outCoords, outFaces, outErrorMsg;
            int errorCode = Kernel.Processing.CgeomRemeshAlongIsoline(numVertices, numFaces, coords, faces, scalarField, isoValue, out outNumVertices, out outNumFaces, out outCoords, out outFaces, out outErrorMsg);

            if (errorCode == 0)
            {
                Point3d[] outV = Utils.ParsePointerToPoint3DArr(outCoords, outNumVertices, Utils.StorageOrder.RowMajor);
                MeshFace[] outF = Utils.ParsePointerToMeshFaceArr(outFaces, outNumFaces, Utils.StorageOrder.RowMajor);

                Mesh outMesh = new Mesh();
                outMesh.Vertices.AddVertices(outV);
                outMesh.Faces.AddFaces(outF);
                outMesh.Vertices.CombineIdentical(true, true);
                outMesh.Faces.CullDegenerateFaces();
                outMesh.Normals.ComputeNormals();

                return outMesh;
            }
            else
            {
                string errorMsg = Marshal.PtrToStringAnsi(outErrorMsg);
                throw new Exception(errorMsg);
            }
        }
    }
}
