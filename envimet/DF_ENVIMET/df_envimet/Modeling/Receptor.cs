using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Modeling
{
    public class Receptor : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Receptor class.
        /// </summary>
        public Receptor()
          : base("DF Envimet Receptor", "DFEnvimetReceptor",
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
            pManager.AddMeshParameter("_receptors", "_receptors", "Geometry that represent ENVI-Met receptor. ID is calculated automatically with a concatenation of x and y.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetReceptors", "envimetReceptors", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add 3d plants to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            Mesh _receptors = null;

            DA.GetData<Mesh>(0, ref _receptors);

            // actions

            MorphoEnvimetLibrary.Geometry.Receptor receptor = new MorphoEnvimetLibrary.Geometry.Receptor(_receptors);

            // OUTPUT
            DA.SetData(0, receptor);
            receptor.Dispose();
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
                return Properties.Resources.envimetReceptorsIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5e63f5a2-6887-46eb-acc7-eebb99c4f8fe"); }
        }
    }
}