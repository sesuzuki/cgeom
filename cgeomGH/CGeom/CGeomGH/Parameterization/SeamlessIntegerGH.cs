using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using CGeom.Tools;

namespace CGeomGH.Parameterization
{
    public class SeamlessIntegerGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SeamlessIntegerGH()
          : base("SIGParam", "SIGParam",
            "Seamless-Integer-Grid Parameterization",
            "CGeom", "Parameterization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddGenericParameter("NRosy", "NRosy", "NRosy field.", GH_ParamAccess.item);
            pManager.AddNumberParameter("GradSize","GradSize","Gradient size.", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Stiffness", "Stiffness", "Stiffness", GH_ParamAccess.item,5.0);
            pManager.AddBooleanParameter("Round", "Round", "Direct round", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Iterations", "Iter", "Number of iterations", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("UV", "UV", "UV parameterization.", GH_ParamAccess.list);
            pManager.AddMeshFaceParameter("FUV", "FUV", "Indexes of UV parameters per mesh face.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            NRosy rosy = new NRosy();
            double gradient_size = 10;
            int iter = 0;
            double stiffness = 5.0;
            bool direct_round = false;
            DA.GetData(0, ref m);
            DA.GetData(1, ref rosy);
            DA.GetData(2, ref gradient_size);
            DA.GetData(3, ref stiffness);
            DA.GetData(4, ref direct_round);
            DA.GetData(5, ref iter);

            Vector3d[] UV;
            MeshFace[] FUV;
            Parameterizations.BuildSeamlessIntegerParameterization(m, rosy.X1, rosy.X2, gradient_size, stiffness, direct_round, iter, out UV, out FUV);

            DA.SetDataList(0, UV);
            DA.SetDataList(1, FUV);
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
            get { return new Guid("1baf7a04-a76b-49bd-b47f-720611c80b38"); }
        }
    }
}
