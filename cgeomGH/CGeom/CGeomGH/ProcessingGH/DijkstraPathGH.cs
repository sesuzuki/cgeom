using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class DijkstraPathGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DijkstraPathGH()
          : base("DijkstraPath", "DijkstraPath",
                "Compute Dijkstra paths on a triangular mesh.",
                "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Triangular Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Start", "Start", "Use the reference start point to find the path. The closest vertex to this point will be selected.", GH_ParamAccess.list);
            pManager.AddPointParameter("End", "End", "Use the reference end point to find the path. The closest vertex to this point will be selected.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Geodesics", "Geodesics", "Geodesic paths", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = new Mesh();
            List<Point3d> start = new List<Point3d>();
            List<Point3d> end = new List<Point3d>();
            DA.GetData(0, ref m);
            DA.GetDataList(1, start);
            DA.GetDataList(2, end);

            if (m.Faces.QuadCount > 0) m.Faces.ConvertQuadsToTriangles();

            var paths = Processing.DijkstraPath(m, start, end);

            DA.SetDataList(0, paths);
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
            get { return new Guid("80c86a54-4589-42e4-b66a-f68aec74df22"); }
        }
    }
}
