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
            "Create a smooth N-RoSy field.",
            "CGeom", "Parameterization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ConstrainedFaces","CFace","Indexes of faces to be constrained.",GH_ParamAccess.list);
            pManager.AddVectorParameter("ConstrainedVectors", "CVec", "Representative vectors for constrained faces (one per face).", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("X1", "X1", "First orthogonal vectors of the frame field.", GH_ParamAccess.list);
            pManager.AddVectorParameter("X2", "X2", "Second orthogonal vectors of the frame field.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Barycenters", "Barycenters", "Face barycenters.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Singularities", "Singularities", "Singularity index for each vertex.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            List<int> faceIdx = new List<int>();
            List<Vector3d> vec = new List<Vector3d>();
            DA.GetData(0, ref m);
            DA.GetDataList(1, faceIdx);
            DA.GetDataList(2, vec);

            if (faceIdx.Count != vec.Count) new GH_RuntimeMessage("The number of constrained faces doesn't match with the number of representative vectors per constrained face.", GH_RuntimeMessageLevel.Error);

            Vector3d[] x1, x2, barycenters;
            double[] singularities;
            Parameterizations.BuildNRosy(m, faceIdx, vec, out x1, out x2, out barycenters, out singularities);

            DA.SetDataList(0, x1);
            DA.SetDataList(1, x2);
            DA.SetDataList(2, barycenters);
            DA.SetDataList(3, singularities);
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
