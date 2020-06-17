using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class ParallelCPUsettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ParallelCPUsettings class.
        /// </summary>
        public ParallelCPUsettings()
          : base("DF Envimet Parallel CPU Settings", "DFenvimetParallelCPUSettings",
              "Set parallel calculation with your CPU.",
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
            pManager.AddBooleanParameter("_active", "_active", "Run parallel calculation [bool].", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("parallelCPU", "parallelCPU", "Parallel CPU settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool _active = true;

            DA.GetData(0, ref _active);

            ParallelCPU parallel = new ParallelCPU();

            DA.SetData(0, parallel);
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
                return Properties.Resources.parallelcpu;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dc30f205-9ec4-428e-bc48-dc03b46ad784"); }
        }
    }
}