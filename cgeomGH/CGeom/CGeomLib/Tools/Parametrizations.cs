using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CGeom.Wrappers;
using Rhino.Geometry;
using static CGeom.Tools.Utils;
using System.Linq;
using Eto.Forms;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry.Intersect;

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

        public static void CreateCheckerboardOnDisk(Mesh disk, Mesh initialMesh, int uCount, int vCount, double angle, double length, out Curve[][] crvOnDisk, out Curve[][] crvOnMesh3d)
        {
            // Create texture on disk
            Circle circle;
            Circle.TryFitCircleToPoints(disk.GetNakedEdges()[0], out circle);
            var crv = circle.ToNurbsCurve();

            var dom = new Interval(-circle.Radius, circle.Radius);
            var pl = circle.Plane;
            pl.Rotate(angle, pl.ZAxis);

            double uStep = dom.Length / (uCount - 1);
            double vStep = dom.Length / (vCount - 1);
            List<PolylineCurve> polyA = new List<PolylineCurve>();
            List<PolylineCurve> polyB = new List<PolylineCurve>();

            for (int i = 0; i < uCount; i++)
            {
                for (int j = 0; j < vCount; j++)
                {

                    if (i > 0 && i < uCount - 1 && j < vCount - 1)
                    {
                        Point3d p0 = pl.PointAt(dom.T0 + uStep * i, dom.T0 + vStep * j, 0);
                        Point3d p1 = pl.PointAt(dom.T0 + uStep * i, dom.T0 + vStep * (j + 1), 0);
                        var e = new LineCurve(p0, p1);

                        var inter = Intersection.CurveCurve(crv, e, 1e-6, 1e-6);

                        if (inter.Count == 1)
                        {
                            e = new LineCurve(crv.Contains(p0, pl, 1e-3) == PointContainment.Inside ? p0 : inter[0].PointA, crv.Contains(p0, pl, 1e-3) == PointContainment.Inside ? inter[0].PointA : p1);
                            int count = (int)(e.GetLength() / length);
                            polyA.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                        else if (crv.Contains(p0, pl, 1e-3) == PointContainment.Inside && crv.Contains(p1, pl, 1e-3) == PointContainment.Inside)
                        {
                            e = new LineCurve(p0, p1);
                            int count = (int)(e.GetLength() / length);
                            polyA.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                    }

                    if (i < uCount - 1 && j > 0 && j < vCount - 1)
                    {
                        Point3d p0 = pl.PointAt(dom.T0 + uStep * i, dom.T0 + vStep * j, 0);
                        Point3d p1 = pl.PointAt(dom.T0 + uStep * (i + 1), dom.T0 + vStep * j, 0);
                        var e = new LineCurve(p0, p1);
                        var inter = Intersection.CurveCurve(crv, e, 1e-6, 1e-6);

                        if (inter.Count == 1)
                        {
                            e = new LineCurve(crv.Contains(p0, pl, 1e-3) == PointContainment.Inside ? p0 : inter[0].PointA, crv.Contains(p0, pl, 1e-3) == PointContainment.Inside ? inter[0].PointA : p1);
                            int count = (int)(e.GetLength() / length);
                            polyB.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                        else if (crv.Contains(p0, pl, 1e-3) == PointContainment.Inside && crv.Contains(p1, pl, 1e-3) == PointContainment.Inside)
                        {
                            e = new LineCurve(p0, p1);
                            int count = (int)(e.GetLength() / length);
                            polyB.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                    }
                }
            }

            crvOnDisk = new Curve[2][];
            crvOnDisk[0] = Curve.JoinCurves(polyA);
            crvOnDisk[1] = Curve.JoinCurves(polyB);


            // Map disk texture to 3d mesh
            crvOnMesh3d = new Curve[2][];
            for (int i = 0; i < 2; i++)
            {
                int count = crvOnDisk[i].Count();
                crvOnMesh3d[i] = new Curve[count];
                for (int j = 0; j < count; j++)
                {
                    Polyline poly;
                    crvOnDisk[i][j].TryGetPolyline(out poly);

                    List<Point3d> tempP = new List<Point3d>();
                    foreach (Point3d p in poly) tempP.Add(initialMesh.PointAt(disk.ClosestMeshPoint(p, 0.0)));

                    crvOnMesh3d[i][j] = new PolylineCurve(tempP);
                }
            }
        }
    }
}
