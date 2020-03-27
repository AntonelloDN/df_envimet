using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Grasshopper.Modeling
{
    public class Plant2d : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plant2d class.
        /// </summary>
        public Plant2d()
          : base("DF Envimet 2d Plant", "DFEnvimet2dPlant",
              "Use this component to generate plant2d for \"Dragonfly Envimet Spaces\". E.g. grass, simple tree of 10 meters and so on.",
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
            pManager.AddMeshParameter("_plant2D", "_plant2D", "Geometry that represent ENVI-Met plant 2d.  Geometry must be a Surface or Brep on xy plane.", GH_ParamAccess.item);
            pManager.AddTextParameter("_plantId_", "_plantId_", "ENVI-Met plant id. You can use \"id outputs\" which comes from \"DF Envimet Read Library\".\nDefault is 0000XX.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimet2dPlants", "envimet2dPlants", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add 3d plants to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _plant2D = null;
            string _plantId_ = df_envimet_lib.Geometry.Material.CommonPlant2dMaterial;

            DA.GetData<Mesh>(0, ref _plant2D);
            DA.GetData<string>(1, ref _plantId_);
            
            // actions
            df_envimet_lib.Geometry.Material material = new df_envimet_lib.Geometry.Material
            {
                Custom2dMaterial = _plantId_
            };

            df_envimet_lib.Geometry.Plant2d trees = new df_envimet_lib.Geometry.Plant2d(_plant2D, material);

            // OUTPUT
            DA.SetData(0, trees);
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
                return Properties.Resources.envimet2dPlantIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("109e606e-066b-4507-ac57-661bd93fe187"); }
        }
    }
}