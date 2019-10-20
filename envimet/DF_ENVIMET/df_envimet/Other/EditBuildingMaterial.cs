using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using editEnvimetObject;
using Grasshopper.Kernel;
using MorphoEnvimetLibrary.Geometry;
using Rhino.Geometry;

namespace df_envimet.Other
{
    public class EditBuildingMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EditBuildingMaterial class.
        /// </summary>
        public EditBuildingMaterial()
          : base("DF Edit Building Materials", "DFeditBuildingMaterials",
              "Use this component to edit building materials. You can use it to generate windows.\nYou need to use both Curve and Shapes inputs for windows. Check where the points are using\"DFvisualizeBuildings\"",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nNOV_19_2019";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_INXfileAddress", "_INXfileAddress", "Output of \"DF Envimet Spaces\".", GH_ParamAccess.item);
            pManager.AddGenericParameter("_envimentGrid", "_envimentGrid", "Connect the output of \"Dragonfly Envimet Grid\".", GH_ParamAccess.item);
            pManager.AddCurveParameter("_selectorCurve", "_selectorCurve", "A closed curve on Plane World XY. Use it to select parts you want to change.", GH_ParamAccess.item);
            pManager.AddMeshParameter("selectorShape_", "selectorShape_", "A list of solid to select parts to edit. If point is inside it will be modified.", GH_ParamAccess.list);
            pManager.AddTextParameter("materialX_", "materialX_", "Material in X direction. Default is 000000.", GH_ParamAccess.item, Material.CommonWallMaterial);
            pManager.AddTextParameter("materialY_", "materialY_", "Material in Y direction. Default is 000000.", GH_ParamAccess.item, Material.CommonWallMaterial);
            pManager.AddTextParameter("materialZ_", "materialZ_", "Material in Z direction. Default is 000000.", GH_ParamAccess.item, Material.CommonRoofMaterial);
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
            PostProcessing editEnvimetBuilding = new PostProcessing();

            MorphoEnvimetLibrary.Geometry.Grid _envimentGrid = null;
            Curve crv = null;
            List<Mesh> meshes = new List<Mesh>();
            string matx = Material.CommonWallMaterial;
            string maty = Material.CommonWallMaterial;
            string matz = Material.CommonRoofMaterial;
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
                        List<Point3d> points = editEnvimetBuilding.ExtractPointFromINX(_INXfileAddress, _envimentGrid, word, ref rowData);
                        Mesh testShapes = editEnvimetBuilding.UnionMeshUtility(meshes);


                        // every row is a series of numbers and text separated by comma
                        string[] newValueText = new string[points.Count];
                        for (int i = 0; i < points.Count; i++)
                        {
                            if (PostProcessing.PointContainmentCheck(points[i], crv))
                            {

                                CellMaterial newMaterial;
                                newMaterial.MaterialX = matx;
                                newMaterial.MaterialY = maty;
                                newMaterial.MaterialZ = matz;

                                if (meshes.Count > 0)
                                {

                                    if (editEnvimetBuilding.PointsInShapesCheck(testShapes, points[i]))
                                    {
                                        newValueText[i] = editEnvimetBuilding.UpdateRowTextMaterial(rowData[i], newMaterial);
                                    }
                                    else
                                    {
                                        newValueText[i] = rowData[i];
                                    }

                                }
                                else
                                {
                                    newValueText[i] = editEnvimetBuilding.UpdateRowTextMaterial(rowData[i], newMaterial);
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

                        string finalText = editEnvimetBuilding.RestoreInvalidTag(doc.ToString());

                        // write file in a new destination
                        File.WriteAllText(_INXfileAddress, finalText);

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
                return Properties.Resources.envimetEditBuildingMaterial;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9bf90213-165d-4ff8-8513-89722420961f"); }
        }
    }
}