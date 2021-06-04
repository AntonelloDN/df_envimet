﻿using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Grasshopper.Modeling
{
    public class Plant3d : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plant3d class.
        /// </summary>
        public Plant3d()
          : base("DF Envimet 3d Plant", "DFEnvimet3dPlant",
              "Use this component to generate 3d trees for \"Dragonfly Envimet Spaces\". ENVI_MET has a big library of 3d tree divided into Coniferous and Deciduous",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_plant3D", "_plant3D", "Geometry that represent ENVI-Met plant 3d.  Geometry must be a Surface or Brep on xy plane.", GH_ParamAccess.item);
            pManager.AddTextParameter("_plant3Did_", "_plant3Did_", "ENVI-Met plant id. You can use \"id outputs\" which comes from \"DF Envimet Read Library\".\nDefault is PINETREE.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimet3dPlants", "envimet3dPlants", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add 3d plants to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _plant3D = null;
            string _plant3Did_ = df_envimet_lib.Geometry.Material.CommonPlant3dMaterial;

            DA.GetData<Mesh>(0, ref _plant3D);
            DA.GetData<string>(1, ref _plant3Did_);

            // actions
            df_envimet_lib.Geometry.Material material = new df_envimet_lib.Geometry.Material
            {
                Plant3dMaterial = _plant3Did_
            };

            df_envimet_lib.Geometry.Plant3d trees = new df_envimet_lib.Geometry.Plant3d(material, _plant3D);

            // OUTPUT
            DA.SetData(0, trees);
            trees.Dispose();
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
                return Properties.Resources.envimet3dPlantIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2e487338-cd57-4c3d-9b49-9cb1f16c78dc"); }
        }
    }
}