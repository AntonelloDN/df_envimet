using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class FullForcingSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FullForcing class.
        /// </summary>
        public FullForcingSettings()
          : base("DF Envimet Full Forcing settings", "DFenvimetFullForcingSettings",
              "Force boundary condition using EPW file.\nNOTE. Some EPW could not be compatible with Envimet FOX Manager. You get an error in case of incompatibility.", "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_epw", "_epw", "Absolute path of Epw file to use to create FOX file. E.g. C:/MyEpw/Example.epw", GH_ParamAccess.item);
            pManager.AddTextParameter("_envimetFolder", "_envimetFolder", "Folder where project is. Connect output of Workspace component.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_forceTemperature_", "_forceTemperature_", "Set it to 'True' to use temperature values of EPW as boundary condition [bool]. Default value is 'True'.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_forceWind_", "_forceWind_", "Set it to 'True' to use wind speed and direction values of EPW as boundary condition [bool]. Default value is 'True'.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_forceRelativeHumidity_", "_forceRelativeHumidity_", "Set it to 'True' to use relative humidity values of EPW as boundary condition [bool]. Default value is 'True'.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_forcePrecipitation_", "_forcePrecipitation_", "Set it to 'True' to use precipitation values of EPW as boundary condition [bool]. Default value is 'False'.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("_forceRadiationClouds_", "_forceRadiationClouds_", "Set it to 'True' to use radiation and cloudiness values of EPW as boundary condition [bool]. Default value is 'True'.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("minFlowSteps_", "minFlowSteps_", "FOR EXPERT ONLY. Adjust the minimum interal for updating the Full Forcing inflow. Default value is 50.", GH_ParamAccess.item, 50);
            pManager.AddNumberParameter("limitWind2500_", "limitWind2500_", "FOR EXPERT ONLY. Limit of wind speed at 2500 meter. Default is 0.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("maxWind2500_", "maxWind2500_", "FOR EXPERT ONLY. Max wind speed at 2500 meter. Default is 20 m/s.", GH_ParamAccess.item, 20);
            pManager.AddTextParameter("envimetInstallationFolder_", "envimetInstallationFolder_", "Path where ENVI-Met software is. Use it only if ENVI-Met is not recognized automatically.", GH_ParamAccess.item);

            pManager[10].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("fullForcing", "fullForcing", "Full Forcing settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _epw = null;
            string _envimetFolder = null;
            bool _forceTemperature_ = true;
            bool _forceWind_ = true;
            bool _forceRelativeHumidity_ = true;
            bool _forcePrecipitation_ = false;
            bool _forceRadiationClouds_ = true;
            int minFlowSteps_ = 50;
            double limitWind2500_ = 0;
            double maxWind2500_ = 20;
            string envimetInstallationFolder_ = null;

            DA.GetData(0, ref _epw);
            DA.GetData(1, ref _envimetFolder);
            DA.GetData(2, ref _forceTemperature_);
            DA.GetData(3, ref _forceWind_);
            DA.GetData(4, ref _forceRelativeHumidity_);
            DA.GetData(5, ref _forcePrecipitation_);
            DA.GetData(6, ref _forceRadiationClouds_);
            DA.GetData(7, ref minFlowSteps_);
            DA.GetData(8, ref limitWind2500_);
            DA.GetData(9, ref maxWind2500_);
            DA.GetData(10, ref envimetInstallationFolder_);

            Active temperature = (_forceTemperature_) ? Active.YES : Active.NO;
            Active wind = (_forceWind_) ? Active.YES : Active.NO;
            Active relativeHumidity = (_forceRelativeHumidity_) ? Active.YES : Active.NO;
            Active precipitation = (_forcePrecipitation_) ? Active.YES : Active.NO;
            Active radiation = (_forceRadiationClouds_) ? Active.YES : Active.NO;

            FullForcing fullforcing = new FullForcing(_epw, _envimetFolder, envimetInstallationFolder_)
            {
                LimitWind2500 = 0,
                MaxWind2500 = 20.0,
                MinFlowsteps = 50,
                ForceTemperature = (int)temperature,
                ForceWind = (int)wind,
                ForceRelativeHumidity = (int)relativeHumidity,
                ForcePrecipitation = (int)precipitation,
                ForceRadClouds = (int)radiation
            };

            DA.SetData(0, fullforcing);
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
                return Properties.Resources.fullfor;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e3afc945-1493-4a91-92cf-47405a4489dd"); }
        }
    }
}