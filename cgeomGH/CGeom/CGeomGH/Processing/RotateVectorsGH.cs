using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using CGeom.Tools;

namespace CGeomGH.Processing
{
    public class RotateVectorsGH : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RotateVectorsGH()
          : base("RotateVectors", "RotateVectors",
            "Rotate vectors with respect to reference planes.",
            "CGeom", "Processing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Vector", "Vec", "Vectors to rotate.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Angle", "Ang", "Rotation angle (in degrees).", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "Pl", "Reference planes to use for rotation.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("RVector", "RVec", "Rotated vector.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Vector3d> vec = new List<Vector3d>();
            List<Plane> planes = new List<Plane>();
            List<double> angles = new List<double>();
            DA.GetDataList(0, vec);
            DA.GetDataList(1, angles);
            DA.GetDataList(2, planes);

            if (vec.Count != planes.Count) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of vectors doesn't match with the number of reference planes.");

            int numPlanes = planes.Count;
            Vector3d[] B1 = new Vector3d[numPlanes];
            Vector3d[] B2 = new Vector3d[numPlanes];
            double[] inAngles = new double[numPlanes];
            
            for (int i=0; i<numPlanes; i++)
            {
                var pl = planes[i];
                B1[i] = pl.XAxis;
                B2[i] = pl.YAxis;

                if (angles.Count == numPlanes) inAngles[i] = angles[i] * Math.PI / 180;
                else inAngles[i] = angles[0] * Math.PI/180;
            }

            Vector3d[] X2;
            CGeom.Tools.Processing.RotateVectors(vec, inAngles, B1, B2, out X2);

            DA.SetDataList(0, X2);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Resources.RotateVectors;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d7c6d76b-d391-46dc-b3db-53395a3c9c1f"); }
        }
    }
}
