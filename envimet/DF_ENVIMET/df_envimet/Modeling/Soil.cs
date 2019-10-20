using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Modeling
{
    public class Soil : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Soil class.
        /// </summary>
        public Soil()
          : base("DF Envimet Soil", "DFEnvimetSoil",
              "Use this component to generate inputs for \"df_envimet Envimet Spaces\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_19_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_soil", "_soil", "Geometry that represent ENVI-Met soil.  Geometry must be a Surface or Brep on xy plane.", GH_ParamAccess.item);
            pManager.AddTextParameter("_profileId_", "_profileId_", "ENVI-Met profile id. You can use \"id outputs\" which comes from \"LB ENVI - Met Read Library\"\nDefault is 000000.", GH_ParamAccess.item, "000000");
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetSoils", "envimetSoils", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add soils to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _soil = null;
            string _profileId_ = MorphoEnvimetLibrary.Geometry.Material.CommonSoilMaterial;

            DA.GetData<Mesh>(0, ref _soil);
            DA.GetData<string>(1, ref _profileId_);


            // actions
            MorphoEnvimetLibrary.Geometry.Material material = new MorphoEnvimetLibrary.Geometry.Material
            {
                Custom2dMaterial = _profileId_
            };

            MorphoEnvimetLibrary.Geometry.Soil soil = new MorphoEnvimetLibrary.Geometry.Soil(_soil, material);


            // OUTPUT
            DA.SetData(0, soil);
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
                return Properties.Resources.envimetSoilIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("72280d88-4505-4811-810e-baea19c1a370"); }
        }
    }
}