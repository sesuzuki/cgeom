using System;
using CGeom.Wrappers;
using Rhino.Geometry;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Rhino.Render.CustomRenderMeshes;
using Rhino.Geometry.Intersect;
using static Rhino.Render.CustomRenderMeshes.RenderMeshProvider;

namespace CGeom.Tools
{
    public static class Processing
    {
        public enum ForceFieldTypes { Gaussian, InverseDistances }

        public static Polyline[] FlipGeodesics(Mesh mesh, IEnumerable<Curve> polylines)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            Mesh copy = mesh.DuplicateMesh();
            PointCloud cloud = new PointCloud(copy.Vertices.ToPoint3dArray());
            Utils.ParseTriangleRhinoMesh(copy, out coords, out faces, out numVertices, out numFaces);

            IntPtr outPointCoords, outPointOffsets;
            int outNumCoords, outNumOffsets;

            int numPointOffsets = polylines.Count();
            int[] inPointOffsets = new int[numPointOffsets];
            List<int> inPtsIndices = new List<int>();
            int offset = 0;

            for (int i = 0; i < numPointOffsets; i++)
            {
                Polyline poly;
                bool flag = polylines.ElementAt(i).TryGetPolyline(out poly);
                if (!flag) throw new Exception("Input curve should be a polyline");

                foreach (var p in poly) inPtsIndices.Add(cloud.ClosestPoint(p));
                offset += poly.Count;

                inPointOffsets[i] = offset;
            }

            Kernel.GeometryCentral.CgeomGetFlipGeodesics(numVertices, numFaces, numPointOffsets, coords, faces, inPtsIndices.ToArray(), inPointOffsets, out outPointCoords, out outPointOffsets, out outNumCoords, out outNumOffsets);

            double[] outCoords = new double[outNumCoords];
            Marshal.Copy(outPointCoords, outCoords, 0, outNumCoords);
            Marshal.FreeCoTaskMem(outPointCoords);

            int[] outOffsets = new int[outNumOffsets];
            Marshal.Copy(outPointOffsets, outOffsets, 0, outNumOffsets);
            Marshal.FreeCoTaskMem(outPointOffsets);

            Polyline[] geodesics = new Polyline[outNumOffsets];
            int startOffset = 0, endOffset;
            for (int i = 0; i < outNumOffsets; i++)
            {
                endOffset = outOffsets[i];
                int numPts = (int)(endOffset - startOffset) / 3;

                Point3d[] pts = new Point3d[numPts];
                for (int j = 0; j < numPts; j++) pts[j] = new Point3d(outCoords[startOffset + j * 3], outCoords[startOffset + j * 3 + 1], outCoords[startOffset + j * 3 + 2]);
                geodesics[i] = new Polyline(pts);
                startOffset = endOffset;
            }

            return geodesics;
        }

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

        public static void RepulsivePointsWithGaussianPotentials(IEnumerable<Point3d> pts, IEnumerable<Point3d> fixedPoints, IEnumerable<double> AList, IEnumerable<double> sigmaList, out Point3d[] positions, out Vector3d[] forces, out double[] potentials, double dt= 1.0, int numIterations=100)
        {
            // A is a scaling factor determining the strength of the interaction.
            // sigma is the width parameter controlling the range of the interaction

            int numPts = pts.Count();
            int numFixed = fixedPoints.Count();
            positions = new Point3d[numPts];
            forces = new Vector3d[numPts];
            potentials = new double[numPts];
            bool flagA = AList.Count() == numFixed ? true : false;
            bool flagSigma = sigmaList.Count() == numFixed ? true : false;

            for (int i=0; i<numPts; i++)
            {
                var p = pts.ElementAt(i);
                var f = new Vector3d(0, 0, 0);
                double pot = 0;
                for (int j=0; j<numFixed; j++)
                {
                    var pf = fixedPoints.ElementAt(j);
                    double A = flagA ? AList.ElementAt(j) : AList.ElementAt(0);
                    double sigma = flagSigma ? sigmaList.ElementAt(j) : sigmaList.ElementAt(0);

                    // gradient 
                    double dist = p.DistanceTo(pf);
                    Vector3d gradient = A * (p - pf) * Math.Exp(-Math.Pow(dist, 2) / (2 * Math.Pow(sigma, 2))) / Math.Pow(sigma, 2);
                    // Force is the negative gradient of the potential
                    f -= gradient;

                    double distanceSq = Math.Pow(p.DistanceTo(pf), 2);
                    pot += A * Math.Exp(-distanceSq / (2 * Math.Pow(sigma, 2)));
                }

                positions[i] = p;
                forces[i] = f;
                potentials[i] = pot;
            }

            // Euler integration to update positions
            for (int iter = 0; iter < numIterations; iter++)
            {
                for (int i = 0; i < numPts; i++) positions[i] += forces[i] * dt;
            }
        }

        public static void RepulsivePointsWithInverseDistances(IEnumerable<Point3d> pts, IEnumerable<Point3d> fixedPoints, IEnumerable<double> AList, out Point3d[] positions, out Vector3d[] forces, out double[] potentials, double dt = 1.0, int numIterations = 100)
        {
            // A is a scaling factor determining the strength of the interaction.

            int numPts = pts.Count();
            int numFixed = fixedPoints.Count();
            positions = new Point3d[numPts];
            forces = new Vector3d[numPts];
            potentials = new double[numPts];
            bool flagA = AList.Count() == numFixed ? true : false;

            for (int i = 0; i < numPts; i++)
            {
                var p = pts.ElementAt(i);
                var direction = new Vector3d(0, 0, 0);
                double distanceSq = 0;
                for (int j = 0; j < numFixed; j++)
                {
                    double A = flagA ? AList.ElementAt(j) : AList.ElementAt(0);
                    var pf = fixedPoints.ElementAt(j);

                    var dist = p.DistanceTo(pf);
                    direction += (p - pf) / Math.Sqrt(Math.Pow(dist, 2));
                    distanceSq += A * Math.Pow(dist, 2);
                }

                positions[i] = p;
                potentials[i] = 1 / Math.Sqrt(distanceSq);
                forces[i] = direction * potentials[i];
            }

            // Euler integration to update positions
            for (int iter = 0; iter < numIterations; iter++)
            {
                for (int i = 0; i < numPts; i++) positions[i] += forces[i] * dt;
            }
        }
    }
}
