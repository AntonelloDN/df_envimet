using System;
using System.Collections.Generic;
using df_envimet_lib.IO;
using Grasshopper.Kernel;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace df_envimet.Grasshopper.Other
{
    public class EditFacadeMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EditFacadeMaterial class.
        /// </summary>
        public EditFacadeMaterial()
          : base("DF Edit Facade Materials", "DFeditFacadeMaterials",
              "Use this component to edit facade material. You can use it to generate windows or adding green walls.\nYou need to use baked facades.\n" +
                "Istructions:\n1. Bake facades with 'DF Envimet Read Facade';\n2. Set 'Shaded' view Type\n3. Select Mesh facades in Rhino Canvas;\n4. Run component.\n\n" +
                "If you set greening input to True you will overwrite greening matrix.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nAPR_14_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.senary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_INXfileAddress", "_INXfileAddress", "Output of \"DF Envimet Spaces\".", GH_ParamAccess.item);
            pManager.AddTextParameter("material_", "material_", "Material to assign. Default is 000000.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("greening_", "greening_", "Set it to True if you want to replace greening matrix.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set it to True to save new INX.", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("INXfileAddress", "INXfileAddress", "The file path of the inx result file that has been generated on your machine.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            string inx = null;

            Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;
            List<Rhino.DocObjects.RhinoObject> selectedObjects = ot.GetSelectedObjects(false, false).ToList();

            //List<System.Guid> facades = new List<System.Guid>();
            string material = "000000";
            bool greening = false;
            bool runIt = false;

            DA.GetData(0, ref inx);
            DA.GetData(1, ref material);
            DA.GetData(2, ref greening);
            DA.GetData(3, ref runIt);

            if (runIt)
            {
                // Handle invalid tags within INX
                string file = File.ReadAllText(inx);
                file = file.Replace("3Dplants", "threeDplants");

                // Parse XML
                XDocument doc = XDocument.Parse(file);

                XElement wall = doc.Root.Element("WallDB").Element("ID_wallDB");
                XElement singleWall = doc.Root.Element("SingleWallDB").Element("ID_singlewallDB");
                XElement greenWall = doc.Root.Element("GreeningDB").Element("ID_GreeningDB");
                XElement terrain = doc.Root.Element("dem3D").Element("terrainflag");
                WorkaroundNewString(wall);
                WorkaroundNewString(singleWall);
                WorkaroundNewString(greenWall);
                WorkaroundNewString(terrain);

                List<PixelCoordinate> pixelSelection = Facade.GetPixelFromSelection(selectedObjects, material).ToList();

                List<string> matrixInx = Facade.GetFacadeSparseMatrix(inx).ToList();
                List<PixelCoordinate> pixelsInx = Facade.GetPixelFromSparseMatrix(matrixInx).ToList();

                string sparseMatrix = "\n" + String.Join("\n", matrixInx) + "\n";
                string greenSparseMatrix = null;

                if (greening)
                {
                    List<PixelCoordinate> pixels = Facade.GetUniqueSelectedPixels(pixelSelection).ToList();
                    greenSparseMatrix = Facade.GetFacadeSparseMatrixFromPixels(pixels);
                    greenWall.SetValue("\n" + greenSparseMatrix + "\n");
                }
                else
                {
                    IEnumerable<PixelCoordinate> pixels = Facade.UpdatePixelMaterial(pixelsInx, pixelSelection);
                    sparseMatrix = Facade.GetFacadeSparseMatrixFromPixels(pixels);
                    wall.SetValue("\n" + sparseMatrix + "\n");
                }

                string text = Facade.GetXmlWithWrongTag(doc.ToString());

                // write file in a new destination
                File.WriteAllText(inx, text);

                DA.SetData(0, inx);

            }

        }

        public void WorkaroundNewString(XElement elementXml)
        {
            if (elementXml.Value == "")
            {
                elementXml.SetValue("\n");
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
                return Properties.Resources.envimetEditBuildingMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cf4179a6-0a7f-4fa6-8934-798703b5f7be"); }
        }
    }
}