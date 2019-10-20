using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MorphoEnvimetLibrary.Settings;

namespace df_envimet.Settings
{
    public class Location : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Location class.
        /// </summary>
        public Location()
          : base("DF Envimet Location", "DFEnvimetLocation",
              "Use this component to generate inputs for \"df_envimet Spaces\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_19_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_location", "_location", "The output from the importEPW or constructLocation component.  This is essentially a list of text summarizing a location on the earth.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("modelRotation_", "modelRotation_", "Input a number between 0 and 360 that represents the degrees off from the y-axis to make North.  The default North direction is set to the Y-axis (0 degrees).", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("envimetLocation", "envimetLocation", "Connect this output to \"Dragonfly Envimet Spaces\" in order to add location data to ENVI-Met model.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string _location = null;
            int modelRotation_ = 0;

            DA.GetData(0, ref _location);
            DA.GetData(1, ref modelRotation_);

            // actions
            MorphoEnvimetLibrary.Settings.Location myLoc = new MorphoEnvimetLibrary.Settings.Location(_location, modelRotation_);

            // OUTPUT
            DA.SetData(0, myLoc);
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
                return Properties.Resources.envimetLocationIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fe6362b4-0975-4275-936d-fcb949f3b7a7"); }
        }
    }
}