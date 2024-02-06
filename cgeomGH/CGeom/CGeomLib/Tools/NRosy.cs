using System;
using System.Collections.Generic;
using System.Linq;
using CGeom.Wrappers;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using static CGeom.Tools.Utils;

namespace CGeom.Tools
{
    public struct NRosy : IGH_Goo
    {
        public Vector3d[] X1 { get; private set; }
        public Vector3d[] X2 { get; private set; }
        public double[] Singularities { get; private set; }
        public Point3d[] Barycenters { get; private set; }
        public int Degree { get; set; }

        public NRosy(Mesh mesh, IEnumerable<int> B, IEnumerable<Vector3d> FF1, IEnumerable<Vector3d> FF2, int degree = 4)
        {
            if (B.Count() != FF1.Count()) throw new ArgumentException("The number of constrained faces doesn't match the number of first representative vectors.");

            // Parse mesh data
            double[] coords;
            int[] faces;
            int numVertices, numFaces;
            ParseTriangleRhinoMesh(mesh, out coords, out faces, out numVertices, out numFaces);

            double[] inFF1 = FlattenVector3dData(FF1.ToArray(), StorageOrder.ColumnMajor);
            double[] inFF2 = FlattenVector3dData(FF2.ToArray(), StorageOrder.ColumnMajor);
            IntPtr ptrX1, ptrX2, ptrS;
            int outX1CoordsCount, outX2CoordsCount, outSingularitiesCount;

            if (inFF2.Length == 0)
            {
                Kernel.Processing.CgeomNRosy(numVertices, numFaces, B.Count(), coords, faces, B.ToArray(), inFF1, degree, out outX1CoordsCount, out outX2CoordsCount, out outSingularitiesCount, out ptrX1, out ptrX2, out ptrS);
            }
            else
            {
                if (B.Count() != FF2.Count()) throw new ArgumentException("The number of constrained faces doesn't match the number of second representative vectors.");

                Kernel.Processing.CgeomNRosyFromFrameField(numVertices, numFaces, B.Count(), coords, faces, B.ToArray(), inFF1, inFF2, degree, out outX1CoordsCount, out outX2CoordsCount, out outSingularitiesCount, out ptrX1, out ptrX2, out ptrS);
            }

            X1 = ParsePointerToVectorArr(ptrX1, outX1CoordsCount);
            X2 = ParsePointerToVectorArr(ptrX2, outX2CoordsCount);
            Singularities = ParsePointerToDoubleArr(ptrS, outSingularitiesCount);
            Degree = degree;

            IntPtr outCoords;
            int outCoordsCount;
            Kernel.DiscreteQuantities.CgeomBarycenters(numVertices, numFaces, coords, faces, out outCoordsCount, out outCoords);

            Barycenters = Utils.ParsePointerToPoint3DArr(outCoords, outCoordsCount);
        }

        public override string ToString()
        {
            return "NRosy";
        }

        #region GH
        public bool IsValid
        {
            get
            {
                if (X1 == null || X2 == null && X1.Length!=X2.Length) return false;
                else return true;
            }
        }

        public string IsValidWhyNot => "Incorrect initialization.";

        public string TypeName => ToString();

        public string TypeDescription => Degree + "N-rotationally Symmetric Tangent Fields";

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo)this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            target = default(T);
            return false;
        }

        public object ScriptVariable()
        {
            return null;
        }

        public bool Write(GH_IWriter writer)
        {
            return false;
        }

        public bool Read(GH_IReader reader)
        {
            return false;
        }
        #endregion
    }
}
