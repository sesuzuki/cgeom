using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using System.Runtime.InteropServices;

namespace CGeom.Tools
{
    public static class Utils
    {
        public enum StorageOrder {RowMajor=0, ColumnMajor=1};

        public static void ParseTriangleRhinoMesh(Mesh mesh, out double[] coords, out int[] faces, out int numVertices, out int numFaces)
        {
            coords = Utils.FlattenPoint3dData(mesh.Vertices.ToPoint3dArray(), Utils.StorageOrder.ColumnMajor);
            faces = Utils.FlattenTriaFaceData(mesh, Utils.StorageOrder.ColumnMajor);

            numVertices = coords.Count() / 3;
            numFaces = faces.Count() / 3;
        }

        public static void ParseQuadRhinoMesh(Mesh mesh, out double[] coords, out int[] faces, out int numVertices, out int numFaces)
        {
            coords = Utils.FlattenPoint3dData(mesh.Vertices.ToPoint3dArray(), Utils.StorageOrder.ColumnMajor);
            faces = Utils.FlattenQuadFaceData(mesh, Utils.StorageOrder.ColumnMajor);

            numVertices = coords.Count() / 3;
            numFaces = faces.Count() / 4;
        }

        /// <summary>
        /// Flatten a collection of rhino points into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="pts"> Collection of rhino points. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static double[] FlattenPoint3dData(IEnumerable<Point3d> pts, StorageOrder order = StorageOrder.ColumnMajor)
        {
            double[,] data = new double[pts.Count(), 3];
            for (int i = 0; i < pts.Count(); i++)
            {
                var v = pts.ElementAt(i);
                data[i, 0] = v.X;
                data[i, 1] = v.Y;
                data[i, 2] = v.Z;
            }

            return FlattenDoubleData(data, order);
        }

        /// <summary>
        /// Flatten a multidimensional array into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="data"> Multidimensional array. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static double[] FlattenDoubleData(double[,] data, StorageOrder order)
        {
            int row = data.GetLength(0);
            int column = data.GetLength(1);

            double[] outData = new double[row * column];

            if (order == StorageOrder.ColumnMajor)
            {
                for (int i = 0; i < column; i++)
                {
                    for (int j = 0; j < row; j++)
                    {
                        outData[i * row + j] = data[j, i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        outData[i * column + j] = data[i, j];
                    }
                }
            }
            return outData;
        }

        /// <summary>
        /// Flatten a multidimensional array into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="data"> Multidimensional array. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static int[] FlattenIntData(int[,] data, StorageOrder order = StorageOrder.ColumnMajor)
        {
            int row = data.GetLength(0);
            int column = data.GetLength(1);

            int[] outData = new int[row * column];

            if (order == StorageOrder.ColumnMajor)
            {
                for (int i = 0; i < column; i++)
                {
                    for (int j = 0; j < row; j++)
                    {
                        outData[i * row + j] = data[j, i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        outData[i * column + j] = data[i, j];
                    }
                }
            }

            return outData;
        }

        /// <summary>
        /// Flatten a triangular mesh faces into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static int[] FlattenTriaFaceData(Mesh m, StorageOrder order)
        {
            m.Faces.ConvertQuadsToTriangles();
            int row = m.Faces.TriangleCount;

            int[,] faces = new int[row, 3];
            for (int i = 0; i < m.Faces.Count; i++)
            {
                MeshFace f = m.Faces[i];
                faces[i, 0] = f.A;
                faces[i, 1] = f.B;
                faces[i, 2] = f.C;
            }

            return FlattenIntData(faces, order);
        }

        public static int[] FlattenQuadFaceData(Mesh m, StorageOrder order)
        {
            if (m.Faces.TriangleCount > 0) throw new Exception("The mesh contains triangular faces.");

            int rows = m.Faces.QuadCount;
            int[,] faces = new int[rows, 4];
            for (int i = 0; i < rows; i++)
            {
                MeshFace f = m.Faces[i];
                faces[i, 0] = f.A;
                faces[i, 1] = f.B;
                faces[i, 2] = f.C;
                faces[i, 3] = f.D;
            }

            return FlattenIntData(faces, order);
        }

        /// <summary>
        /// Flatten a list of 3d vectors into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static double[] FlattenVector3dData(IEnumerable<Vector3d> vec, StorageOrder order = StorageOrder.ColumnMajor)
        {
            double[,] data = new double[vec.Count(), 3];
            for (int i = 0; i < vec.Count(); i++)
            {
                var v = vec.ElementAt(i);
                data[i, 0] = v.X;
                data[i, 1] = v.Y;
                data[i, 2] = v.Z;
            }

            return FlattenDoubleData(data, order);
        }

        /// <summary>
        /// Flatten a list of triangular faces into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="trias"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static int[] FlattenMeshFaceData(IEnumerable<MeshFace> trias, StorageOrder order = StorageOrder.ColumnMajor)
        {
            int[,] data = new int[trias.Count(), 3];
            for (int i = 0; i < trias.Count(); i++)
            {
                var f = trias.ElementAt(i);
                if (f.IsQuad) throw new ArgumentException("All faces need to be triangles.");
                data[i, 0] = f.A;
                data[i, 1] = f.B;
                data[i, 2] = f.C;
            }

            return FlattenIntData(data, order);
        }

        /// <summary>
        /// Build a quad mesh from flatten vertex and face data.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="quads"></param>
        /// <returns></returns>
        public static Mesh GetQuadMesh(double[] coords, int[] quads)
        {
            Mesh m = new Mesh();
            int vCount = coords.Length / 3;
            for (int i = 0; i < vCount; i++)
            {
                m.Vertices.Add(new Point3d(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]));
            }

            int eCount = quads.Length / 4;
            for (int i = 0; i < eCount; i++)
            {
                m.Faces.AddFace(new MeshFace(quads[i * 4], quads[i * 4 + 1], quads[i * 4 + 2], quads[i * 4 + 3]));
            }
            m.UnifyNormals();

            return m;
        }

        /// <summary>
        /// Flatten a multidimensional array of doubles into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="data"> Multidimensional array. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static double[,] ParseDoubleArrToMatrixXd(double[] data, int column = 3, StorageOrder order = StorageOrder.ColumnMajor)
        {
            int row = data.Length / column;

            double[,] outData = new double[row,column];

            if (order == StorageOrder.ColumnMajor)
            {
                for (int i = 0; i < column; i++)
                {
                    for (int j = 0; j < row; j++)
                    {
                        outData[j, i] = data[i * row + j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        outData[i, j] = data[i * column + j];
                    }
                }
            }

            return outData;
        }

        /// <summary>
        /// Flatten a multidimensional array of integers into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="data"> Multidimensional array. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static int[,] ParseDoubleArrToMatrixXi(int[] data, int column = 3, StorageOrder order = StorageOrder.ColumnMajor)
        {
            int row = data.Length / column;

            int[,] outData = new int[row, column];

            if (order == StorageOrder.ColumnMajor)
            {
                for (int i = 0; i < column; i++)
                {
                    for (int j = 0; j < row; j++)
                    {
                        outData[j, i] = data[i * row + j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        outData[i, j] = data[i * column + j];
                    }
                }
            }

            return outData;
        }

        public static double[] ParsePointerToDoubleArr(IntPtr ptr, int count)
        {
            // Marshal flat list of vector components
            double[] outData = new double[count];
            Marshal.Copy(ptr, outData, 0, count);

            // Free pointer
            Marshal.FreeCoTaskMem(ptr);

            return outData;
        }

        public static int[] ParsePointerToIntArr(IntPtr ptr, int count)
        {
            // Marshal flat list of vector components
            int[] outData = new int[count];
            Marshal.Copy(ptr, outData, 0, count);

            // Free pointer
            Marshal.FreeCoTaskMem(ptr);

            return outData;
        }

        public static Vector3d[] ParsePointerToVectorArr(IntPtr ptr, int count, bool is3D= true, StorageOrder order = StorageOrder.ColumnMajor)
        {
            // Marshal flat list of vector components
            int cols = 3;
            if (!is3D) cols = 2;
            double[,] data = ParseDoubleArrToMatrixXd(ParsePointerToDoubleArr(ptr, count), cols, order);

            // Matrix data to 3D vector
            Vector3d[] vec = new Vector3d[data.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                if(is3D) vec[i] = new Vector3d(data[i, 0], data[i, 1], data[i, 2]);
                else vec[i] = new Vector3d(data[i, 0], data[i, 1], 0);
            }

            return vec;
        }

        public static bool ParsePointerToMeshVertices(IntPtr ptr, int count, ref Mesh mesh, StorageOrder order = StorageOrder.ColumnMajor)
        {
            if (mesh.Vertices.Count == count/3)
            {
                // Marshal flat list of vector components
                double[,] data = ParseDoubleArrToMatrixXd(ParsePointerToDoubleArr(ptr, count), 3, order);

                // Matrix data to mesh vertices

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    mesh.Vertices.SetVertex(i, new Point3d(data[i, 0], data[i, 1], data[i, 2]));
                }

                return true;
            }
            else return false;
        }

        public static Point3d[] ParsePointerToPointArr(IntPtr ptr, int count, StorageOrder order = StorageOrder.ColumnMajor)
        {
            // Marshal flat list of vector components
            double[,] data = ParseDoubleArrToMatrixXd(ParsePointerToDoubleArr(ptr, count), 3, order);

            // Matrix data to 3D point
            Point3d[] pts = new Point3d[data.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                pts[i] = new Point3d(data[i, 0], data[i, 1], 0);
            }

            return pts;
        }

        public static MeshFace[] ParsePointerToMeshFaceArr(IntPtr ptr, int count, StorageOrder order = StorageOrder.ColumnMajor)
        {
            // Marshal flat list of vector components
            int[,] data = ParseDoubleArrToMatrixXi(ParsePointerToIntArr(ptr, count), 3, order);

            // Matrix data to meshface
            MeshFace[] faces = new MeshFace[data.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                faces[i] = new MeshFace(data[i, 0], data[i, 1], data[i, 2]);
            }

            return faces;
        }
    }
}
