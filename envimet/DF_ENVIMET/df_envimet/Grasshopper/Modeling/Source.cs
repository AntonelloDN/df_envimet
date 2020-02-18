using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Grasshopper.Modeling
{
    public class Source : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Source class.
        /// </summary>
        public Source()
          : base("DF Envimet Source", "DFEnvimetSource",
              "Use this component to generate sources for \"Dragonfly Envimet Spaces\". E.g. a fountain to apply evaporation strategy.",
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
            pManager.AddMeshParameter("_source", "_source", "Geometry that represent ENVI-Met source.  Geometry must be a Surface or Brep on xy plane.", GH_ParamAccess.item);
            pManager.AddTextParameter("_sourceId_", "_sourceId_", "ENVI-Met source id. You can use \"id outputs\" which comes from \"DF Envimet Read Library\".\nDefault is 0000FT.", GH_ParamAccess.item, df_envimet_lib.Geometry.Material.CommonSourceMaterial);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetSources", "envimetSources", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add source to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _source = null;
            string _sourceId_ = df_envimet_lib.Geometry.Material.CommonSourceMaterial;

            DA.GetData<Mesh>(0, ref _source);
            DA.GetData<string>(1, ref _sourceId_);

            // actions
            df_envimet_lib.Geometry.Material material = new df_envimet_lib.Geometry.Material
            {
                Custom2dMaterial = _sourceId_
            };

            df_envimet_lib.Geometry.Source sources = new df_envimet_lib.Geometry.Source(_source, material);

            // OUTPUT
            DA.SetData(0, sources);
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
                return Properties.Resources.envimetSourceIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0112621f-39e6-47b4-ac61-793f122bd3ad"); }
        }
    }
}