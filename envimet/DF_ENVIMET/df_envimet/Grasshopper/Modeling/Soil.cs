using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Grasshopper.Modeling
{
    public class Soil : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Soil class.
        /// </summary>
        public Soil()
          : base("DF Envimet Soil", "DFEnvimetSoil",
              "Use this component to generate inputs for \"Dragonfly Envimet Spaces\"",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_soil", "_soil", "Geometry that represent ENVI-Met soil.  Geometry must be a Surface or Brep on xy plane.", GH_ParamAccess.item);
            pManager.AddTextParameter("_profileId_", "_profileId_", "ENVI-Met profile id. You can use \"id outputs\" which comes from \"DF Envimet Read Library\"\nDefault is 000000.", GH_ParamAccess.item);
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
            string _profileId_ = df_envimet_lib.Geometry.Material.CommonSoilMaterial;

            DA.GetData<Mesh>(0, ref _soil);
            DA.GetData<string>(1, ref _profileId_);

            // actions
            df_envimet_lib.Geometry.Material material = new df_envimet_lib.Geometry.Material
            {
                Custom2dMaterial = _profileId_
            };

            df_envimet_lib.Geometry.Soil soil = new df_envimet_lib.Geometry.Soil(_soil, material);


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