using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using envimentManagment;

namespace DragonflyEnvimet
{
    public class EditBuildingMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EditBuildingMaterial()
          : base("DF Edit Building Materials", "DFeditBuildingMaterials",
              "Use this component to edit building materials. You can use it to generate windows.\nYou need to use both Curve and Shapes inputs for windows. Check where the points are using\"DFvisualizeBuildings\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_20_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_INXfileAddress", "_INXfileAddress", "Output of \"DF Envimet Spaces\".", GH_ParamAccess.item);
            pManager.AddGenericParameter("_envimentGrid", "_envimentGrid", "Connect the output of \"Dragonfly Envimet Grid\".", GH_ParamAccess.item);
            pManager.AddCurveParameter("_selectorCurve", "_selectorCurve", "A closed curve on Plane World XY. Use it to select parts you want to change.", GH_ParamAccess.item);
            pManager.AddMeshParameter("selectorShape_", "selectorShape_", "A list of solid to select parts to edit. If point is inside it will be modified.", GH_ParamAccess.list);
            pManager.AddTextParameter("materialX_", "materialX_", "Material in X direction. Default is 000000.", GH_ParamAccess.item, "000000");
            pManager.AddTextParameter("materialY_", "materialY_", "Material in Y direction. Default is 000000.", GH_ParamAccess.item, "000000");
            pManager.AddTextParameter("materialZ_", "materialZ_", "Material in Z direction. Default is 000000.", GH_ParamAccess.item, "000000");
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set it to True to save the modified.", GH_ParamAccess.item, false);

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
            string _INXfileAddress = null;
            ReadEnvimet readInx = new ReadEnvimet();
            envimetGrid.AutoGrid _envimentGrid = null;
            Curve crv = null;
            List<Mesh> meshes = new List<Mesh>();
            string matx = "000000";
            string maty = "000000";
            string matz = "000000";
            bool runIt = false;

            DA.GetData(0, ref _INXfileAddress);
            DA.GetData(1, ref _envimentGrid);
            DA.GetData(2, ref crv);
            DA.GetDataList(3, meshes);
            DA.GetData(4, ref matx);
            DA.GetData(5, ref maty);
            DA.GetData(6, ref matz);
            DA.GetData(7, ref runIt);


            // action
            string word = "ID_wallDB";
            List<string> rowData = new List<string>();
            


            if (_INXfileAddress != null)
            {
                try
                {
                    if (runIt)
                    {
                        // create a list of points
                        List<Point3d> points = readInx.ExtractPointFromINX(_INXfileAddress, _envimentGrid, word, ref rowData);
                        Mesh testShapes = readInx.UnionMeshUtility(meshes);


                        // every row is a series of numbers and text separated by comma
                        string[] newValueText = new string[points.Count];
                        for (int i = 0; i < points.Count; i++)
                        {
                            if (envimentManagment.ReadEnvimet.PointContainmentCheck(points[i], crv))
                            {

                                if (meshes.Count > 0)
                                {

                                    if (readInx.PointsInShapesCheck(testShapes, points[i]))
                                    {
                                        newValueText[i] = readInx.UpdateRowTextMaterial(rowData[i], matx, maty, matz);
                                    }
                                    else
                                    {
                                        newValueText[i] = rowData[i];
                                    }

                                }
                                else
                                {
                                    newValueText[i] = readInx.UpdateRowTextMaterial(rowData[i], matx, maty, matz);
                                }


                            }
                            else
                            {
                                newValueText[i] = rowData[i];
                            }


                        }


                        // Handle invalid tags within INX
                        string myFile = File.ReadAllText(_INXfileAddress);
                        myFile = myFile.Replace("3Dplants", "threeDplants");


                        // Parse XML
                        XDocument doc = XDocument.Parse(myFile);

                        string newValue = "\n" + String.Join("\n", newValueText) + "\n";

                        XElement wall = doc.Root.Element("WallDB").Element("ID_wallDB");
                        XElement singleWall = doc.Root.Element("SingleWallDB").Element("ID_singlewallDB");
                        XElement greenWall = doc.Root.Element("GreeningDB").Element("ID_GreeningDB");
                        XElement terrain = doc.Root.Element("dem3D").Element("terrainflag");

                        WorkaroundNewString(wall);
                        WorkaroundNewString(singleWall);
                        WorkaroundNewString(greenWall);
                        WorkaroundNewString(terrain);

                        wall.SetValue(newValue);


                        string finalText = readInx.RestoreInvalidTag(doc.ToString());

                        // write file in a new destination
                        System.IO.File.WriteAllText(_INXfileAddress, finalText);

                        DA.SetData(0, _INXfileAddress);
                    }

                }
                catch
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something went wrong... only output from DF Envimet Spaces are supported now.");
                }
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
                return DragonflyEnvimet.Properties.Resources.envimetEditBuildingMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a883fdbc-4fa6-44bf-b957-dfb5ff7505d3"); }
        }
    }
}