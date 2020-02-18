using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using df_envimet_lib.Geometry;


namespace df_envimet.Grasshopper.Modeling
{
    public class Building : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Building class.
        /// </summary>
        public Building()
          : base("DF Envimet Buildings", "DFEnvimetBuildings",
              "Use this component to generate buildings for \"Dragonfly Envimet Spaces\". Make sure buildings are " +
                "clean geometries otherwise you can use boolean function to merge parts together.\nAnother possibility is to use tools like MorphoMesh or similar to work with meshes performing complex boolean union operations.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_geometry", "_geometry", "Geometry that represent ENVI-Met building.\nGeometry has to be closed Brep or Mesh", GH_ParamAccess.item);
            pManager.AddTextParameter("wallMaterial_", "wallMaterial_", "Use this input to change wall materials.\nMaterials count have to be equals buildings count, otherwise it will be set the default material.", GH_ParamAccess.item, Material.CommonWallMaterial);
            pManager.AddTextParameter("roofMaterial_", "roofMaterial_", "Use this input to change roof materials. Materials count have to be equals buildings count, otherwise it will be set the default material.", GH_ParamAccess.item, Material.CommonRoofMaterial);
            pManager.AddTextParameter("greenWallMaterial_", "greenWallMaterial_", "Connect a list of greenings material to apply them to walls of selected buildings.", GH_ParamAccess.item);
            pManager.AddTextParameter("greenRoofMaterial_", "greenRoofMaterial_", "Connect a list of greenings material to apply them to roofs of selected buildings.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("simplifiedCalculation_", "simplifiedCalculation_", "Connect a boolean to simplify calculation.", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetBuiding", "envimetBuiding", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add buildings to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _geometry = null;
            bool simplifiedCalculation_ = false;
            string wallMaterial_ = null;
            string roofMaterial_ = null; 

            string greenWallMaterial_ = null;
            string greenRoofMaterial_ = null; 

            DA.GetData(0, ref _geometry);
            DA.GetData(1, ref wallMaterial_);
            DA.GetData(2, ref roofMaterial_);
            DA.GetData(3, ref greenWallMaterial_);
            DA.GetData(4, ref greenRoofMaterial_);
            DA.GetData(5, ref simplifiedCalculation_);

            if (wallMaterial_ == null)
                 wallMaterial_ = Material.CommonWallMaterial;
            if (roofMaterial_ == null)
                roofMaterial_ = Material.CommonRoofMaterial;
            if (greenWallMaterial_ == null)
                greenWallMaterial_ = Material.DefaultGreenWallMaterial;
            if (greenRoofMaterial_ == null)
                greenRoofMaterial_ = Material.DefaultGreenRoofMaterial;

            Material material = new Material()
            {
                WallMaterial = wallMaterial_,
                RoofMaterial = roofMaterial_,
                GreenWallMaterial = greenWallMaterial_,
                GreenRoofMaterial = greenRoofMaterial_
            };
            // actions
            try
            {
                df_envimet_lib.Geometry.Building envimetBuilding = new df_envimet_lib.Geometry.Building(_geometry, material);
                envimetBuilding.SimplifiedCalculation = simplifiedCalculation_;

                if (!(_geometry.IsClosed))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please provide closed geometries");
                    return;
                }

                // OUTPUT
                //DA.SetData(0, (object)envimetBuildings);
                DA.SetData(0, envimetBuilding);

            }
            catch
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something went wrong... connect valid geometries.");
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
                return Properties.Resources.envimetBuildingsIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("aefc6af9-e6ad-4bdb-af42-385df681e32d"); }
        }
    }
}