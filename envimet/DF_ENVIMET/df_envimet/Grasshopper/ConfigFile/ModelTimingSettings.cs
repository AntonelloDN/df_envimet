using System;
using System.Collections.Generic;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class ModelTimingSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ModelTimingSettings class.
        /// </summary>
        public ModelTimingSettings()
          : base("DF Envimet Model interval Settings", "DFenvimetModelintervalSettings",
              "Force timing conditions of envimet objects.",
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
            pManager.AddIntegerParameter("_surfaceStep_", "_surfaceStep_", "Update Surface Data each ? sec. [float] Default value is 30.00 seconds.", GH_ParamAccess.item, 30);
            pManager.AddIntegerParameter("_flowStep_", "_flowStep_", "Update Wind field each ? sec. [float] Default value is 900.00 seconds.", GH_ParamAccess.item, 900);
            pManager.AddIntegerParameter("_radiationStep_", "_radiationStep_", "Update Radiation and Shadows each ? sec. [float] Default value is 600.00 seconds.", GH_ParamAccess.item, 600);
            pManager.AddIntegerParameter("_plantStep_", "_plantStep_", "Update Plant Data each ? sec. [float] Default value is 600.00 seconds.", GH_ParamAccess.item, 600);
            pManager.AddIntegerParameter("_sourceStep_", "_sourceStep_", "Update Emmission Data each ? sec. [float] Default value is 600.00 seconds.", GH_ParamAccess.item, 600);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("modelTimingSettings", "modelTimingSettings", "Model Timing settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int _surfaceStep_ = 30;
            int _flowStep_ = 900;
            int _radiationStep_ = 600;
            int _plantStep_ = 600;
            int _sourceStep_ = 600;

            DA.GetData(0, ref _surfaceStep_);
            DA.GetData(1, ref _flowStep_);
            DA.GetData(2, ref _radiationStep_);
            DA.GetData(3, ref _plantStep_);
            DA.GetData(4, ref _sourceStep_);

            var modelTiming = new ModelTiming()
            {
                SurfaceSteps = _surfaceStep_,
                FlowSteps = _flowStep_,
                RadiationSteps = _radiationStep_,
                PlantSteps = _plantStep_,
                SourcesSteps = _sourceStep_
            };

            DA.SetData(0, modelTiming);
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
                return Properties.Resources.out_set;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a7a0cdc4-e513-4e43-9783-835e88e334f0"); }
        }
    }
}