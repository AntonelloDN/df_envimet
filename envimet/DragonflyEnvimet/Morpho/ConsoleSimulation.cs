using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Diagnostics;
using envimentFileManagement;

namespace DragonflyEnvimet
{
    public class ConsoleSimulation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ConsoleSimulation()
          : base("DF Envimet Run Simulation", "DFEnvimetRunSimulation",
              "Use this component to run directly simulation by Grasshopper.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_03_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_envimetFolder", "_envimetFolder", "Envimet project folder which comes from \"DF Envimet Manage Workspace\".", GH_ParamAccess.item);
            pManager.AddTextParameter("_SIMXfileAddress", "_SIMXfileAddress", "Connect the output of DF Envimet Config.", GH_ParamAccess.item);
            pManager.AddTextParameter("ENVImetInstallFolder_", "ENVImetInstallFolder_", "Optional folder path for ENVImet4 installation folder.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_type_", "_type_", "False = 32-bit or True = 64-bit. Default is 64-bit.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set runIt to \"True\" to run model.", GH_ParamAccess.item, false);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string envimetFolder = null;
            string INXfileAddress = null;
            string ENVImetInstallFolder = null;

            bool runIt = false;
            bool type = true;

            DA.GetData(0, ref envimetFolder);
            DA.GetData(1, ref INXfileAddress);
            DA.GetData(2, ref ENVImetInstallFolder);
            DA.GetData(3, ref type);
            DA.GetData(4, ref runIt);

            // exe

            // actions
            string mainDirectory = envimentFileManagement.WorkspaceFolderLB.FindENVI_MET(ENVImetInstallFolder);

            if (mainDirectory == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Envimet Main Folder not found!");
                runIt = false;
            }

            if (runIt == true)
            {
                string systemBase = (type) ? "win64" : "win32";
                string mainFolderWithBase = envimentFileManagement.WorkspaceFolderLB.CreateFolderWithBase(mainDirectory, systemBase);

                try
                {
                    envimentFileManagement.WorkspaceFolderLB.WriteBatchFile(mainFolderWithBase, envimetFolder, INXfileAddress);
                    Process.Start(envimentFileManagement.WorkspaceFolderLB.GetBatchFilePath(envimetFolder));
                }
                catch (System.IO.DirectoryNotFoundException dirNotFound)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"{dirNotFound} not found!");
                }
            }



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
                return DragonflyEnvimet.Properties.Resources.envimetRunSimulationIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3819c812-0047-4663-bc03-8990432d940e"); }
        }
    }
}