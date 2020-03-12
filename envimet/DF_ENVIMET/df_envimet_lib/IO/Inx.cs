using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using df_envimet_lib.Geometry;
using df_envimet_lib.Settings;

namespace df_envimet_lib.IO
{
    public class Inx
    {
        static public void CreateXmlSection(XmlTextWriter w, string sectionTitle, string[] tags, string[] values, int flag, string[] attributes)
        {

            w.WriteStartElement(sectionTitle);
            w.WriteString("\n ");
            foreach (var item in tags.Zip(values, (a, b) => new { A = a, B = b }))
            {
                if (flag == 0)
                {
                    w.WriteStartElement(item.A);
                    w.WriteString(item.B);
                    w.WriteEndElement();
                }
                else if (flag == 1)
                {
                    w.WriteStartElement(item.A);
                    w.WriteAttributeString("", "type", null, attributes[0]);
                    w.WriteAttributeString("", "dataI", null, attributes[1]);
                    w.WriteAttributeString("", "dataJ", null, attributes[2]);
                    w.WriteString(item.B);
                    w.WriteEndElement();
                }
                else
                {
                    w.WriteStartElement(item.A);
                    w.WriteAttributeString("", "type", null, attributes[0]);
                    w.WriteAttributeString("", "dataI", null, attributes[1]);
                    w.WriteAttributeString("", "dataJ", null, attributes[2]);
                    w.WriteAttributeString("", "zlayers", null, attributes[3]);
                    w.WriteAttributeString("", "defaultValue", null, attributes[4]);
                    w.WriteString(item.B);
                    w.WriteEndElement();
                }
                w.WriteString("\n ");

            }
            w.WriteEndElement();
            w.WriteString("\n");
        }

        public static void INXwriteMethod(string path, Preparation preparation)
        {

            // preparation
            var now = DateTime.Now;
            string revisionDate = now.ToString("yyyy-MM-dd HH:mm:ss");
            const string NewLine = "\n";

            var grid = (Grid)preparation.EnvimetPart["grid"];
            var building = (Tuple<Dictionary<string, string>, List<string[]>>)preparation.EnvimetPart["building"];
            var dem = (Tuple<Point3d[], string, string>) preparation.EnvimetPart["dem"];
            var plant3d = (List<string[]>) preparation.EnvimetPart["plant3d"];
            var receptors = (List<string[]>) preparation.EnvimetPart["receptors"];

            var nestingGrid = (NestingGrid)preparation.EnvimetPart["nestingGrid"];
            var simpleplants2D = (string) preparation.EnvimetPart["simpleplants2D"];
            var soils2D = (string) preparation.EnvimetPart["soils2D"];
            var sources2D = (string) preparation.EnvimetPart["sources2D"];
            var simpleW = (string) preparation.EnvimetPart["simpleW"];
            var location = (Location)preparation.EnvimetPart["location"];

            var envimetZeroMatrix = (string) preparation.EnvimetPart["envimetZeroMatrix"];
            var envimetEmptyMatrix = (string) preparation.EnvimetPart["envimetEmptyMatrix"];
            var emptySoilMatrix = (string) preparation.EnvimetPart["emptySoilMatrix"];

            // unwrap buildings
            string buildingNumberMatrix = building.Item1["buildingNumberMatrix"];
            string wallDBMatrix = building.Item1["wallDBMatrix"];
            string greenDBMatrix = building.Item1["greenDBMatrix"];
            string topMatrix = building.Item1["topMatrix"];
            string bottomMatrix = building.Item1["bottomMatrix"];
            string fixedMatrix = building.Item1["fixedMatrix"];
            string idMatrix = building.Item1["idMatrix"];
            var greenInfo = building.Item2;

            // unwrap dem
            string dem2d = dem.Item3;
            string terrainflag = dem.Item2;


            // start with xml
            XmlTextWriter xWriter = new XmlTextWriter(path, Encoding.UTF8);
            xWriter.WriteStartElement("ENVI-MET_Datafile");
            xWriter.WriteString(NewLine + " ");

            string[] empty = { };

            // section Header
            string headerTitle = "Header";
            string[] headerTag = new string[] { "filetype", "version", "revisiondate", "remark", "encryptionlevel" };
            string[] headerValue = new string[] { "INPX ENVI-met Area Input File", "401", revisionDate, "Created with lb_envimet", "0" };

            Inx.CreateXmlSection(xWriter, headerTitle, headerTag, headerValue, 0, empty);

            // section baseData
            string baseDataTitle = "baseData";
            string[] baseDataTag = new string[] { "modelDescription", "modelAuthor" };
            string[] baseDataValue = new string[] { " A brave new area ", " DragonFly envimet " };

            Inx.CreateXmlSection(xWriter, baseDataTitle, baseDataTag, baseDataValue, 0, empty);

            // section modelGeometry
            // preparation telescope / splitted grid

            string useSplitting = null;
            string verticalStretch = null;
            string startStretch = null;
            string grids3DK = null;
            string gridsZ = null;
            string useTelescoping = null;
            string gridsI = (grid.NumX).ToString();
            string gridsJ = (grid.NumY).ToString();
            string[] attribute2dElements = { "matrix-data", gridsI, gridsJ };
            string dx = grid.DimX.ToString("n5");
            string dy = grid.DimY.ToString("n5");
            string dz = grid.DimZ.ToString("n5");

            if (grid.Telescope > 0)
            {
                useTelescoping = "1";
                useSplitting = "0";
                verticalStretch = grid.Telescope.ToString();
                startStretch = grid.StartTelescopeHeight.ToString();
                grids3DK = gridsZ = grid.NumZ.ToString();
                if (grid.CombineGridType)
                {
                    useSplitting = "1";
                    gridsZ = grid.NumZ.ToString();
                    grids3DK = (grid.NumZ + Grid.FIRST_CELL_COMBINED_GRID).ToString();
                }
            }
            else
            {
                useTelescoping = "0";
                useSplitting = "1";
                verticalStretch = "0";
                startStretch = "0";
                gridsZ = grid.NumZ.ToString();
                grids3DK = grids3DK = (grid.NumZ + Grid.FIRST_CELL_COMBINED_GRID).ToString();
            }

            string modelGeometryTitle = "modelGeometry";
            string[] modelGeometryTag = new string[] { "grids-I", "grids-J", "grids-Z", "dx", "dy", "dz-base", "useTelescoping_grid", "useSplitting", "verticalStretch", "startStretch", "has3DModel", "isFull3DDesign" };
            string[] modelGeometryValue = new string[] { gridsI, gridsJ, gridsZ, dx, dy, dz, useTelescoping, useSplitting, verticalStretch, startStretch, "1", "1" };

            Inx.CreateXmlSection(xWriter, modelGeometryTitle, modelGeometryTag, modelGeometryValue, 0, empty);


            // section nestingArea
            string nestingAreaTitle = "nestingArea";
            string[] nestingAreaTag = new string[] { "numberNestinggrids", "soilProfileA", "soilProfileB" };
            string[] nestingAreaValue = new string[] { nestingGrid.NumNestingGrid.ToString(), nestingGrid.SoilProfileA, nestingGrid.SoilProfileB };

            Inx.CreateXmlSection(xWriter, nestingAreaTitle, nestingAreaTag, nestingAreaValue, 0, empty);


            // section locationData
            string locationDataTitle = "locationData";
            string[] locationDataTag = new string[] { "modelRotation", "projectionSystem", "realworldLowerLeft_X", "realworldLowerLeft_Y", "locationName", "location_Longitude", "location_Latitude", "locationTimeZone_Name", "locationTimeZone_Longitude" };
            string[] locationDataValue = new string[] { location.ModelRotation.ToString("n5"), " ", " 0.00000 ", " 0.00000 ", location.LocationName, location.Longitude.ToString(), location.Latitude.ToString(), location.TimeZone, " 15.00000 " };

            Inx.CreateXmlSection(xWriter, locationDataTitle, locationDataTag, locationDataValue, 0, empty);


            // section defaultSettings
            string defaultSettingsTitle = "defaultSettings";
            string[] defaultSettingsTag = new string[] { "commonWallMaterial", "commonRoofMaterial" };
            string[] defaultSettingsValue = new string[] { Material.CommonWallMaterial, Material.CommonRoofMaterial };

            Inx.CreateXmlSection(xWriter, defaultSettingsTitle, defaultSettingsTag, defaultSettingsValue, 0, empty);


            // section buildings2D
            
            string buildings2DTitle = "buildings2D";
            string[] buildings2DTag = new string[] { "zTop", "zBottom", "buildingNr", "fixedheight" };
            string[] buildings2DValue = new string[] { NewLine + topMatrix, NewLine + bottomMatrix, NewLine + idMatrix, NewLine + fixedMatrix };

            Inx.CreateXmlSection(xWriter, buildings2DTitle, buildings2DTag, buildings2DValue, 1, attribute2dElements);
            
            // section simpleplants2D
            string simpleplants2DTitle = "simpleplants2D";
            string[] simpleplants2DTag = new string[] { "ID_plants1D" };
            string[] simpleplants2DValue = new string[] { NewLine + simpleplants2D };

            Inx.CreateXmlSection(xWriter, simpleplants2DTitle, simpleplants2DTag, simpleplants2DValue, 1, attribute2dElements);


            // section plant3d
            if (plant3d.Count > 0)
            {

                for (int i = 0; i < plant3d.Count; i++)
                {
                    string plants3DTitle = "3Dplants";
                    string[] plants3DTag = new string[] { "rootcell_i", "rootcell_j", "rootcell_k", "plantID", "name", "observe" };
                    string[] plants3DValue = new string[] { plant3d[i][0], plant3d[i][1], plant3d[i][2], plant3d[i][3], plant3d[i][4], plant3d[i][5] };

                    Inx.CreateXmlSection(xWriter, plants3DTitle, plants3DTag, plants3DValue, 0, empty);
                }
                
            }

            // section receptors
            if (receptors.Count > 0)
            {

                for (int i = 0; i < receptors.Count; i++)
                {
                    string receptorsTitle = "Receptors";
                    string[] receptorsTag = new string[] { "cell_i", "cell_j", "name" };
                    string[] receptorsValue = new string[] { receptors[i][0], receptors[i][1], receptors[i][2] };

                    Inx.CreateXmlSection(xWriter, receptorsTitle, receptorsTag, receptorsValue, 0, empty);
                }

            }

            // section soils2D
            string soils2DTitle = "soils2D";
            string[] soils2DTag = new string[] { "ID_soilprofile" };
            string[] soils2DValue = new string[] { NewLine + soils2D };

            Inx.CreateXmlSection(xWriter, soils2DTitle, soils2DTag, soils2DValue, 1, attribute2dElements);


            // section dem
            string demTitle = "dem";
            string[] demDTag = new string[] { "terrainheight" };

            string[] demValue = new string[] { NewLine + dem2d };
            Inx.CreateXmlSection(xWriter, demTitle, demDTag, demValue, 1, attribute2dElements);

            
            // section sources2D
            string sources2DTitle = "sources2D";
            string[] sources2DTag = new string[] { "ID_sources" };
            string[] sources2DValue = new string[] { NewLine + sources2D };

            Inx.CreateXmlSection(xWriter, sources2DTitle, sources2DTag, sources2DValue, 1, attribute2dElements);


            // section receptors2D (does not exist anymore)
            //string receptors2DTitle = "receptors2D";
            //string[] receptors2DTag = new string[] { "ID_receptors" };
            //string[] receptors2DValue = new string[] { NewLine + envimetEmptyMatrix };

            //Inx.CreateXmlSection(xWriter, receptors2DTitle, receptors2DTag, receptors2DValue, 1, attribute2dElements);


            // section additionalData (maybe next release)
            string additionalDataTitle = "additionalData";
            string[] additionalDataTag = new string[] { "db_link_point", "db_link_area" };
            string[] additionalDataValue = new string[] { NewLine + envimetEmptyMatrix, NewLine + envimetEmptyMatrix };

            Inx.CreateXmlSection(xWriter, additionalDataTitle, additionalDataTag, additionalDataValue, 1, attribute2dElements);
            
            // section greenins part 1
            if (greenInfo.Count > 0)
            {
                foreach (string[] green in greenInfo)
                {
                    string buildinginfoTitle = "Buildinginfo";
                    string[] buildinginfoTag = new string[] { "BuildingInternalNr", "BuildingName", "BuildingWallMaterial", "BuildingRoofMaterial", "BuildingFacadeGreening", "BuildingRoofGreening" };
                    string[] buildinginfoValue = green;

                    Inx.CreateXmlSection(xWriter, buildinginfoTitle, buildinginfoTag, buildinginfoValue, 0, empty);
                }
            }

            // section modelGeometry3D
            // preparation
            string grids3D_I = gridsI;
            string grids3D_J = gridsJ;
            string grids3D_K = grids3DK;
            string[] attribute3dElementsBuilding = { "sparematrix-3D", gridsI, gridsJ, grids3DK, "0" };
            string[] attribute3dElementsTerrain = { "sparematrix-3D", gridsI, gridsJ, grids3DK, "0.00000" };
            string[] attribute3dElementsWallDB = { "sparematrix-3D", gridsI, gridsJ, grids3DK, "" };

            string modelGeometry3DTitle = "modelGeometry3D";
            string[] modelGeometry3DTag = new string[] { "grids3D-I", "grids3D-J", "grids3D-K" };
            string[] modelGeometry3DValue = new string[] { gridsI, gridsJ, grids3DK };

            Inx.CreateXmlSection(xWriter, modelGeometry3DTitle, modelGeometry3DTag, modelGeometry3DValue, 0, empty);


            // section buildings3D
            string buildings3DTitle = "buildings3D";
            string[] buildings3DTag = new string[] { "buildingFlagAndNr" };
            string[] buildings3DValue = new string[] { NewLine + buildingNumberMatrix };

            Inx.CreateXmlSection(xWriter, buildings3DTitle, buildings3DTag, buildings3DValue, 2, attribute3dElementsBuilding);


            // section dem3D
            string dem3DTitle = "dem3D";
            string[] dem3DTag = new string[] { "terrainflag" };
            string[] dem3DValue = new string[] { NewLine + terrainflag };

            Inx.CreateXmlSection(xWriter, dem3DTitle, dem3DTag, dem3DValue, 2, attribute3dElementsTerrain);


            // section WallDB
            string WallDBTitle = "WallDB";
            string[] WallDBTag = new string[] { "ID_wallDB" };
            string[] WallDBValue = new string[] { NewLine + wallDBMatrix };

            Inx.CreateXmlSection(xWriter, WallDBTitle, WallDBTag, WallDBValue, 2, attribute3dElementsWallDB);


            // section SingleWallDB
            string SingleWallDBTitle = "SingleWallDB";
            string[] SingleWallDBTag = new string[] { "ID_singlewallDB" };
            string[] SingleWallDBValue = new string[] { NewLine + simpleW };

            Inx.CreateXmlSection(xWriter, SingleWallDBTitle, SingleWallDBTag, SingleWallDBValue, 2, attribute3dElementsWallDB);

            
            // section GreeningDB
            string GreeningDBTitle = "GreeningDB";
            string[] GreeningDBTag = new string[] { "ID_GreeningDB" };
            string[] GreeningDBValue = null;
            if (greenInfo.Count > 0)
            {
                GreeningDBValue = new string[] { NewLine + greenDBMatrix };
            }
            else
            {
                GreeningDBValue = new string[] { " " };
            }

            Inx.CreateXmlSection(xWriter, GreeningDBTitle, GreeningDBTag, GreeningDBValue, 2, attribute3dElementsWallDB);
            
            xWriter.WriteEndElement();
            xWriter.Close();
        }
    }
}
