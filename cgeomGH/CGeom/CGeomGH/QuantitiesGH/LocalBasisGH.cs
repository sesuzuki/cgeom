using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.QuantitiesGH
{
    public class LocalBasisGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public LocalBasisGH()
          : base("LocalBasis", "LocalBasis",
            "Get a local reference plane for each face on a triangular mesh.",
            "CGeom", "Quantities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "Local reference plane for each mesh face.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            DA.GetData(0, ref m);

            Point3d[] orig = DiscreteQuantities.Barycenters(m);
            Vector3d[] B1, B2, B3;
            DiscreteQuantities.LocalBasis(m, out B1, out B2, out B3);

            Plane[] pl = new Plane[B1.Length];
            for(int i=0; i<B1.Length; i++)
            {
                pl[i] = new Plane(orig[i], B1[i], B2[i]);
            }

            DA.SetDataList(0, pl);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;// Properties.Resources.Resources.LocalBasis;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3db0ef9f-2851-4405-819b-ae5b8d359674"); }
        }
    }
}
