﻿using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.Quantities
{
    public class PerVertexPrincipalCurvaturesGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PerVertexPrincipalCurvaturesGH()
          : base("PerVertexPrincipalCurvatures", "VPrincipalCurvatures",
            "Computes the principal curvatures at vertices.",
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
            pManager.AddNumberParameter("Val1", "V1", "First principal curvature value.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Val2", "V2", "Second principal curvature value.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Dir1", "D1", "First principal curvature direction.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Dir2", "D2", "Second principal curvature direction.", GH_ParamAccess.list);
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

            double[] val1, val2;
            Vector3d[] dir1, dir2;
            DiscreteQuantities.PerVertexPrincipalCurvatures(m, out val1, out val2, out dir1, out dir2);

            DA.SetDataList(0, val1);
            DA.SetDataList(1, val2);
            DA.SetDataList(2, dir1);
            DA.SetDataList(3, dir2);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.PerVertexPrincipalCurvatures;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7aae8316-d918-4691-a42f-cec383b9b851"); }
        }
    }
}
