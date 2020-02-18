using System;
using System.Drawing;
using Grasshopper.Kernel;


namespace df_envimet
{
    public class df_envimetInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "dfenvimet";
            }
        }

        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "A series of component for envimet.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("5a4f6fb7-1dbc-4f9c-bc1c-0cb8f681c695");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Antonello Di Nunzio";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "antonellodinunzio@gmail.com";
            }
        }
    }
}
