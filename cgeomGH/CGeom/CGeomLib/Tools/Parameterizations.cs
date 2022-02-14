using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CGeom.Wrappers;
using Rhino.Geometry;
using static CGeom.Tools.Utils;
using System.Linq;

namespace CGeom.Tools
{
    public static class Parameterizations
    {
        public static void BuildNRosy(Mesh mesh, IEnumerable<int> constraintFaces, IEnumerable<Vector3d> constraintVectorFaces, out Vector3d[] x1, out Vector3d[] x2, out Vector3d[] barycenters, out double[] singularities, int degree=4)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            double[] vectorConstraint = FlattenVector3dData(constraintVectorFaces.ToArray(), StorageOrder.ColumnMajor);
            IntPtr ptrX1, ptrX2, ptrB, ptrS;
            Kernel.Parameterization.CgeomNRosy(numVertices, numFaces, constraintFaces.Count(), coords, faces, constraintFaces.ToArray(), vectorConstraint, degree, out ptrX1, out ptrX2, out ptrB, out ptrS);

            x1 = ParsePointerToVectorArr(ptrX1, numFaces*3);
            x2 = ParsePointerToVectorArr(ptrX2, numFaces*3);
            barycenters = ParsePointerToVectorArr(ptrB, numFaces*3);
            singularities = ParsePointerToDoubleArr(ptrS, numVertices*3);
        }

        public static void BuildSeamlessIntegerParameterization(Mesh mesh, IEnumerable<Vector3d> X1, IEnumerable<Vector3d> X2, double gradient_size, double stiffness, bool direct_round, int numIterations, out Vector3d[] UV, out MeshFace[] FUV)
        {
            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            ParseRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            if(X1.Count()!=X2.Count()) throw new ArgumentException("Incorrect number of vectors (X1 and X2) for defining the cross-fields.");
            if(numFaces != X1.Count() || numFaces != X2.Count()) throw new ArgumentException("The number of cross-fields doesn't match the number of faces.");

            double[] inCoordsX1 = FlattenVector3dData(X1, StorageOrder.ColumnMajor);
            double[] inCoordsX2 = FlattenVector3dData(X2, StorageOrder.ColumnMajor);

            int numUV, numFUV;
            IntPtr ptrUV, ptrFUV;
            Kernel.Parameterization.CgeomSeamlessIntegerGridParameterization(numVertices, numFaces, coords, faces, inCoordsX1, inCoordsX2, gradient_size, stiffness, direct_round, numIterations, out numUV, out numFUV, out ptrUV, out ptrFUV);

            UV = ParsePointerToVectorArr(ptrUV, numUV, false);
            FUV = ParsePointerToMeshFaceArr(ptrFUV, numFUV);
        }

        public static Mesh QuadMeshExtraction(Mesh mesh, IEnumerable<Vector3d> UV, IEnumerable<MeshFace> FUV)
        {
            // Parse mesh data
            double[] inCoords;
            int[] inTrias;
            int inVertexCount, inTriasCount;
            ParseRhinoMesh(mesh, out inCoords, out inTrias, out inVertexCount, out inTriasCount);

            double[] inUV = FlattenVector3dData(UV);
            int[] inFUV = FlattenMeshFaceData(FUV);

            int outVertexCount, outQuadsCount;
            IntPtr ptrCoords, ptrQuads;
            Kernel.Parameterization.CgeomQuadMeshExtraction(inVertexCount, inTriasCount, inCoords, inTrias, inUV, inFUV, out outVertexCount, out outQuadsCount, out ptrCoords, out ptrQuads);

            double[] outCoords = ParsePointerToDoubleArr(ptrCoords, outVertexCount);
            int[] outQuads = ParsePointerToIntArr(ptrQuads, outQuadsCount);

            return Utils.GetQuadMesh(outCoords, outQuads);
        }
    }
}
