using System;
using Grasshopper.Kernel;
using df_envimet.Grasshopper.UI_GH;

namespace df_envimet
{
    public class TestComponentAntonello : ExtensionReceptorComponent
    {

        /// <summary>
        /// Initializes a new instance of the TestComponentAntonello class.
        /// </summary>
        public TestComponentAntonello()
           : base("DF Envimet aaaa", "aaaaa",
              "Use this component to generate buildings for \"Dragonfly Envimet Spaces\". Make sure buildings are " +
                "clean geometries otherwise you can use boolean function to merge parts together.\nAnother possibility is to use tools like MorphoMesh or similar to work with meshes performing complex boolean union operations.",
              "Dragonfly", "5 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("genere", "genere", "...", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, _value);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("38a9c457-1ddd-4b76-9367-abee6f1565a9"); }
        }
    }
}