using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class LaplacianForOpenMeshGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public LaplacianForOpenMeshGH()
          : base("LaplacianOpenMesh", "LaplacianOpenMesh",
            "Apply a Laplacian smoothing to a given open triangular mesh.",
            "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddPointParameter("Anchors", "Anchors", "Location of vertices to be fixed during smoothing.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Threshold distance for searching anchor vertices.", GH_ParamAccess.item, 1e-3);
            pManager.AddIntegerParameter("Iterations", "Iterations", "Number of iterations.", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Resulting mesh.", GH_ParamAccess.item);
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
            double tol = 1e-3;
            List<Point3d> anchors = new List<Point3d>();
            DA.GetData(0, ref m);
            DA.GetDataList(1, anchors);
            DA.GetData(2, ref tol);
            DA.GetData(3, ref iterations);

            CGeom.Tools.Processing.LaplacianSmoothingForOpenMesh(iterations, ref m, anchors, 1e-3);

            DA.SetData(0, m);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.LaplaceO;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ffd6bbda-4c77-4c4d-acb4-a035289388ca"); }
        }
    }
}
