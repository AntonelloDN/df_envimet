using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;


namespace df_envimet.Grasshopper.ConfigFile
{
    public class CloudSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CloudSettings class.
        /// </summary>
        public CloudSettings()
          : base("DF Envimet Cloud Settings", "DFenvimetCloudSettings",
              "Add cloud to your model. The default setting is with no clouds. EXPERT SETTINGS.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("_lowClouds_", "_lowClouds_", "Fraction of LOW clouds (x/8). Default value is 0 (no clouds).", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_middleClouds_", "_middleClouds_", "Fraction of MIDDLE clouds (x/8). Default value is 0 (no clouds).", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_highClouds_", "_highClouds_", "Fraction of HIGH clouds (x/8). Default value is 0 (no clouds).", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("clouds", "clouds", "Cloud settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double _lowClouds_ = 0;
            double _middleClouds_ = 0;
            double _highClouds_ = 0;

            DA.GetData(0, ref _lowClouds_);
            DA.GetData(1, ref _middleClouds_);
            DA.GetData(2, ref _highClouds_);

            Cloud cloud = new Cloud()
            {
                LowClouds = _lowClouds_,
                MiddleClouds = _middleClouds_,
                HighClouds = _highClouds_
            };

            DA.SetData(0, cloud);
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
                return Properties.Resources.clouds;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fe43cca0-ac8e-4c90-9b48-bfc8a5ef31bc"); }
        }
    }
}