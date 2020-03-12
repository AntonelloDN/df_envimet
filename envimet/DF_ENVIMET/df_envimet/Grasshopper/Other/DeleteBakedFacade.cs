using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino;

namespace df_envimet.Grasshopper.Other
{
    public class DeleteBakedFacade : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeleteBackedFacade class.
        /// </summary>
        public DeleteBakedFacade()
          : base("DF Delete Baked Facade", "DFDeleteBakedFacade",
              "Use this component to delete baked facades on Rhino canvas.\nNote: use it only if need it. " +
                "Remember to disable it if you are editing inx file.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nJAN_23_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.senary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("_delete", "_delete", "Set runIt to \"True\" to delete facades.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool delete = false;

            DA.GetData(0, ref delete);

            Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;
            if (delete)
            {

                List<Rhino.DocObjects.RhinoObject> objects = new List<Rhino.DocObjects.RhinoObject>();
                Rhino.DocObjects.ObjectEnumeratorSettings s = new Rhino.DocObjects.ObjectEnumeratorSettings
                {
                    HiddenObjects = true,
                    LockedObjects = true
                };

                foreach (Rhino.DocObjects.RhinoObject obj in RhinoDoc.ActiveDoc.Objects.GetObjectList(s))
                {
                    if (obj.Name.StartsWith("Y:") || obj.Name.StartsWith("X:") || obj.Name.StartsWith("Z:"))
                        ot.Delete(obj.Id, true);
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
                return Properties.Resources.envimetBuildingDeleteFacade;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e9710f21-4587-43c6-b802-ca71324179f7"); }
        }
    }
}