using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class ParallelTransportGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParallelTransportGH()
          : base("ParallelTransport", "ParallelTransp",
                  "Compute parallel transport of a vector.",
                  "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("SourceIndex","SourceIndex","Source index", GH_ParamAccess.item);
            pManager.AddNumberParameter("parParam", "ParParam", "Parallel parameter", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("perpParam", "PerParam", "Perpendicualr parameters", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Reference points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Vectors", "Vec", "Parallel transported vector", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            int sourceIndex = -1;
            double parParam = 1.00, perpParam = 1.00;
            DA.GetData(0, ref m);
            DA.GetData(1, ref sourceIndex);
            DA.GetData(2, ref parParam);
            DA.GetData(3, ref perpParam);


            Vector3d[] vectors;
            Point3d[] points;
            Processing.ParallelTransport(m, sourceIndex, parParam, perpParam, out points, out vectors); 

            DA.SetDataList(0, points);
            DA.SetDataList(1, vectors);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return new Guid("1deb623f-19fb-4705-a460-8b44a5781797"); }
        }
    }
}
