using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DragonflyEnvimet
{
    public class LBCsettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public LBCsettings()
          : base("DF Envimet LBC settings", "DFenvimetLBCsettings",
              "EXPERT SETTINGS: This component let you change boundary condition of the model.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nGEN_23_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("_lbcTq_", "_lbcTq_", "Lateral Boundary Condition for Temperature and humidity. Default value is 1 (Open).\nConnect a value from 1 to 3:\n1 = Open\n2 = Forced\n3 = Cyclic.", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("_lbcTKE_", "_lbcTKE_", "Lateral Boundary Condition for Turbulence. Default value is 1 (Open).\nConnect a value from 1 to 3:\n1 = Open\n2 = Forced\n3 = Cyclic.", GH_ParamAccess.item, 1);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("lbcTypes", "lbcTypes", "Timestep settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            int lbcTq = 1;
            int lbcTKE = 1;

            DA.GetData(0, ref lbcTq);
            DA.GetData(1, ref lbcTKE);


            // actions
            envimetSimulationFile.BoundaryCondition lbcSettings = new envimetSimulationFile.BoundaryCondition()
            {
                LBC_TQ = lbcTq,
                LBC_TKE = lbcTKE
            };

            // OUTPUT
            DA.SetData(0, lbcSettings);

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
                return DragonflyEnvimet.Properties.Resources.envimetLBCtypeIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("de462132-27f5-49b2-a2ef-4a55ca86ae97"); }
        }
    }
}