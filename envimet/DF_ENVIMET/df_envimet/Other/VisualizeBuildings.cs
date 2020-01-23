using System;
using System.Collections.Generic;
using editEnvimetObject;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace df_envimet.Other
{
    public class VisualizeBuildings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VisualizeBuildings()
          : base("DF Visualize Buildings", "DFvisualizeBuildings",
              "Use this component to see buildings centroids of ENVI_MET model file.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_INXfileAddress", "_INXfileAddress", "Output of \"DF Envimet Spaces\".", GH_ParamAccess.item);
            pManager.AddGenericParameter("_envimentGrid_", "_envimentGrid_", "Connect the output of \"Dragonfly Envimet Grid\".", GH_ParamAccess.item);
            pManager.AddBooleanParameter("innerOuter_", "innerOuter_", "True = AllPoints; False = Outer Points", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("points", "points", "Points that represent buildings cells.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string _INXfileAddress = null;
            PostProcessing readInx = new PostProcessing();
            MorphoEnvimetLibrary.Geometry.Grid _envimentGrid_ = null;
            bool innerOuter_ = true;

            DA.GetData(0, ref _INXfileAddress);
            DA.GetData(1, ref _envimentGrid_);
            DA.GetData(2, ref innerOuter_);

            // action
            string word = (innerOuter_ == true) ? "buildingFlagAndNr" : "ID_wallDB";
            List<string> rowData = new List<string>();

            if (_INXfileAddress != null)
            {
                try
                {
                    DA.SetDataList(0, readInx.ExtractPointFromINX(_INXfileAddress, _envimentGrid_, word, ref rowData));
                }
                catch
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something went wrong... only output from DF Envimet Spaces are supported now.");
                }
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
                return Properties.Resources.envimetBuildingsVisualizerIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7d3470bd-b298-47d9-bf32-20047914990b"); }
        }
    }
}