using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using envimetGrid;

namespace DragonflyEnvimet
{
    public class PlanarSingleWallZ : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PlanarSingleWallZ()
          : base("DF Horizontal Overhang", "DFhorizontalOverhang",
              "Use this component to horizontal overangs \"Dragonfly Envimet Spaces\".",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_24_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("_horizontalPlanarSurface", "_horizontalPlanarSurface", "Horizoantal planar surface that represent ENVI-Met horazontal overhang 2d. Geometry must be horizonatal planar Surface or Brep.\nTry to generate planar shadings using Ladybug!", GH_ParamAccess.list);
            pManager.AddTextParameter("_singleWallId_", "_singleWallId_", "ENVI-Met single wall id. You can use \"id outputs\" which comes from \"LB ENVI - Met Read Library\".\nDefault is 0000XX.", GH_ParamAccess.list, "000001");
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetShadings", "envimetShadings", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add horizontal overhang to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            List<Brep> geo = new List<Brep>();
            List<string> ids = new List<string>();

            DA.GetDataList<Brep>(0, geo);
            DA.GetDataList<string>(1, ids);

            // actions
            try
            {
                envimetGrid.SimpleWall hrShadings = new envimetGrid.SimpleWall("000001", ids, geo);

                // OUTPUT
                DA.SetData(0, hrShadings);
            }
            catch
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something went wrong... please check if there are null inputs.");
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
                return DragonflyEnvimet.Properties.Resources.envimetShadingHrzIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9056a9bd-4d6e-42c2-8c5b-a3fbc48ddf1d"); }
        }
    }
}