using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.Parameterization
{
    public class NRosyGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public NRosyGH()
          : base("NRosy", "NRosy",
            "Create a N-rotationally Symmetric Tangent Field.",
            "CGeom", "Parameterization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("b","b", "Sparse set of indices of constrained faces.", GH_ParamAccess.list);
            pManager.AddVectorParameter("bc1", "bc1", "First representative vector for constrained faces (one per face).", GH_ParamAccess.list);
            pManager.AddVectorParameter("bc2", "bc2", "Second representative vector for constrained faces (one per face).", GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("N-Rosy", "N-Rosy", "N-rotationally Symmetric Tangent Field.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            List<int> B = new List<int>();
            List<Vector3d> FF1 = new List<Vector3d>();
            List<Vector3d> FF2 = new List<Vector3d>();

            DA.GetData(0, ref m);
            DA.GetDataList(1, B);
            DA.GetDataList(2, FF1);
            DA.GetDataList(3, FF2);

            if (B.Count == 0) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid number of constraints");
            if (m.Faces.Count < 20) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The input mesh is too coarse.");
            if (B.Count > (m.Faces.Count * 0.1)) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Too many constraints for the given mesh resolution.");

            NRosy rosy = new NRosy(m, B, FF1, FF2);

            DA.SetData(0, rosy);
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
            get { return new Guid("8006e788-333c-41ad-8744-9655c794ff34"); }
        }
    }
}
