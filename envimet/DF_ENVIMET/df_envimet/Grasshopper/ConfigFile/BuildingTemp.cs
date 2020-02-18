using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class BuildingTemp : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BuildingTemp class.
        /// </summary>
        public BuildingTemp()
          : base("DF Envimet Building Temp", "DFenvimetBuildingTemp",
              "This component let you change the indoor temperature of the buildings.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_19_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("_indoorTemp_", "_indoorTemp_", "Indoor temperature in Kelvin [K]. Default value is 293.00.", GH_ParamAccess.item, 293.00);
            pManager.AddIntegerParameter("_indoorConst_", "_indoorConst_", "Keep the building indoor temperature constant.\nConnect an integer: 0 = false; 1 = true. Default is 0.", GH_ParamAccess.item, 0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("buildingTemp", "buildingTemp", "building Temperature settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            double _indoorTemp_ = 293.00;
            int _indoorConst_ = 0;

            DA.GetData(0, ref _indoorTemp_);
            DA.GetData(1, ref _indoorConst_);

            BuildingSetting buildingTemp = new BuildingSetting()
            {
                IndoorTemp = _indoorTemp_,
                IndoorConst = _indoorConst_
            };

            // OUTPUT
            DA.SetData(0, buildingTemp);
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
                return Properties.Resources.envimetBuildingTempIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4770a642-120e-448d-b170-1eb1da758994"); }
        }
    }
}