using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class RemeshAlongCurvatureGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RemeshAlongCurvatureGH()
          : base("RemeshAlongCurvature", "RemeshCurvature",
                    "Remesh so that a given isovalue of the scalar field follows (new) edges of the output mesh.",
                    "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddNumberParameter("EdgeLength", "EdgeLength", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Anchor", "Anchor", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("TriMesh", "TriMesh", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Log", "Log", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            double edgeLength = 1.0, anchor = 0.5;
            DA.GetData(0, ref m);
            DA.GetData(1, ref edgeLength);
            DA.GetData(2, ref anchor);

            string log; 
            var outMesh = Processing.RemeshAlignedToCurvatureField(m, edgeLength, anchor, out log);
            if (outMesh == null) this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failure case! Check log.");

            DA.SetData(0, outMesh);
            DA.SetData(1, log);
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
            get { return new Guid("45f83f36-4386-4c50-8ea0-2cf21ee98baf"); }
        }
    }
}
