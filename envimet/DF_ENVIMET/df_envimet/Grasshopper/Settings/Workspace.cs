﻿using System;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.Settings
{
    public class Workspace : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Workspace()
          : base("DF Envimet Manage Workspace", "DFEnvimetManageWorkspace",
              "Use this component to create a Workspace folder",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_workspaceFolder", "_workspaceFolder", "Main folder where you have to save an Envimet project.", GH_ParamAccess.item);
            pManager.AddTextParameter("_projectName_", "_projectName_", "Name of Envimet project folder where you have to save:\n1) EnviMet geometry file(*.INX)\n2) Configuration file(*.SIMX).\nYou can also open and edit config file with ENVI_MET Guide module.", GH_ParamAccess.item, "df_envimet");
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("envimetFolder", "envimetFolder", "Envimet project folder. Connect it to \"_envimetFolder\" input of Dragonfly Envimet Spaces", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string _workspaceFolder = null;
            string _projectName_ = "df_envimet";

            DA.GetData(0, ref _workspaceFolder);
            DA.GetData(1, ref _projectName_);

            // actions
            string mainDirectory = df_envimet_lib.IO.Workspace.FindENVI_MET();

            if (mainDirectory != null)
            {
                df_envimet_lib.IO.Workspace dirEnvimetModel = new df_envimet_lib.IO.Workspace(_workspaceFolder, _projectName_);
                string fullFolder = dirEnvimetModel.WorkspaceFolderLBwrite(mainDirectory);

                DA.SetData(0, fullFolder);
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Envimet Main Folder not found! Use DF Envimet Installation Directory to set installation folder of envimet.");
                return;
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
                return Properties.Resources.envimetWorkspaceFolder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f4ffa46d-1fae-4416-b264-de90fab38902"); }
        }
    }
}