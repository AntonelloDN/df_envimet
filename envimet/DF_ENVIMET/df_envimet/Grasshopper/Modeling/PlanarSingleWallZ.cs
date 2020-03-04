using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Grasshopper.Modeling
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
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_horizontalPlanarSurface", "_horizontalPlanarSurface", "Horizoantal planar surface that represent ENVI-Met horazontal overhang 2d. Geometry must be horizonatal planar Surface or Brep.\nTry to generate planar shadings using Ladybug!", GH_ParamAccess.item);
            pManager.AddTextParameter("_singleWallId_", "_singleWallId_", "ENVI-Met single wall id. You can use \"id outputs\" which comes from \"DF Envimet Read Library\".\nDefault is 000001.", GH_ParamAccess.item);
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
            Mesh _horizontalPlanarSurface = null;
            string _singleWallId_ = df_envimet_lib.Geometry.Material.CommonSimpleWall;

            DA.GetData<Mesh>(0, ref _horizontalPlanarSurface);
            DA.GetData<string>(1, ref _singleWallId_);

            // actions
            df_envimet_lib.Geometry.Material material = new df_envimet_lib.Geometry.Material
            {
                CustomSimpleWallMaterial = _singleWallId_
            };

            df_envimet_lib.Geometry.SimpleWall shadings = new df_envimet_lib.Geometry.SimpleWall(_horizontalPlanarSurface, material);

            // OUTPUT
            DA.SetData(0, shadings);
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
                return Properties.Resources.envimetShadingHrzIcon; ;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e91afff6-c21d-44c3-84f5-62e5c9829b01"); }
        }
    }
}