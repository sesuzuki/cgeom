using System;
using System.Collections.Generic;
using System.Resources;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ParametrizationGH
{
    public class DeconstructNRosyGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DeconstructNRosyGH()
          : base("Deconstruct NRosy", "DeNRosy",
            "Extract the components of an N-Rosy field.",
            "CGeom", "Parametrization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("N-Rosy", "N-Rosy", "N-Rosy field.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Barycenter", "B", "Barycenters associated with the field.", GH_ParamAccess.list);
            pManager.AddVectorParameter("X1", "X1", "First representative vectors of the field.", GH_ParamAccess.list);
            pManager.AddVectorParameter("X2", "X2", "Second representative vectors of the field.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Singularities", "S", "Singularities of the field.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Degree", "Degree", "Degree of the field.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            NRosy rosy = new NRosy();

            DA.GetData(0, ref rosy);

            DA.SetDataList(0, rosy.Barycenters);
            DA.SetDataList(1, rosy.X1);
            DA.SetDataList(2, rosy.X2);
            DA.SetDataList(3, rosy.Singularities);
            DA.SetData(4, rosy.Degree);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.DeconstructNRosy;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4fe1d1b6-9fe6-4d2c-a38c-853f445c2202"); }
        }
    }
}
