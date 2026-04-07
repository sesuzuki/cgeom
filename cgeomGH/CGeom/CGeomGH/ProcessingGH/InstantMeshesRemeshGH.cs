using System;
using System.Collections.Generic;
using CGeom.Tools;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CGeomGH.ProcessingGH
{
    public class InstantMeshesRemeshGH : GH_Component
    {
        public InstantMeshesRemeshGH()
          : base("InstantMeshesRemesh", "IMRemesh",
                    "Field-aligned remeshing using Instant Meshes (Jakob et al. 2015). " +
                    "Provide either EdgeLength or TargetVertexCount to control output density.",
                    "CGeom", "Processing")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Input triangular mesh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("EdgeLength", "EdgeLength", "Target output edge length. Ignored when TargetVertexCount > 0.", GH_ParamAccess.item, 0.0);
            pManager.AddIntegerParameter("TargetVertexCount", "VertexCount", "Target number of output vertices. When > 0, overrides EdgeLength.", GH_ParamAccess.item, 500);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Remeshed output.", GH_ParamAccess.item);
            pManager.AddTextParameter("Log", "Log", "Status log.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh m = null;
            double edgeLength = 0.0;
            int targetVertexCount = 0;

            DA.GetData(0, ref m);
            DA.GetData(1, ref edgeLength);
            DA.GetData(2, ref targetVertexCount);

            string log;
            var outMesh = Processing.InstantMeshesRemesh(m, edgeLength, targetVertexCount, out log);
            if (outMesh == null) this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Remesh failed. Check log.");

            DA.SetData(0, outMesh);
            DA.SetData(1, log);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("b2e4a1c7-3f85-4d92-ae61-7f30c9d48e25"); }
        }
    }
}
