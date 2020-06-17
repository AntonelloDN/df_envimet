using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class SourceSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SourceSettings class.
        /// </summary>
        public SourceSettings()
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
            pManager.AddTextParameter("_pollutantName", "_pollutantName", "Name of pollutant source [string].", GH_ParamAccess.item);
            pManager.AddIntegerParameter("_pollutantType", "_pollutantType", "Type of pollutant source. Connect an integer to set pollutant type:\n0 = PM" +
                "\n1 = CO" +
                "\n2 = CO2" +
                "\n3 = NO" +
                "\n4 = NO2" +
                "\n5 = SO2" +
                "\n6 = NH3" +
                "\n7 = H2O2" +
                "\n8 = SPRAY", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_multipleSources_", "_multipleSources_", "Set it to 'True' to set multi pollutant. [bool] Default value is single pollutant.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("_activeChem_", "_activeChem_", "Set it to 'True' to set dispersion and active chemistry. [bool] Default value is dispersion only.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("_diameter_", "_diameter_", "Particle diameter (μm) [float]. Default is 10.00.\nDefine it if you select SPRAY or PM.", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("_density_", "_density_", "Particle density (g/cm3) [float]. Default is 1.00.", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sourceSettings", "sourceSettings", "Source settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string _pollutantName = String.Empty;
            int _pollutantType = 0;
            bool _multipleSources_ = false;
            bool _activeChem_ = false;
            double _diameter_ = 10.0;
            double _density_ = 1.0;

            DA.GetData(0, ref _pollutantName);
            DA.GetData(1, ref _pollutantType);
            DA.GetData(2, ref _multipleSources_);
            DA.GetData(3, ref _activeChem_);
            DA.GetData(4, ref _diameter_);
            DA.GetData(5, ref _density_);

            Active multipleSources = (_multipleSources_) ? Active.YES : Active.NO;
            Active activeChem = (_activeChem_) ? Active.YES : Active.NO;

            Sources sources = new Sources(_pollutantName, PollutantWrapper(_pollutantType), multipleSources, activeChem)
            {
                UserPartDiameter = _diameter_,
                UserPartDensity = _density_
            };

            DA.SetData(0, sources);
        }

        private Pollutant PollutantWrapper(int index)
        {
            Pollutant pollutant;

            switch (index)
            {
                case 0:
                    pollutant = Pollutant.PM;
                    break;
                case 1:
                    pollutant = Pollutant.CO;
                    break;
                case 2:
                    pollutant = Pollutant.CO2;
                    break;
                case 3:
                    pollutant = Pollutant.NO;
                    break;
                case 4:
                    pollutant = Pollutant.NO2;
                    break;
                case 5:
                    pollutant = Pollutant.SO2;
                    break;
                case 6:
                    pollutant = Pollutant.NH3;
                    break;
                case 7:
                    pollutant = Pollutant.H2O2;
                    break;
                case 8:
                    pollutant = Pollutant.SPRAY;
                    break;
                default:
                    pollutant = Pollutant.PM;
                    break;

            }
            return pollutant;
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
                return Properties.Resources.sourceset;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ab96d80b-9388-4292-a83f-024af185ce7f"); }
        }
    }
}