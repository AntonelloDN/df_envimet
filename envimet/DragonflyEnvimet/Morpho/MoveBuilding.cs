using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using envimetGeometry;

namespace DragonflyEnvimet
{
    public class MoveBuilding : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MoveBuilding()
          : base("DF Move Building Up", "DFMoveBuildingUp",
              "Use this component to align buildings to the terrain surface",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_26_2018";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("_buildings", "_buildings", "Geometry that represent ENVI-Met buildings.\nGeometries have to be closed Brep or Mesh", GH_ParamAccess.list);
            pManager.AddMeshParameter("_terrain", "_terrain", "2d Geometry that represent ENVI-Met buildings.\nGeometries have to be Surface/Open Brep or 2d Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("buildigs", "buildigs", "Result. Connect this output to \"DF Buildings\"", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            List<Mesh> _buildings = new List<Mesh>();
            Mesh _terrain = null;

            DA.GetDataList<Mesh>(0, _buildings);
            DA.GetData(1, ref _terrain);


            var xBuildings = _buildings.Select(m => envimetGeometry.BuildingMatrix.MoveBuildingsUp(m, _terrain)).ToList();

            // OUTPUT
            DA.SetDataList(0, xBuildings);

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
                return DragonflyEnvimet.Properties.Resources.envimetMoveUpIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d1749318-9a93-489e-a7d5-7e8bd0df6ca9"); }
        }
    }
}