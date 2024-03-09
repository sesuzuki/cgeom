using System;
using System.Collections.Generic;
using CGeom.Tools;
using CGeom.Utils;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using static Rhino.FileIO.File3dmCurvePiping;
using static System.Collections.Specialized.BitVector32;
using static CGeom.Utils.ColorMaps;
using System.Linq;
using CGeomGH.Utils;
using GH_IO.Serialization;
using static CGeom.Tools.DiscreteQuantities;

namespace CGeomGH.QuantitiesGH
{
    public class GaussianCurvatureGH : GH_Component
    {
        int metricIdx, cmapIdx;
        List<List<string>> menuAttributes;
        List<string> selection;
        bool buildAttributes = true;

        #region dropdownmenu content
        readonly List<string> categories = new List<string>(new string[] { "Metrics", "ColorMaps" });
        readonly List<string> metricTypes = ((CurvatureMetricTypes[])Enum.GetValues(typeof(CurvatureMetricTypes))).Select(t => t.ToString()).ToList();
        readonly List<string> cmapTypes = ((ColorMapTypes[])Enum.GetValues(typeof(ColorMapTypes))).Select(t => t.ToString()).ToList();
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GaussianCurvatureGH()
          : base("Curvatures", "Curvatures",
            "Compute curvatures of a given mesh.",
            "CGeom", "Quantities")
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
                menuAttributes.Add(metricTypes);
                menuAttributes.Add(cmapTypes);
                selection.Add(metricTypes[metricIdx]);
                selection.Add(cmapTypes[cmapIdx]);
            }

            if (dropdownListId == 0)
            {
                metricIdx = selectedItemId;
                selection[0] = menuAttributes[0][selectedItemId];
            }

            if (dropdownListId == 1)
            {
                cmapIdx = selectedItemId;
                selection[1] = menuAttributes[1][selectedItemId];
            }

            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Initial triangular mesh (quad-meshes will be triangulated).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Alpha", "Alpha", "Set the alpha value (from 0.0 to 1.0) to control the transparency of the visualization", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("cMesh", "cMesh", "Colored mesh based on evaluated metrics.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Data", "Data", "Metrics per vertex.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            double alpha = 1;
            DA.GetData(0, ref m);
            DA.GetData(1, ref alpha);

            if (alpha < 0) alpha = 0;
            if (alpha > 1) alpha = 1;

            CurvatureMetricTypes metricType = ((CurvatureMetricTypes[])Enum.GetValues(typeof(CurvatureMetricTypes)))[metricIdx];
            ColorMapTypes cmapType = ((ColorMapTypes[])Enum.GetValues(typeof(ColorMapTypes)))[cmapIdx];

            Mesh cMesh;
            double[] data;
            MetricsVisualization.BuildColoredMeshMetrics(m, metricType, cmapType, (int)(alpha * 255), out cMesh, out data);

            DA.SetData(0, cMesh);
            DA.SetDataList(1, data);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("metricsIdx", metricIdx);
            writer.SetInt32("cmapIdx", cmapIdx);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.TryGetInt32("metricsIdx", ref metricIdx))
            {
                FunctionToSetSelectedContent(0, metricIdx);
                m_attributes = new DropDownAttributesGH(this, FunctionToSetSelectedContent, menuAttributes, selection, categories);
            }

            if (reader.TryGetInt32("cmapIdx", ref cmapIdx))
            {
                FunctionToSetSelectedContent(1, cmapIdx);
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
                return null;// Properties.Resources.Resources.Gauss;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9183e42c-defe-4e64-b1ca-175d8df38183"); }
        }
    }
}
