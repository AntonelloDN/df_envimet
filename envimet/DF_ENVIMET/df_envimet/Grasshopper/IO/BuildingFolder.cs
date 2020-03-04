using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using df_envimet_lib.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace df_envimet.Grasshopper.IO
{
    public class BuildingFolder : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public BuildingFolder()
          : base("DF Envimet Building Files", "DFBuildingFiles",
              "Use this component to get avg output files of buildings. ",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_29_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_outputFolder", "_outputFolder", "Connect full path of output folder where envimet results are. E.g. 'C:\\...\\NewSimulation_output", GH_ParamAccess.item);
            pManager.AddTextParameter("_filter", "_filter", "Connect envimet Id of building to filter results.", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("buildingFiles", "buildingFiles", "Output avg files of each building.", GH_ParamAccess.list);
            pManager.AddTextParameter("buildingId", "buildingId", "Building Id.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _outputFolder = null;
            List<string> fileIds = new List<string>();

            DA.GetData(0, ref _outputFolder);
            DA.GetDataList(1, fileIds);

            IEnumerable<string> directories = BuildingOutput.GetAllBuildingDirectory(_outputFolder);
            IEnumerable<string> outputFile = new List<string>();
            List<string> buildingId = new List<string>();

            foreach (string dir in directories)
            {
                if (dir.Contains(BuildingFileType.DYN_FOLDER))
                {
                    outputFile = BuildingOutput.GetAllBuildingDatFiles(dir, BuildingFileType.AVG_FILE).ToList();

                    List<string> selectedFile = new List<string>();
                    List<string> selectedId = new List<string>();

                    foreach (string file in outputFile)
                    {
                        string name = Regex.Split(file, "Building").Last();
                        string id = Regex.Match(name, @"\d+").Value;

                        buildingId.Add(id);

                        if (fileIds.Count > 0)
                        {
                            foreach (string item in fileIds)
                            {
                                if (id == item)
                                {
                                    selectedFile.Add(file);
                                    selectedId.Add(id);
                                }
                            }
                        }
                    }

                    if (fileIds.Count > 0)
                    {
                        outputFile = selectedFile;
                        buildingId = selectedId;
                    }
                }
            }

            DA.SetDataList(0, outputFile);
            DA.SetDataList(1, buildingId);
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
                return Properties.Resources.envimetBuildingFolder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("943d44fe-087a-4774-853c-3d8157559afa"); }
        }
    }
}