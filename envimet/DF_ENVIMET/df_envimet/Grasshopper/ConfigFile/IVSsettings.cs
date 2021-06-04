using System;
using df_envimet_lib.Settings;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class IVSsettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVSsettings class.
        /// </summary>
        public IVSsettings()
          : base("DF Envimet IVS Settings", "DFenvimetIVSsettings",
              "Advance radiation transfer schema to use. IVS allows a detailed analysis and calculation of shortwave and longwave radiation fluxes with taking into account multiple interactions between surfaces.",
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
            pManager.AddBooleanParameter("_IVSon", "_IVSon", "Use Index View Sphere (IVS) for radiation transfer [bool].", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_IVSmemory", "_IVS_memory", " Do you want to store the values in the memory? [bool]", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IVS", "IVS", "IVS settings of SIMX file. Connect it to DF Enviment Config.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool _IVSon = true;
            bool _IVSmemory = true;

            DA.GetData(0, ref _IVSon);
            DA.GetData(1, ref _IVSmemory);

            Active ivsOn = (_IVSon) ? Active.YES : Active.NO;
            Active ivsMem = (_IVSmemory) ? Active.YES : Active.NO;

            IVS ivs = new IVS(ivsOn, ivsMem);

            DA.SetData(0, ivs);
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
                return Properties.Resources.ivs;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b7888baf-349f-4bcc-8a65-4fadce9906b4"); }
        }
    }
}