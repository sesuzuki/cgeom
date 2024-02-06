using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CGeom.Wrappers;
using Rhino.Geometry;
using static CGeom.Tools.Utils;
using System.Linq;

namespace CGeom.Tools
{
    public static class Parametrizations
    {
        public static void BuildSeamlessIntegerParameterization(Mesh mesh, IEnumerable<Vector3d> X1, IEnumerable<Vector3d> X2, double gradient_size, double stiffness, bool direct_round, int numIterations, out Vector3d[] UV, out MeshFace[] FUV)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            if(X1.Count()!=X2.Count()) throw new ArgumentException("Incorrect number of vectors (X1 and X2) for defining the cross-fields.");
            if(numFaces != X1.Count() || numFaces != X2.Count()) throw new ArgumentException("The number of cross-fields doesn't match the number of faces.");

            double[] inCoordsX1 = FlattenVector3dData(X1, StorageOrder.ColumnMajor);
            double[] inCoordsX2 = FlattenVector3dData(X2, StorageOrder.ColumnMajor);

            int numUV, numFUV;
            IntPtr ptrUV, ptrFUV;
            Kernel.Processing.CgeomSeamlessIntegerGridParameterization(numVertices, numFaces, coords, faces, inCoordsX1, inCoordsX2, gradient_size, stiffness, direct_round, numIterations, out numUV, out numFUV, out ptrUV, out ptrFUV);

            UV = ParsePointerToVectorArr(ptrUV, numUV, false);
            FUV = ParsePointerToMeshFaceArr(ptrFUV, numFUV);
        }

        public static Mesh QuadMeshExtraction(Mesh mesh, IEnumerable<Vector3d> UV, IEnumerable<MeshFace> FUV)
        {
            // Parse mesh data
            double[] inCoords;
            int[] inTrias;
            int inVertexCount, inTriasCount;
            ParseTriangleRhinoMesh(mesh, out inCoords, out inTrias, out inVertexCount, out inTriasCount);

            double[] inUV = FlattenVector3dData(UV);
            int[] inFUV = FlattenMeshFaceData(FUV);
            int inUVCount = inUV.Length / 3;
            int inFUVCount = inFUV.Length / 3;

            int outVertexCount, outQuadsCount;
            IntPtr ptrCoords, ptrQuads;
            Kernel.Processing.CgeomQuadMeshExtraction(inVertexCount, inTriasCount, inUVCount, inFUVCount, inCoords, inTrias, inUV, inFUV, out outVertexCount, out outQuadsCount, out ptrCoords, out ptrQuads);

            double[] outCoords = ParsePointerToDoubleArr(ptrCoords, outVertexCount);
            int[] outQuads = ParsePointerToIntArr(ptrQuads, outQuadsCount);

            return Utils.GetQuadMesh(outCoords, outQuads);
        }

        public static Mesh Planarization(Mesh mesh, int iterations, out double[] planarity, double threshold = 1e-3)
        {
            double[] inCoords;
            int[] inQuads;
            int inVertexCount, inQuadsCount;
            ParseQuadRhinoMesh(mesh, out inCoords, out inQuads, out inVertexCount, out inQuadsCount);

            int outVertexCount, outPlanarityCount;
            IntPtr ptrCoords, ptrPlanarity;
            Kernel.Processing.CgeomPlanarization(inVertexCount, inQuadsCount, inCoords, inQuads, iterations, threshold, out outVertexCount, out outPlanarityCount, out ptrCoords, out ptrPlanarity);

            Mesh pm = mesh.DuplicateMesh();
            ParsePointerToMeshVertices(ptrCoords, outVertexCount, ref pm);
            planarity = ParsePointerToDoubleArr(ptrPlanarity, outPlanarityCount);

            return pm;
        }

        public static void HarmonicParametrization(Mesh mesh, double scaleFactor, out Mesh mesh3D, out Mesh mesh2D)
        {
            // Parse mesh data
            double[] inCoords;
            int[] inFaces;
            int numVertices, numFaces;
            ParseTriangleRhinoMesh(mesh, out inCoords, out inFaces, out numVertices, out numFaces);

            int numUV;
            IntPtr ptrUV, outErrorMsg;
            Kernel.Processing.CgeomHarmonicParametrization(numVertices, numFaces, inCoords, inFaces, out numUV, out ptrUV, out outErrorMsg);

            Point2f[] UV = ParsePointerToPoint2fArr(ptrUV, numUV);

            // 3d mesh with texture coordinates
            mesh3D = new Mesh();
            mesh3D.Vertices.AddVertices(mesh.Vertices);
            mesh3D.TextureCoordinates.AddRange(UV);
            mesh3D.Faces.AddFaces(mesh.Faces);
            mesh3D.UnifyNormals();
            mesh3D.Normals.ComputeNormals();

            // 2d mesh with texture coordinates
            mesh2D = new Mesh();
            mesh2D.Vertices.AddVertices(UV.Select( p => new Point3d(p.X, p.Y, 0)));
            mesh2D.Faces.AddFaces(mesh.Faces);
            mesh2D.UnifyNormals();
            mesh2D.Normals.ComputeNormals();
            mesh2D.Scale(scaleFactor);
        }

        public static void LeastSquaresConformalMaps(Mesh mesh, double scaleFactor, out Mesh mesh3D, out Mesh mesh2D)
        {
            // Parse mesh data
            double[] inCoords;
            int[] inFaces;
            int numVertices, numFaces;
            ParseTriangleRhinoMesh(mesh, out inCoords, out inFaces, out numVertices, out numFaces);

            int numUV;
            IntPtr ptrUV, outErrorMsg;
            Kernel.Processing.CgeomLeastSquaresConformalMaps(numVertices, numFaces, inCoords, inFaces, out numUV, out ptrUV, out outErrorMsg);

            Point2f[] UV = ParsePointerToPoint2fArr(ptrUV, numUV);

            // 3d mesh with texture coordinates
            mesh3D = new Mesh();
            mesh3D.Vertices.AddVertices(mesh.Vertices);
            mesh3D.TextureCoordinates.AddRange(UV);
            mesh3D.Faces.AddFaces(mesh.Faces);
            mesh3D.UnifyNormals();
            mesh3D.Normals.ComputeNormals();

            // 2d mesh with texture coordinates
            mesh2D = new Mesh();
            mesh2D.Vertices.AddVertices(UV.Select(p => new Point3d(p.X, p.Y, 0)));
            mesh2D.Faces.AddFaces(mesh.Faces);
            mesh2D.UnifyNormals();
            mesh2D.Normals.ComputeNormals();
            mesh2D.Scale(scaleFactor);
        }
    }
}
