using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Settings
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
            pManager.AddTextParameter("commonWallMaterial_", "commonWallMaterial_", "Default wall material.", GH_ParamAccess.item, MorphoEnvimetLibrary.Geometry.Material.CommonWallMaterial);
            pManager.AddTextParameter("commonRoofMaterial_", "commonRoofMaterial_", "Default roof material.", GH_ParamAccess.item, MorphoEnvimetLibrary.Geometry.Material.CommonRoofMaterial);
            pManager.AddTextParameter("commonSoilMaterial_", "commonSoilMaterial_", "Default soil material.", GH_ParamAccess.item, MorphoEnvimetLibrary.Geometry.Material.CommonSoilMaterial);
            pManager.AddTextParameter("commonPlant2dMaterial_", "commonPlant2dMaterial_", "Default plant2d material.", GH_ParamAccess.item, MorphoEnvimetLibrary.Geometry.Material.CommonPlant2dMaterial);
            pManager.AddBooleanParameter("resetValues_", "resetValues_", "Set it to true if you want to reset default values.", GH_ParamAccess.item, false);
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
            string commonWallMaterial_ = MorphoEnvimetLibrary.Geometry.Material.CommonWallMaterial;
            string commonRoofMaterial_ = MorphoEnvimetLibrary.Geometry.Material.CommonRoofMaterial;
            string commonSoilMaterial_ = MorphoEnvimetLibrary.Geometry.Material.CommonSoilMaterial;
            string commonPlant2dMaterial_ = MorphoEnvimetLibrary.Geometry.Material.CommonPlant2dMaterial;
            bool reset = false;

            DA.GetData(0, ref commonWallMaterial_);
            DA.GetData(1, ref commonRoofMaterial_);
            DA.GetData(2, ref commonSoilMaterial_);
            DA.GetData(3, ref commonPlant2dMaterial_);
            DA.GetData(4, ref reset);

            // start
            MorphoEnvimetLibrary.Geometry.Material.CommonWallMaterial = commonWallMaterial_;
            MorphoEnvimetLibrary.Geometry.Material.CommonRoofMaterial = commonRoofMaterial_;
            MorphoEnvimetLibrary.Geometry.Material.CommonSoilMaterial = commonSoilMaterial_;
            MorphoEnvimetLibrary.Geometry.Material.CommonPlant2dMaterial = commonPlant2dMaterial_;
            if (reset)
                MorphoEnvimetLibrary.Geometry.Material.ResetValue();
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