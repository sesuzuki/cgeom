using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class PlanarizationGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PlanarizationGH()
          : base("Planarization", "Planarization",
            "Planarization of a given quad mesh.",
            "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Threshold", "Threshold", "Threshold for considering planarity.", GH_ParamAccess.item, 1e-3);
            pManager.AddIntegerParameter("Iterations", "Iterations", "Number of iterations.", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Resulting mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Planarity", "Planarity", "Planarity.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            int iterations = 1;
            double threshold = 1e-3;
            DA.GetData(0, ref m);
            DA.GetData(1, ref threshold);
            DA.GetData(2, ref iterations);

            if (m.Faces.TriangleCount > 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The mesh contains triangular faces. Only quad meshes can be planarize.");
                return;
            }

            double[] planarity;
            Mesh pm = Parameterizations.Planarization(m, iterations, out planarity, threshold);

            DA.SetData(0, pm);
            DA.SetDataList(1, planarity);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.Planarization;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("40d6187a-d8dd-4507-bf24-9c93d65800d6"); }
        }
    }
}
