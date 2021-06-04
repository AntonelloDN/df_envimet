using System;
using System.Collections.Generic;
using System.Linq;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class Turbolence : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Turbolence class.
        /// </summary>
        public Turbolence()
          : base("DF Envimet Turbolence Settings", "DFenvimetTurbolenceSettings",
              "Set Turbolence model. EXPERT SETTINGS.",
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
            pManager.AddIntegerParameter("_modelType", "_modelType", "Connect an integer to select turbolence model." +
                "\n0 = MellorAndYamada" +
                "\n1 = KatoAndLaunder" +
                "\n2 = Lopez" +
                "\n3 = Bruse ENVI_MET", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("turbulence", "turbulence", "Turbolence settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int _modelType = 0;

            DA.GetData(0, ref _modelType);

            List<TurbolenceType> turbolenceType = Enum.GetValues(typeof(TurbolenceType))
                .Cast<TurbolenceType>()
                .ToList();

            Turbulence turbulence = new Turbulence(turbolenceType[_modelType]);

            DA.SetData(0, turbulence);
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
                return Properties.Resources.turbolence;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0c8bd080-0856-49ca-9b59-15ce40c3c28b"); }
        }
    }
}