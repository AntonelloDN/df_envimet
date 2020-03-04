using System;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.Settings
{
    public class DefaultMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DefaultMaterial class.
        /// </summary>
        public DefaultMaterial()
          : base("DF Envimet Default Material", "DFEnvimetDefaultMaterial",
              "Use this component to override common wall material, common roof material and common soil material inputs for \"Dragonfly Envimet Spaces\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("commonWallMaterial_", "commonWallMaterial_", "Default wall material.", GH_ParamAccess.item);
            pManager.AddTextParameter("commonRoofMaterial_", "commonRoofMaterial_", "Default roof material.", GH_ParamAccess.item);
            pManager.AddTextParameter("commonSoilMaterial_", "commonSoilMaterial_", "Default profile material.", GH_ParamAccess.item);
            pManager.AddTextParameter("commonPlant2dMaterial_", "commonPlant2dMaterial_", "Default plant2d material.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("resetValues_", "resetValues_", "Set it to true if you want to reset default values.", GH_ParamAccess.item, false);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string commonWallMaterial_ = df_envimet_lib.Geometry.Material.CommonWallMaterial;
            string commonRoofMaterial_ = df_envimet_lib.Geometry.Material.CommonRoofMaterial;
            string commonSoilMaterial_ = df_envimet_lib.Geometry.Material.CommonSoilMaterial;
            string commonPlant2dMaterial_ = df_envimet_lib.Geometry.Material.CommonPlant2dMaterial;
            bool reset = false;

            DA.GetData(0, ref commonWallMaterial_);
            DA.GetData(1, ref commonRoofMaterial_);
            DA.GetData(2, ref commonSoilMaterial_);
            DA.GetData(3, ref commonPlant2dMaterial_);
            DA.GetData(4, ref reset);

            // start
            df_envimet_lib.Geometry.Material.CommonWallMaterial = commonWallMaterial_;
            df_envimet_lib.Geometry.Material.CommonRoofMaterial = commonRoofMaterial_;
            df_envimet_lib.Geometry.Material.CommonSoilMaterial = commonSoilMaterial_;
            df_envimet_lib.Geometry.Material.CommonPlant2dMaterial = commonPlant2dMaterial_;
            if (reset)
                df_envimet_lib.Geometry.Material.ResetValue();
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
                return Properties.Resources.envimetEditBasicMaterialIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fe0491bb-653c-4436-b36f-82c4c137f352"); }
        }
    }
}