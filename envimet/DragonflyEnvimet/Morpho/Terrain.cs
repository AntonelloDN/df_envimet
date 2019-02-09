using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using envimetGrid;

namespace DragonflyEnvimet
{
    public class Terrain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Terrain()
          : base("DF Envimet Terrain", "DFenvimetTerrain",
              "Use this component to generate inputs for \"Dragonfly Envimet Spaces\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_26_2018";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_terrain", "_terrain", "2d Geometry that represent ENVI-Met buildings.\nGeometries have to be Surface/Open Brep or 2d Mesh", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetTerrain", "envimetTerrain", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add terrain to ENVI-Met model.", GH_ParamAccess.item);

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

            envimetGrid.Dem envimetTerrain = new envimetGrid.Dem();

            envimetTerrain.TerrainMesh = envimetGrid.Dem.CreateClosedMeshTerrain(_terrain);

            // OUTPUT
            DA.SetData(0, envimetTerrain);
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
                return DragonflyEnvimet.Properties.Resources.envimetTerrainIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e06b0e29-0b34-4418-b11d-613c93123d4a"); }
        }
    }
}