using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class TreeSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TreeSettings class.
        /// </summary>
        public TreeSettings()
          : base("DF Envimet Tree settings", "DFenvimetTreeSettings",
              "EXPERT SETTINGS: Set plant settings such as tree calendar and CO2.",
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
            pManager.AddIntegerParameter("_CO2_", "_CO2_", "CO2 background level (ppm)." +
                "\nThe default value of the CO2 background concentration is set to 400 ppm without this settings.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_leafTransmittance", "_leafTransmittance", "Set it to 'True' to use old calculation model. 'False' to use user defined value.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("_treeCalendar", "_treeCalendar", "Set it to 'True' to use tree calendar, 'False' to disable tree calendar." +
                "Tree calendar = foliage of your tree will be calculated depending on the month your simulation is set and the hemisphere where location is.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("plantSettings", "plantSettings", "Wind resistance settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int _CO2_ = 0;
            bool _leafTransmittance = false;
            bool _treeCalendar = false;

            DA.GetData(0, ref _CO2_);
            DA.GetData(1, ref _leafTransmittance);
            DA.GetData(2, ref _treeCalendar);

            Active leafTransmittance = (_leafTransmittance) ? Active.YES : Active.NO;
            Active treeCalendar = (_treeCalendar) ? Active.YES : Active.NO;

            PlantSetting plantsettings = new PlantSetting(leafTransmittance, treeCalendar, _CO2_);

            DA.SetData(0, plantsettings);
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
                return Properties.Resources.plant_set;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("933dbbac-4eaf-48cf-b476-3ff88025beb0"); }
        }
    }
}