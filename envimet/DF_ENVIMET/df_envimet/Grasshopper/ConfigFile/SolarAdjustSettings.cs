using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class SolarAdjustSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SolarAdjust class.
        /// </summary>
        public SolarAdjustSettings()
          : base("DF Envimet Solar Adjust Settings", "DFenvimetSolarAdjustSettings",
              "Set sw factor. Modify the irradiation of mode area(W) by increasing or decreasing the solar adjustment factor. If you use FullForcing do not use this settings.",
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
            pManager.AddNumberParameter("_swFactor", "_swFactor", "Solar adjustment factor to apply. Connect a float in range (0.5, 1.50) [float].", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("solarAdjust", "solarAdjust", "Solar adjust settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double _swFactor = 0.5;

            DA.GetData(0, ref _swFactor);

            SolarAdjust solarAdjust = new SolarAdjust(_swFactor);

            DA.SetData(0, solarAdjust);
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
                return Properties.Resources.swfactor;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b69a99e8-13c9-4884-a527-a073a82b977e"); }
        }
    }
}