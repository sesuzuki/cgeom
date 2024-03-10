using System;
using CGeom.Wrappers;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using static CGeom.Tools.Utils;

namespace CGeom.Tools
{
	public static class Bff
	{
		public static Mesh OpenSurfaceWithoutHolesToDisk(Mesh m)
		{
            // Parse mesh data
            double[] inCoords;
            int[] inFaces;
            int numVertices, numFaces;
            ParseTriangleRhinoMesh(m, out inCoords, out inFaces, out numVertices, out numFaces, false);

            int numUV, numUVFaces;
            IntPtr ptrUV, ptrUVFaces, outErrorMsg;
            Kernel.BFF.CgeomOpenSurfaceWithoutHolesToDisk(numVertices, numFaces, inCoords, inFaces, out numUV, out ptrUV, out numUVFaces, out ptrUVFaces, out outErrorMsg);

            // Marshal flat list of vector components
            double[] outUV = new double[numUV];
            Marshal.Copy(ptrUV, outUV, 0, numUV);

            int count = numUV / 3;
            Point3d[] pts = new Point3d[count];
            for (int i = 0; i < count; i++)
            {
                pts[i] = new Point3d(outUV[i * 3], outUV[i * 3 + 1], outUV[i * 3 + 2]);
            }

            // Marshal flat list of vector components
            int[] outUVFaces = new int[numUVFaces];
            Marshal.Copy(ptrUVFaces, outUVFaces, 0, numUVFaces);

            count = numUVFaces / 3;
            MeshFace[] faces = new MeshFace[count];
            for (int i = 0; i < count; i++) faces[i] = new MeshFace(outUVFaces[i * 3], outUVFaces[i * 3 + 1], outUVFaces[i * 3 + 2]);

            // 3d mesh with texture coordinates
            Mesh disk = new Mesh();
            disk.Vertices.AddVertices(pts);
            disk.Faces.AddFaces(faces);
            disk.UnifyNormals();
            disk.Normals.ComputeNormals();

            return disk;
		}

    }
}

