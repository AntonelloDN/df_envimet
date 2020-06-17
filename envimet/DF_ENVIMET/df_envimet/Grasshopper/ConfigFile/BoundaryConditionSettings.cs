using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;
using System.Collections.Generic;
using System.Linq;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class BoundaryConditionSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BoundaryConditionSettings class.
        /// </summary>
        public BoundaryConditionSettings()
          : base("DF Envimet Wind Resistence settings", "DFenvimetWindResistenceSettings",
              "Set lateral boundary condition." +
                "\n'Forced' is used when simpleforcing is activate." +
                "\n'Open' copy the values of the next grid point close to the border back to the border each timestep which mean overstimate the influence of environment near border. " +
                "\n'Cyclic' describes the process of copying values of the downstream boarder to the upstream boarder.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("_LBCtemperatureHumidity", "_LBCtemperatureHumidity", "Connect an integer to select LBC model." +
                "\n0 = Open" +
                "\n1 = Forced" +
                "\n2 = Cyclic", GH_ParamAccess.item);
            pManager.AddIntegerParameter("_LBCturbolence", "_LBCturbolence", "Connect an integer to select LBC model." +
                "\n0 = Open" +
                "\n1 = Forced" +
                "\n2 = Cyclic", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LBC", "LBC", "LBC settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int _LBCtemperatureHumidity = 0;
            int _LBCturbolence = 0;

            DA.GetData(0, ref _LBCtemperatureHumidity);
            DA.GetData(1, ref _LBCturbolence);

            List<BoundaryCondition> boundaryCondition = Enum.GetValues(typeof(BoundaryCondition))
                .Cast<BoundaryCondition>()
                .ToList();

            LBC lbc = new LBC(boundaryCondition[_LBCtemperatureHumidity], boundaryCondition[_LBCturbolence]);

            DA.SetData(0, lbc);
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
                return Properties.Resources.LBC;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("32f5b4f1-4283-410f-8ec9-088dd6d56ae2"); }
        }
    }
}