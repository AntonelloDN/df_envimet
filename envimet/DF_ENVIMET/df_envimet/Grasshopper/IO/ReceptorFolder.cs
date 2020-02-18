using System;
using Grasshopper.Kernel;
using df_envimet_lib.IO;
using System.Collections.Generic;
using System.Linq;

namespace df_envimet.Grasshopper.IO
{
    public class ReceptorFolder : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReceptorFolder class.
        /// </summary>
        public ReceptorFolder()
          : base("DF Envimet Receptor Folder", "DFReceptorFolder",
              "Use this component to get all output folders of receptors. ",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_02_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_outputFolder", "_outputFolder", "Connect full path of output folder where envimet results are. E.g. 'C:\\...\\NewSimulation_output", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("receptorPath", "receptorPath", "Output folder of each simulated receptor.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _outputFolder = null;

            DA.GetData(0, ref _outputFolder);

            IEnumerable<string> directories = ReceptorOutput.GetAllReceptorDirectory(_outputFolder);

            DA.SetDataList(0, directories.ToList());
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
                return Properties.Resources.envimetReadReceptorDirectorycon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2663738d-6962-430c-bcb2-ceb7e55053a9"); }
        }
    }
}