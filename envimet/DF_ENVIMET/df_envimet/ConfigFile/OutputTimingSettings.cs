using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MorphoEnvimetLibrary.Settings;

namespace df_envimet.ConfigFile
{
    public class OutputTimingSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the OutputTimingSettings class.
        /// </summary>
        public OutputTimingSettings()
          : base("DF Envimet Output interval Settings", "DFenvimetOutputintervalSettings",
              "This component let you change the timestep of the sun. For more info see the official website of ENVI_MET.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_19_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("_outputIntervalMainFile_", "_outputIntervalMainFile_", "Output interval main files (min). Default value is 60.00 min (hour by hour you have an output file).", GH_ParamAccess.item, 60);
            pManager.AddIntegerParameter("_outputIntervalText_", "_outputIntervalText_", "Output interval text output files (min). Default value is 30.00 min.", GH_ParamAccess.item, 30);
            pManager.AddBooleanParameter("_includeNestingGrids_", "_includeNestingGrids_", "Include Nesting Grids in Output (0:N,1:Y). Connect a boolean toggle to set it.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("outputSettings", "outputSettings", "Output Timing settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            int outputIntervalMainFile = 60;
            int outputIntervalText = 30;
            bool includeNesting = false;

            DA.GetData(0, ref outputIntervalMainFile);
            DA.GetData(1, ref outputIntervalText);
            DA.GetData(2, ref includeNesting);


            // actions
            OutputTiming outputSettings = new OutputTiming()
            {
                MainFiles = outputIntervalMainFile,
                TextFiles = outputIntervalText,

            };

            outputSettings.InclNestingGrids = (includeNesting) ? 1 : 0;

            // OUTPUT
            DA.SetData(0, outputSettings);

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
                return Properties.Resources.envimetOutputTimingIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("71d6e3c3-9ee5-47a9-9538-42a29bfc5b2c"); }
        }
    }
}