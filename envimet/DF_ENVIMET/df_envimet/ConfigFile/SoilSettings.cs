using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MorphoEnvimetLibrary.Settings;

namespace df_envimet.ConfigFile
{
    public class SoilSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SoilSettings class.
        /// </summary>
        public SoilSettings()
          : base("DF Envimet Soil settings", "DFenvimetSoilSettings",
              "EXPERT SETTINGS: This component let you change initial condition of the soil.",
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
            pManager.AddNumberParameter("_tempUpperlayer_", "_tempUpperlayer_", "Temperature [K] of the upper layer (0-20cm). Default value is 293.00000 K.", GH_ParamAccess.item, 293.00000);
            pManager.AddNumberParameter("_tempMiddlelayer_", "_tempMiddlelayer_", "Temperature [K] of the middle layer (20-50cm). Default value is 293.00000 K.", GH_ParamAccess.item, 293.00000);
            pManager.AddNumberParameter("_tempDeeplayer_", "_tempDeeplayer_", "Temperature [K] of the deep layer (50-200cm). Default value is 293.00000 K.", GH_ParamAccess.item, 293.00000);
            pManager.AddNumberParameter("_tempBedrockLayer_", "_tempBedrockLayer_", "Temperature [K] of the bedrock layer (below 200cm). Default value is 293.00000 K.", GH_ParamAccess.item, 293.00000);

            pManager.AddIntegerParameter("_waterUpperlayer_", "_waterUpperlayer_", "Soil Humidity [%] of the upper layer (below 200cm). Default value is 70%.", GH_ParamAccess.item, 70);
            pManager.AddIntegerParameter("_waterMiddlelayer_", "_waterMiddlelayer_", "Soil Humidity [%] of the middle layer (below 200cm). Default value is 75%.", GH_ParamAccess.item, 75);
            pManager.AddIntegerParameter("_waterDeeplayer_", "_waterDeeplayer_", "Soil Humidity [%] of the deep layer (below 200cm). Default value is 75%.", GH_ParamAccess.item, 75);
            pManager.AddIntegerParameter("_waterBedrocklayer_", "_waterBedrocklayer_", "Soil Humidity [%] of the bedrock layer (below 200cm). Default value is 75%.", GH_ParamAccess.item, 75);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("soilSettings", "soilSettings", "Soil settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            double tul = 293.00000;
            double tml = 293.00000;
            double tdl = 293.00000;
            double tbl = 293.00000;

            int hul = 70;
            int hml = 75;
            int hdl = 75;
            int hbl = 75;

            DA.GetData(0, ref tul);
            DA.GetData(1, ref tml);
            DA.GetData(2, ref tdl);
            DA.GetData(3, ref tbl);
            DA.GetData(4, ref hul);
            DA.GetData(5, ref hml);
            DA.GetData(6, ref hdl);
            DA.GetData(7, ref hbl);


            // actions
            SoilTemp lbcSettings = new SoilTemp()
            {
                TempUpperlayer = tul
                ,TempMiddlelayer = tml
                ,TempDeeplayer = tdl
                ,TempBedrockLayer = tbl
                ,WaterUpperlayer = hul
                ,WaterMiddlelayer = hml
                ,WaterDeeplayer = hdl
                ,WaterBedrockLayer = hbl
            };

            // OUTPUT
            DA.SetData(0, lbcSettings);

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
                return Properties.Resources.envimetSoilSettingsIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("befe2977-ac80-4458-9a64-81a31e261ca6"); }
        }
    }
}