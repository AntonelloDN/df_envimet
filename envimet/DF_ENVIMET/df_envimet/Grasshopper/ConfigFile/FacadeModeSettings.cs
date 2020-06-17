using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;
using System.Linq;
using System.Collections.Generic;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class FacadeModeSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FacadeModeSettings class.
        /// </summary>
        public FacadeModeSettings()
          : base("DF Envimet Wind Resistence settings", "DFenvimetWindResistenceSettings",
              "EXPERT SETTINGS: Set wind resistance model at facede. DIN 6946 includes a higher Z0 value.",
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
            pManager.AddIntegerParameter("_modelType", "_modelType", "Connect an integer to select wind resistance model." +
                "\n0 = MO" +
                "\n1 = DIN6946", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("windResistance", "windResistance", "Wind resistance settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int _modelType = 0;

            DA.GetData(0, ref _modelType);

            List<FacadeMod> turbolenceType = Enum.GetValues(typeof(FacadeMod))
                .Cast<FacadeMod>()
                .ToList();

            Facades facadeWind = new Facades(turbolenceType[_modelType]);

            DA.SetData(0, facadeWind);
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
                return Properties.Resources.wind_res;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("03ba465b-a4a2-4419-b3f6-613f4e0c6a72"); }
        }
    }
}