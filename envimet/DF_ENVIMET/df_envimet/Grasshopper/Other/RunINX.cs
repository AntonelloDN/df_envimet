using System;
using System.Diagnostics;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.Other
{
    public class RunINX : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RunINX class.
        /// </summary>
        public RunINX()
          : base("DF Envimet Run INX", "DFEnvimetRunINX",
              "Use this component to open your ENVI_MET model directly with GH.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.senary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("_INXfileAddress", "_INXfileAddress", "Connect the output of DF Envimet Spaces.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set runIt to \"True\" to run model.", GH_ParamAccess.item, false);
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
            string _INXfileAddress = String.Empty;
            bool _runIt = false;

            DA.GetData(0, ref _INXfileAddress);
            DA.GetData(1, ref _runIt);

            // exe
            if (_runIt)
                if (_INXfileAddress != null)
                    try
                    {
                        Process.Start(_INXfileAddress);
                    }
                    catch
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Connect a valid path of inx file.");
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
                return Properties.Resources.envimetRunINXIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fa9ed0d3-82a7-4223-9f9f-1178ad35d848"); }
        }
    }
}