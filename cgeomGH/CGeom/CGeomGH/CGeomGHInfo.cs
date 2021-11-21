using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace CGeomGH
{
    public class CGeomGHInfo : GH_AssemblyInfo
    {
        public override string Name => "CGeomGH Info";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("DA82E4B0-B56A-435F-A59B-3BBCB4798F59");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}
