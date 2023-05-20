using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.Quantities
{
    public class ExactGeodesicDistancesGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ExactGeodesicDistancesGH()
          : base("GeodesicDistances", "GeoDist",
                    "Compute exact geodesic distances from a set of points.",
                    "CGeom", "Quantities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddPointParameter("SourcePoints", "Source", "Source points. ", GH_ParamAccess.list);
            pManager.AddPointParameter("TargetPoints", "Target", "Target points. ", GH_ParamAccess.list);
            pManager.AddBooleanParameter("UseFaces", "UseFaces", "Compute geodesic distance from face centers.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("GeodesicDistances", "GeodesicDist", "Exact geodesic distances.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            List<Point3d> sourcePoints = new List<Point3d>();
            List<Point3d> targetPoints = new List<Point3d>();
            bool useFaces = false;
            DA.GetData(0, ref m);
            DA.GetDataList(1, sourcePoints);
            DA.GetDataList(2, targetPoints);
            DA.GetData(3, ref useFaces);

            double[] d = DiscreteQuantities.ExactGeodesicDistances(m, sourcePoints, targetPoints, useFaces);

            DA.SetDataList(0, d);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return new Guid("1dcd4176-4d0a-492c-8b35-0cfca3d7af61"); }
        }
    }
}
