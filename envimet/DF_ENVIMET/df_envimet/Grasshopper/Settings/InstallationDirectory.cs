using System;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.Settings
{
    public class InstallationDirectory : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the InstallationDirectory class.
        /// </summary>
        public InstallationDirectory()
          : base("DF Envimet Installation Directory", "DFEnvimetInstallationDirectory",
              "Use this component to set Installation Directory of Envimet on your machine. E.g. C:\\ENVImet444",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("envimetInstallationFolder_", "envimetInstallationFolder_", "Directory where your Envimet software is. C:\\ENVImet444", GH_ParamAccess.item, "C:\\" + df_envimet_lib.IO.Workspace.DEFAULT_FOLDER);
            pManager[0].Optional = true;
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
            string envimetInstallationFolder_ = df_envimet_lib.IO.Workspace.DEFAULT_FOLDER;

            DA.GetData(0, ref envimetInstallationFolder_);

            if (envimetInstallationFolder_ != null)
            {
                df_envimet_lib.IO.Workspace.ENVImetInstallFolder = envimetInstallationFolder_;
                if (df_envimet_lib.IO.Workspace.FindENVI_MET() == null)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please provide a valid directory");
                    return;
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
                return Properties.Resources.envimetInstallationDirectoryIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("82325eff-f7ad-418e-a132-f025408bd720"); }
        }
    }
}