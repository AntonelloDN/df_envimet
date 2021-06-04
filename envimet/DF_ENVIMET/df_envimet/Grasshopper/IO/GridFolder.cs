using System;
using System.Collections.Generic;
using df_envimet.Grasshopper.UI_GH;
using Grasshopper.Kernel;
using System.IO;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.IO
{
    public class GridFolder : ExtensionGridFolderComponent
    {
        /// <summary>
        /// Initializes a new instance of the GridFolder class.
        /// </summary>
        public GridFolder()
          : base("DF Envimet Grid Folder", "DFGridFolder",
              "Use this component to get grid output files.\nRight click to select output folder.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
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
            pManager.AddTextParameter("gridFiles", "gridFiles", "Building files to read.", GH_ParamAccess.list);
            pManager.AddTextParameter("gridBinaryFiles", "gridBinaryFiles", "Building binary files to read.", GH_ParamAccess.list);
            pManager.AddGenericParameter("type", "type", "Grid output type.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _outputFolder = null;

            DA.GetData(0, ref _outputFolder);

            string outputPath = Path.Combine(_outputFolder, _value);
            IEnumerable<string> gridFiles = new List<string>();
            IEnumerable<string> gridBinaryFiles = new List<string>();

            gridFiles = GridOutput.GetAllGridFiles(outputPath, GridOutputExtension.Standard);
            gridBinaryFiles = GridOutput.GetAllGridFiles(outputPath, GridOutputExtension.Binary);

            DA.SetDataList(0, gridFiles);
            DA.SetDataList(1, gridBinaryFiles);
            DA.SetData(2, _value);
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
                return Properties.Resources.envimetGridFolder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e9b143c9-05fb-41c0-a31b-d69b1193d3db"); }
        }
    }
}