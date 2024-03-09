using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using CGeom.Tools;

namespace CGeomGH.ParametrizationGH
{
    public class QuadMeshExtractionGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public QuadMeshExtractionGH()
          : base("QuadMesh", "QMesh",
            "Quad mesh extraction from a given parameterization.",
            "CGeom", "Parametrization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddVectorParameter("UV", "UV", "UV parameterization", GH_ParamAccess.list);
            pManager.AddMeshFaceParameter("FUV", "FUV", "UV indexes per face", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("QuadMesh", "QMesh", "Quadrangulated mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            List<Vector3d> uv = new List<Vector3d>();
            List<MeshFace> fuv = new List<MeshFace>();
            DA.GetData(0, ref m);
            DA.GetDataList(1, uv);
            DA.GetDataList(2, fuv);

            Mesh qm = Parametrizations.QuadMeshExtraction(m, uv, fuv);

            DA.SetData(0, qm);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;/// Properties.Resources.Resources.QuadExtraction;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cfe84ada-7bb8-448b-a8f5-d5385947d2ad"); }
        }
    }
}
