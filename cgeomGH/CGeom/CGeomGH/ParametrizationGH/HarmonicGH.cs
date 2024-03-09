using System;
using System.Collections.Generic;
using System.Linq;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace CGeomGH.ParametrizationGH
{
    public class HarmonicGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public HarmonicGH()
          : base("Harmonic", "Harmonic",
            "Harmonic Parametrization",
            "CGeom", "Parametrization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("uCount", "uCount", "Number of u coordinates", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("vCount", "vCount", "Number of v coordinates", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Angle", "Angle", "Rotation angle in radians", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Disk", "Disk", "Flattened disk mesh.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Texture2D", "Texture2D", "Checkerboard texture in 2D.", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Texture3D", "Texture3D", "Checkerboard texture in 3D.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            double scaleFactor = 1.0, angle = 0;
            int uCount = 10, vCount = 10; 
            DA.GetData(0, ref m);
            DA.GetData(1, ref uCount);
            DA.GetData(2, ref vCount);
            DA.GetData(3, ref angle);

            Mesh mesh3D, disk;
            Parametrizations.HarmonicParametrization(m, scaleFactor, out mesh3D, out disk);

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
                            int count = (int)(e.GetLength() / 0.01);
                            polyA.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                        else if (crv.Contains(p0, pl, 1e-3) == PointContainment.Inside && crv.Contains(p1, pl, 1e-3) == PointContainment.Inside)
                        {
                            e = new LineCurve(p0, p1);
                            int count = (int)(e.GetLength() / 0.01);
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
                            int count = (int)(e.GetLength() / 0.01);
                            polyB.Add(new PolylineCurve(e.DivideByCount(count == 0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                        else if (crv.Contains(p0, pl, 1e-3) == PointContainment.Inside && crv.Contains(p1, pl, 1e-3) == PointContainment.Inside)
                        {
                            e = new LineCurve(p0, p1);
                            int count = (int)(e.GetLength() / 0.01);
                            polyB.Add(new PolylineCurve(e.DivideByCount(count==0 ? 1 : count, true).Select(t => e.PointAt(t)).ToArray()));
                        }
                    }
                }
            }

            var eA = Curve.JoinCurves(polyA);
            var eB = Curve.JoinCurves(polyB);
            GH_Structure<GH_Curve> texture2D = new GH_Structure<GH_Curve>();
            texture2D.AppendRange(eA.Select(c => new GH_Curve(c)), new GH_Path(0));
            texture2D.AppendRange(eB.Select(c => new GH_Curve(c)), new GH_Path(1));

            // Map disk texture to 3d mesh
            List<Curve> eAA = new List<Curve>();
            foreach(var c in eA)
            {
                Polyline poly;
                c.TryGetPolyline(out poly);

                List<Point3d> tempP = new List<Point3d>();
                foreach(Point3d p in poly) tempP.Add(mesh3D.PointAt(disk.ClosestMeshPoint(p, 0.0)));

                eAA.Add(new PolylineCurve(tempP));
            }

            List<Curve> eBB = new List<Curve>();
            foreach (var c in eB)
            {
                Polyline poly;
                c.TryGetPolyline(out poly);

                List<Point3d> tempP = new List<Point3d>();
                foreach (Point3d p in poly) tempP.Add(mesh3D.PointAt(disk.ClosestMeshPoint(p, 0.0)));

                eBB.Add(new PolylineCurve(tempP));
            }
            GH_Structure<GH_Curve> texture3D = new GH_Structure<GH_Curve>();
            texture3D.AppendRange(eAA.Select(c => new GH_Curve(c)), new GH_Path(0));
            texture3D.AppendRange(eBB.Select(c => new GH_Curve(c)), new GH_Path(1));

            DA.SetData(0, disk);
            DA.SetDataTree(1, texture2D);
            DA.SetDataTree(2, texture3D);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d142fa1b-7264-421c-be35-9f834a18861f"); }
        }
    }
}
