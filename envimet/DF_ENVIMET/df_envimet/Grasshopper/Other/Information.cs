using System;
using System.IO;
using System.Net;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.Other
{
    public class Information : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Information()
          : base("DF Envimet Information", "DFEnvimetInformation",
              "News and Info from DF Envimet developer!\nUse this component to see roadmap of Dragonfly Envimet or patches released by developer",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.senary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("message", "message", "Connect a panel to see info.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = client.OpenRead("https://raw.githubusercontent.com/AntonelloDN/df_envimet/master/plugin%20beta/extra/info.txt");
            StreamReader reader = new StreamReader(data);
            string message = reader.ReadToEnd();
            DA.SetData(0, message);
            data.Close();
            reader.Close();
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.envimetInformationcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1b2b6a70-b6c3-464f-ab51-c6ecda527930"); }
        }
    }
}