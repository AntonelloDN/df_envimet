using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class AverageInflow : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AverageInflow class.
        /// </summary>
        public AverageInflow()
          : base("DF Envimet Average Inflow Settings", "DFenvimetAverageInflowSettings",
              "Active averaged inflow, the air temperature change will be calculated with the average inflow values instead of avg values of each grid cell. EXPERT SETTINGS.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("_active", "_active", "Set it to 'True' to calculate air temperature with the average inflow values.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("avgInflow", "avgInflow", "AvgInflow settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool _active = true;

            DA.GetData(0, ref _active);

            Active active = (_active) ? Active.YES : Active.NO;

            InflowAvg inflowAvg = new InflowAvg(active);

            DA.SetData(0, inflowAvg);
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
                return Properties.Resources.avg_inflow;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a56d550d-5c0f-46a1-ba18-65403e8f5e78"); }
        }
    }
}