using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Modeling
{
    public class Terrain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Terrain class.
        /// </summary>
        public Terrain()
          : base("DF Envimet Terrain", "DFenvimetTerrain",
              "Use this component to generate inputs for \"Dragonfly Envimet Spaces\"",
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
            pManager.AddMeshParameter("_terrain", "_terrain", "You have two ways:\n1) A closed Brep/Mesh. Also many closed Breps/Meshes if merged together in a single Mesh.\n2) 2d Geometry that represent ENVI-Met terrain. Geometries have to be Surface/Open Brep or 2d Mesh in this case.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetTerrain", "envimetTerrain", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add terrain to ENVI-Met model.", GH_ParamAccess.item);
            pManager.AddGenericParameter("geometry", "geometry", "Terrain mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _terrain = null;

            DA.GetData(0, ref _terrain);

            // start
            MorphoEnvimetLibrary.Geometry.Material material = new MorphoEnvimetLibrary.Geometry.Material();

            MorphoEnvimetLibrary.Geometry.Terrain envimetTerrain = new MorphoEnvimetLibrary.Geometry.Terrain(_terrain, material);

            // OUTPUT
            DA.SetData(0, envimetTerrain);
            DA.SetData(1, envimetTerrain.GetMesh());
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
                return Properties.Resources.envimetTerrainIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f3483e1e-16b9-4ec6-8cad-4f01145ff452"); }
        }
    }
}