using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class TThreadSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TThreadSettings class.
        /// </summary>
        public TThreadSettings()
          : base("DF Envimet Thread Settings", "DFenvimetThreadSettings",
              "This component let you force threading - expert only.",
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
            pManager.AddBooleanParameter("_forceTreading", "_forceTreading", "Run Threaded. If it is 'True' simulation will stay responsive.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("tthread", "tthread", "Tthread settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool _forceTreading = false;

            DA.GetData(0, ref _forceTreading);

            Active isActive = (_forceTreading) ? Active.YES : Active.NO;

            var tthread = new TThread(isActive);

            DA.SetData(0, tthread);
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
                return Properties.Resources.tthread;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2743c756-cd2a-4776-ad00-4a246da8d2a9"); }
        }
    }
}