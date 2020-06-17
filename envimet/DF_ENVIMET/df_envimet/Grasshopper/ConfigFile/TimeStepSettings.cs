using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class TimeStepSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TimeStepSettings class.
        /// </summary>
        public TimeStepSettings()
          : base("DF Envimet Timesteps Settings", "DFenvimetTimestepsSettings",
              "This component let you change the timestep of the sun. For more info see the official website of ENVI_MET.",
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
            pManager.AddNumberParameter("_sunheightDelta0_", "_sunheightDelta0_", "Sun height for switching dt(0). Defualt vale is 40.00 deg.\nmeaning = From 0 deg to 40.00 deg the time step of sun will be _timeStepInterval1_.", GH_ParamAccess.item, 40.00);
            pManager.AddNumberParameter("_sunheightDelta1_", "_sunheightDelta1_", "Sun height for switching dt(1). Defualt vale is 50.00 deg.", GH_ParamAccess.item, 50.00);
            pManager.AddNumberParameter("_timeStepInterval1_", "_timeStepInterval1_", "Time step (s) for interval 1 dt(0). Default value is 2 seconds.", GH_ParamAccess.item, 2.00);
            pManager.AddNumberParameter("_timeStepInterval2_", "_timeStepInterval2_", "Time step (s) for interval 1 dt(0). Default value is 2 seconds.", GH_ParamAccess.item, 2.00);
            pManager.AddNumberParameter("_timeStepInterval3_", "_timeStepInterval3_", "Time step (s) for interval 1 dt(0). Default value is 1 seconds.", GH_ParamAccess.item, 1.00);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("timestepsSettings", "timestepsSettings", "Timestep settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double _sunheightDelta0_ = 40.00;
            double _sunheightDelta1_ = 50.00;
            double _timeStepInterval1_ = 2.00;
            double _timeStepInterval2_ = 2.00;
            double _timeStepInterval3_ = 1.00;

            DA.GetData(0, ref _sunheightDelta0_);
            DA.GetData(1, ref _sunheightDelta1_);
            DA.GetData(2, ref _timeStepInterval1_);
            DA.GetData(3, ref _timeStepInterval2_);
            DA.GetData(4, ref _timeStepInterval3_);

            TimeSteps timestepsSettings = new TimeSteps()
            {
                SunheightStep01 = _sunheightDelta0_,
                SunheightStep02 = _sunheightDelta1_,
                DtStep00 = _timeStepInterval1_,
                DtStep01 = _timeStepInterval2_,
                DtStep02 = _timeStepInterval3_
            };

            DA.SetData(0, timestepsSettings);
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
                return Properties.Resources.envimetTimestepsTempIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4177394a-f440-4f13-872e-35b5e97cc9c6"); }
        }
    }
}