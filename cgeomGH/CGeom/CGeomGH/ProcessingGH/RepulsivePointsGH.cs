using System;
using System.Collections.Generic;
using System.Linq;
using CGeom.Tools;
using CGeomGH.Utils;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using static CGeom.Tools.DiscreteQuantities;
using static CGeom.Tools.Processing;
using static CGeom.Utils.ColorMaps;
using static Rhino.FileIO.File3dmCurvePiping;

namespace CGeomGH.ProcessingGH
{
    public class RepulsivePointsGH : GH_Component
    {
        int fieldIdx;
        List<List<string>> menuAttributes;
        List<string> selection;
        bool buildAttributes = true;

        #region dropdownmenu content
        readonly List<string> categories = new List<string>(new string[] { "ForceField" });
        readonly List<string> fieldTypes = ((ForceFieldTypes[])Enum.GetValues(typeof(ForceFieldTypes))).Select(t => t.ToString()).ToList();
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RepulsivePointsGH()
          : base("RepulsivePoints", "RepulsivePoints",
                "Convert a fixed point into a repulsive one by computing a force field.",
                "CGeom", "Processing")
        {
        }

        public override void CreateAttributes()
        {
            if (buildAttributes)
            {
                FunctionToSetSelectedContent(0, 0);
                buildAttributes = false;
            }
            m_attributes = new DropDownAttributesGH(this, FunctionToSetSelectedContent, menuAttributes, selection, categories);
        }

        public void FunctionToSetSelectedContent(int dropdownListId, int selectedItemId)
        {
            if (menuAttributes == null)
            {
                menuAttributes = new List<List<string>>();
                selection = new List<string>();
                menuAttributes.Add(fieldTypes);
                selection.Add(fieldTypes[fieldIdx]);
            }

            if (dropdownListId == 0)
            {
                fieldIdx = selectedItemId;
                selection[0] = menuAttributes[0][selectedItemId];
            }

            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Pts", "Pts", "Points where forces are going to be exerted.", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed", "Fixed", "Points to be converted into repulsive points.", GH_ParamAccess.list);
            pManager.AddNumberParameter("A", "A", "Scaling factor determining the strength of the interaction.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Sigma", "Sigma", "Sigma is the width parameter controlling the range of the interaction (only used for Gaussian potentials).", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("dt", "dt", "Time step for integrating forces.", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Iter", "Iter", "Number of iterations for integrating forces.", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Positions", "Pos", "Updated positions", GH_ParamAccess.list);
            pManager.AddVectorParameter("Forces", "Forces", "Calculated forces", GH_ParamAccess.list);
            pManager.AddNumberParameter("Potentials", "Potentials", "Calculated potentials", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            List<Point3d> fixedPts = new List<Point3d>();
            double dt = 1, A = 1, sigma = 0.5;
            int numIter = 100;
            DA.GetDataList(0, pts);
            DA.GetDataList(1, fixedPts);
            DA.GetData(2, ref A);
            DA.GetData(3, ref sigma);
            DA.GetData(4, ref dt);
            DA.GetData(5, ref numIter);

            Point3d[] pos;
            Vector3d[] forces;
            double[] potentials;

            ForceFieldTypes field = ((ForceFieldTypes[])Enum.GetValues(typeof(ForceFieldTypes)))[fieldIdx];
            if(field==ForceFieldTypes.Gaussian) RepulsivePointsWithGaussianPotentials(pts, fixedPts, A, sigma, out pos, out forces, out potentials, dt, numIter);
            else RepulsivePointsWithInverseDistances(pts, fixedPts, A, out pos, out forces, out potentials, dt, numIter);
            DA.SetDataList(0, pos);
            DA.SetDataList(1, forces);
            DA.SetDataList(2, potentials);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("fieldIdx", fieldIdx);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.TryGetInt32("fieldIdx", ref fieldIdx))
            {
                FunctionToSetSelectedContent(0, fieldIdx);
                m_attributes = new DropDownAttributesGH(this, FunctionToSetSelectedContent, menuAttributes, selection, categories);
            }
            return base.Read(reader);
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
            get { return new Guid("fadd832b-75ee-4b60-967c-de8b30f5aaaa"); }
        }
    }
}
