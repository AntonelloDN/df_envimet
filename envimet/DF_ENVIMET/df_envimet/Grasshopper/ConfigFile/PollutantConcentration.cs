using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class PollutantConcentration : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PollutantConcentration class.
        /// </summary>
        public PollutantConcentration()
          : base("DF Envimet Pollutant Concentration Settings", "DFenvimetPollutantConcentrationSettings",
              "Set the pollutant concentration. Similar to the CO2 background concentration in the atmosphere (400 ppm), there might be a specific pollutant background concentration for your simulated area. EXPERT SETTINGS.",
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
            pManager.AddNumberParameter("_NO_", "_NO_", "NO concentration in ppm [float].", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_NO2_", "_NO2_", "NO2 concentration in ppm [float].", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_ozone_", "_ozone_", "Ozone concentration in ppm [float].", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_PM10_", "_PM10_", "PM 10 concentration in ppm [float].", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_PM25_", "_PM25_", "PM 2.5 concentration in ppm [float].", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("_user_pollutant_", "_user_pollutant_", "User defined pollutant concentration in ppm [float].", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("pollutantConcentration", "pollutantConcentration", "Pollutant concentration settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double _NO_ = 0;
            double _NO2_ = 0;
            double _ozone_ = 0;
            double _PM10_ = 0;
            double _PM25_ = 0;
            double _user_pollutant_ = 0;

            DA.GetData(0, ref _NO_);
            DA.GetData(1, ref _NO2_);
            DA.GetData(2, ref _ozone_);
            DA.GetData(3, ref _PM10_);
            DA.GetData(4, ref _PM25_);
            DA.GetData(5, ref _user_pollutant_);

            Background pollutantConcentration = new Background()
            {
                UserSpec = _user_pollutant_,
                No = _NO_,
                No2 = _NO2_,
                O3 = _ozone_,
                Pm10 = _PM10_,
                Pm25 = _PM25_
            };

            DA.SetData(0, pollutantConcentration);
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
                return Properties.Resources.pollutant_set;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("db5977b5-0acd-4c5c-8549-0a393f4a0367"); }
        }
    }
}