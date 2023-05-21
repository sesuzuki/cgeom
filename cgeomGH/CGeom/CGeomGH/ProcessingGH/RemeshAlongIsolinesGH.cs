using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class RemeshAlongIsolinesGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RemeshAlongIsolinesGH()
          : base("RemeshAlongIsoline", "RemeshAlongIso",
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
            pManager.AddNumberParameter("ScalarField","sField","Scalar field as a list of doubles.", GH_ParamAccess.list);
            pManager.AddNumberParameter("IsoValue", "Isovalue", "IsoValue of the scalar field.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("IsoMesh", "IsoMesh", "Mesh remeshed along isolines.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            List<double> scalarField = new List<double>();
            double isoValue= 0;
            DA.GetData(0, ref m);
            DA.GetDataList(1, scalarField);
            DA.GetData(2, ref isoValue);

            if (scalarField.Count != m.Vertices.Count) throw new Exception("Invalid scalar field. The number of scalars doesn't match the number of vertices.");
            if (!scalarField.Contains(isoValue)) throw new Exception("Invalid isoValue. The scalar field doesn't contain the given isoValue.");

            Mesh outMesh = Processing.RemeshAlongIsoline(m, scalarField.ToArray(), isoValue);

            DA.SetData(0, outMesh);
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
            get { return new Guid("6eb6a76f-8ca5-4727-bc20-d86c620f3c30"); }
        }
    }
}
