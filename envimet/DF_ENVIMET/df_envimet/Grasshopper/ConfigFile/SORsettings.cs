using System;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class SORsettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SORsettings class.
        /// </summary>
        public SORsettings()
          : base("DF Envimet SOR settings", "DFenvimetSORSettings",
              "EXPERT SETTINGS: Active SOR mode. If you active it pressure field is calculated via red-black-tree algorithm which allows parallel computation of pressure field.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("_active", "_active", "Run parallel calculation [bool].", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SOR", "SOR", "SOR settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool _active = true;

            DA.GetData(0, ref _active);

            Active active = (_active) ? Active.YES : Active.NO;

            SOR sor = new SOR(active);

            DA.SetData(0, sor);
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
                return Properties.Resources.sor;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3026a9fc-5a04-43c5-823f-b573b08e8cab"); }
        }
    }
}