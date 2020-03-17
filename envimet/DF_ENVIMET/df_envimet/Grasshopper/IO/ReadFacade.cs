using System;
using System.Collections.Generic;
using df_envimet.Grasshopper.UI_GH;
using df_envimet_lib.IO;
using Grasshopper.Kernel;
using System.Linq;
using ReadEnvimet = df_envimet_lib.IO.Read;
using GridEnvimet = df_envimet_lib.Geometry.Grid;

namespace df_envimet.Grasshopper.IO
{
    public class ReadFacade : ComponentWithDirectionLogic
    {
        /// <summary>
        /// Initializes a new instance of the ReadFacade class.
        /// </summary>
        public ReadFacade()
          : base("DF Envimet Read Facade", "DFReadFacade",
              "Use this component to read the facades from Inx file.\n Select a direction using buttons: X, Y, Z.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_07_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_INXfileAddress", "_INXfileAddress", "Inx file here.", GH_ParamAccess.item);
            pManager.AddGenericParameter("envimentGrid_", "envimentGrid_", "Connect it only if you have used Dragonfly to create Inx model.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("runIt_", "runIt_", "Set runIt to True to read output.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("bake_", "bake_", "Set runIt and bake to True to add facade pixel in Rhino canvas.\n" +
                "If you want to change material pixel by pixel select facade elements and use them as input of 'DF Edit Building Material'.", GH_ParamAccess.item, false);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("analysisMesh", "analysisMesh", "An uncolored mesh representing the test facade that will be analyzed.  Connect this output to 'Ladybug Legagy' or 'Ladybug[+]' recolor mesh component.",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string inx = null;
            bool runIt = false;
            bool bake = false;
            GridEnvimet envimentGrid = null;

            DA.GetData(0, ref inx);
            DA.GetData(1, ref envimentGrid);
            DA.GetData(2, ref runIt);
            DA.GetData(3, ref bake);

            if (runIt)
            {
                var modelgeometry = Facade.GetGridDetail(inx);
                if (modelgeometry == null)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "It works only with 3d detailed model for now.\n" +
                        "Please, use ENVI-Met Spaces to convert your 2.5D model into 3D model.");
                    return;
                }

                var matrix = Facade.GetFacadeSparseMatrix(inx).ToList();

                Facade face = new Facade();

                if (envimentGrid == null)
                    envimentGrid = ReadEnvimet.GetGridFromInx(modelgeometry);

                var facades = Facade.GenerateFacadeByDirection(matrix, envimentGrid, Direction, bake);

                DA.SetData(0, face.GetAnalysisMesh(facades.ToList()));
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
            get { return new Guid("b06f7476-3a38-4d3c-8985-76436fdced90"); }
        }
    }
}