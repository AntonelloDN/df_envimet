using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class SimpleForcing : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SimpleForcing class.
        /// </summary>
        public SimpleForcing()
          : base("DF Envimet Config SimpleForcing", "DFenvimetConfigSimpleForcing",
              "This component let you force climate condition of the simulation. You can connect lists of values or data which comes from EPW file.\nUse outputs of DF Envimet Simple Force by EPW.",
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
            pManager.AddNumberParameter("_dryBulbTemperature", "_dryBulbTemperature", "Connect a list of numbers.\nUnit is Kelvin.", GH_ParamAccess.list);
            pManager.AddNumberParameter("_relativeHumidity", "_relativeHumidity", "Connect a list of numbers.\nUnit is %.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("simpleForcing", "simpleForcing", "Simple forcing settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            List<double> _dryBulbTemperature = new List<double>();
            List<double> _relativeHumidity = new List<double>();

            DA.GetDataList<double>(0, _dryBulbTemperature);
            DA.GetDataList<double>(1, _relativeHumidity);

            if (_dryBulbTemperature.Count == _relativeHumidity.Count)
            {
                SampleForcingSettings simpleF = new SampleForcingSettings(_dryBulbTemperature, _relativeHumidity);
                DA.SetData(0, simpleF);
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Lists must be same length.");
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
                return Properties.Resources.envimetConfigSimpleForcing;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ca6192aa-3bfb-47cd-b189-076bf4cb0f32"); }
        }
    }
}