﻿using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;

namespace CGeom.Tools
{
    public static class Helper
    {
        public enum StorargeOrder {RowMajor=0, ColumnMajor=1};

        /// <summary>
        /// Flatten a collection of rhino points into a 1D array with a specific storage order.
        /// </summary>
        /// <param name="pts"> Collection of rhino points. </param>
        /// <param name="order"> Storage order. </param>
        /// <returns></returns>
        public static double[] FlattenPoints(IEnumerable<Point3d> pts, StorargeOrder order)
        {
            int count = pts.Count();
            double[] outData = new double[count * 3];

            if (order == StorargeOrder.ColumnMajor)
            {
                for (int i = 0; i < count; i++)
                {
                    Point3d p = pts.ElementAt(i);
                    outData[i] = p.X;
                    outData[count + i] = p.Y;
                    outData[count * 2 + i] = p.Z;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Point3d p = pts.ElementAt(i);
                    outData[i * 3] = p.X;
                    outData[i * 3 + 1] = p.Y;
                    outData[i * 3 + 2] = p.Z;
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
        public static double[] FlattenDoubleData(double[,] data, StorargeOrder order)
        {
            int row = data.GetLength(0);
            int column = data.GetLength(1);

            double[] outData = new double[row * column];

            if (order == StorargeOrder.ColumnMajor)
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
        public static int[] FlattenIntData(int[,] data, StorargeOrder order)
        {
            int row = data.GetLength(0);
            int column = data.GetLength(1);

            int[] outData = new int[row * column];

            if (order == StorargeOrder.ColumnMajor)
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
        public static int[] FlattenFaceDate(Mesh m, StorargeOrder order)
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
    }
}