using Rhino;
using System;
using System.Xml;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace envimentFileManagement
{
    public class WorkspaceFolderLB
    {
        private string workspaceFolder;
        private string projectFolderName;
        private string fileNamePrj;
        private string iniFileName;
        private string worspaceName;
        private string projectName;
        private string edbFileName;


        public WorkspaceFolderLB(string workspaceFolder, string projectFolderName)
        {
            this.workspaceFolder = workspaceFolder;
            this.projectFolderName = projectFolderName;
            this.fileNamePrj = System.Environment.MachineName + ".projects";
            this.iniFileName = "envimet.ini";
            this.worspaceName = "workspace.infoX";
            this.projectName = "project.infoX";
            this.edbFileName = "projectdatabase.edb";
        }


        public string WorkspaceFolderLBwrite(string mainDirectory)
        {
            // date
            var now = DateTime.Now;

            // complete path
            string fullFolder = System.IO.Path.Combine(this.workspaceFolder, this.projectFolderName);

            // check folder
            if (!(System.IO.Directory.Exists(fullFolder)))
            {
                System.IO.Directory.CreateDirectory(fullFolder);
            }

            // create project
            string prjFile = System.IO.Path.Combine(mainDirectory, this.fileNamePrj);
            System.IO.File.WriteAllText(prjFile, fullFolder);


            // INI and workspace file
            string iniFile = System.IO.Path.Combine(mainDirectory, this.iniFileName);
            string workspaceXml = System.IO.Path.Combine(mainDirectory, this.worspaceName);
            string projectFileInFolder = System.IO.Path.Combine(fullFolder, this.projectName);
            string edbFileInFolder = System.IO.Path.Combine(fullFolder, this.edbFileName);

            // iniFile
            string[] textIniFile = { "[projectdir]", String.Format("dir={0}", this.workspaceFolder) };
            System.IO.File.WriteAllLines(iniFile, textIniFile);

            // workspaceXml
            string[] textWorkspaceXml = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>workspacepointer</filetype>",
        "<version>6811715</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark></remark>", "<encryptionlevel>5150044</encryptionlevel>", "</Header>",
        "<current_workspace>", String.Format("<absolute_path> {0} </absolute_path>", this.workspaceFolder),
        String.Format("<last_active> {0} </last_active>", this.projectFolderName), "</current_workspace>", "</ENVI-MET_Datafile>"};
            System.IO.File.WriteAllLines(workspaceXml, textWorkspaceXml);

            // projectFileInFolder
            string[] textProjectFileInFolder = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>infoX ENVI-met Project Description File</filetype>",
        "<version>4240697</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark></remark>", "<encryptionlevel>5220697</encryptionlevel>", "</Header>",
        "<project_description>", String.Format("<name> {0} </name>", this.projectFolderName),
        "<description>  </description>", "<useProjectDB> 1 </useProjectDB>", "</project_description>", "</ENVI-MET_Datafile>"};
            System.IO.File.WriteAllLines(projectFileInFolder, textProjectFileInFolder);

            // edbFileInFolder
            string[] textEdbFileInFolder = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>DATA</filetype>",
        "<version>1</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark>Envi-Data</remark>", "<encryptionlevel>1701377</encryptionlevel>",
        "</Header>", "</ENVI-MET_Datafile>"};
            System.IO.File.WriteAllLines(edbFileInFolder, textEdbFileInFolder);

            return fullFolder;
        }


        public static string FindENVI_MET(string ENVImetInstallFolder)
        {

            string root = System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string directory = System.IO.Path.Combine(root, "ENVImet4\\sys.basedata\\");

            // custom forlder
            if (ENVImetInstallFolder != null)
            {
                directory = System.IO.Path.Combine(ENVImetInstallFolder, "sys.basedata\\");
            }

            if (System.IO.Directory.Exists(directory))
            {
                return directory;
            }
            else
            {
                return null;
            }
        }


        public static string CreateDestinationFolder()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string destinationFolder = System.IO.Path.Combine(appDataFolder, "DragonflyEnvimet");

            // create folder if not exist
            if (!(System.IO.Directory.Exists(destinationFolder)))
                System.IO.Directory.CreateDirectory(destinationFolder);

            return destinationFolder;
        }

    }


    public class WriteINX
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


        static public string Element2dCalculation(envimetGeometry.Element2dMatrix envimentObj, envimetGeometry.AutoGrid myGrid, double tol)
        {

            List<int[,]> elementList = new List<int[,]>();

            foreach (Brep el in envimentObj.Geometries)
            {
                int index = envimentObj.Geometries.IndexOf(el) + 1;
                int[,] grid2d = myGrid.BasePoints2d(el, index, tol);
                elementList.Add(grid2d);
            }

            int[,] integerMatrix = myGrid.mergeMatrix2d(elementList);

            string matriceEnvimet = envimentObj.MatrixWithMaterials(integerMatrix);

            return matriceEnvimet;

        }


        static public string SimpleWallHorizontalOverhangCalculation(envimetGeometry.SimpleWall envimentObj, envimetGeometry.AutoGrid myGrid, double tol)
        {
            string textXmlSimpleWall = null;

            for (int i = 0; i < envimentObj.Geometries.Count; i++)
            {
                envimentObj.SimpleWallStringCalcZdir(envimentObj.Geometries[i], i, tol, myGrid, ref textXmlSimpleWall);
            }

            return textXmlSimpleWall;

        }


        static public void Three3dCalculation(envimetGeometry.ThreeDimensionalPlants envimentObj, envimetGeometry.AutoGrid myGrid, double tol)
        {

            List<int[,]> elementList = new List<int[,]>();

            foreach (Brep el in envimentObj.Threes)
            {
                int index = envimentObj.Threes.IndexOf(el) + 1;
                int[,] grid2d = myGrid.BasePoints2d(el, index, tol);
                elementList.Add(grid2d);
            }

            int[,] integerMatrix = myGrid.mergeMatrix2d(elementList);
            envimentObj.GenerateLists(integerMatrix);

        }

        public static string BuildingCalculation(envimetGeometry.BuildingMatrix edifici, envimetGeometry.AutoGrid myGrid, double tol, Point3d[] terrainPoints, ref string dbMatrix, ref string vBuildingMatrixId, ref string vBuildingMatrixBottom, ref string vBuildingMatrixUp, ref string dbGreenMatrix)
        {

            // building calculation
            string buildingFlagAndNr = null;
            string buildingFlagAndNrGreen = null;
            List<int> greenBuildingsId = edifici.GreenIdBuildings;

            List<int[,,]> listaArray = new List<int[,,]>();
            List<int[,,]> listaArrayGreen = new List<int[,,]>();

            foreach (Mesh m in edifici.Buildings)
            {
                try
                {
                    int index = edifici.Buildings.IndexOf(m) + 1;

                    Point3d[] buildingPoints = myGrid.VoxelPoints(m, tol);

                    int[,,] grid3d = myGrid.VoxMatrixBuilding(buildingPoints, terrainPoints, index, ref buildingFlagAndNr);

                    //int[,,] grid3d = myGrid.BasePoints(m, index, tol, ref buildingFlagAndNr);

                    listaArray.Add(grid3d);
                }
                catch
                {
                    continue;
                }
            }

            /*
                GREENINGS
            */
            if (greenBuildingsId != null)
            {

                int count = 1;
                foreach (int index in greenBuildingsId)
                {

                    Point3d[] buildingPoints = myGrid.VoxelPoints(edifici.Buildings[index], tol);

                    int[,,] grid3d = myGrid.VoxMatrixBuilding(buildingPoints, terrainPoints, count, ref buildingFlagAndNrGreen);

                    listaArrayGreen.Add(grid3d);

                    //int[,,] grid3d = myGrid.BasePoints(edifici.Buildings[index], count, tol, ref buildingFlagAndNrGreen);

                    count += 1;
                }
                // green
                int[,,] integerGreen = myGrid.mergeMatrix(listaArrayGreen);
                dbGreenMatrix = edifici.GreenSparseMatrix(integerGreen);
            }

            // make materials
            // preparation and delegate
            int[,,] integerMatrix = myGrid.mergeMatrix(listaArray);
            dbMatrix = edifici.NormalSparseMatrix(integerMatrix);

            // 2d matrix
            envimetGeometry.BuildingMatrix.BuildingMatrix2d matrix2d = edifici.BuildingId2d;
            int[,] buildingId2d = matrix2d(integerMatrix);

            matrix2d = edifici.BuildingBottom2d;
            int[,] buildingBottom2d = matrix2d(integerMatrix);

            matrix2d = edifici.BuildingTop2d;
            int[,] buildingTop2d = matrix2d(integerMatrix);

            // static method for visualization / string parsing
            vBuildingMatrixId = envimetGeometry.AutoGrid.View2dMatrix(buildingId2d);
            vBuildingMatrixBottom = envimetGeometry.AutoGrid.View2dMatrix(buildingBottom2d);
            vBuildingMatrixUp = envimetGeometry.AutoGrid.View2dMatrix(buildingTop2d);

            return buildingFlagAndNr;

        }

        // WRITER

        static public void INXwriteMethod(
          string path,
          envimetGeometry.AutoGrid grid,
          envimetGeometry.NestingGrid nestingGrid,
          envimentIntegration.LocationFromLB location,
          envimetGeometry.BuildingMatrix building,
          envimetGeometry.Element2dMatrix simpleplants2D,
          envimetGeometry.Element2dMatrix soils2D,
          envimetGeometry.Element2dMatrix sources2D,
          envimetGeometry.ThreeDimensionalPlants plants3D,
          envimetGeometry.Dem dem,
          envimetGeometry.SimpleWall simpleW
          )
        {

            // preparation
            var now = DateTime.Now;
            string revisionDate = now.ToString("yyyy-MM-dd HH:mm:ss");
            const double tol = 0.0001;
            const string NewLine = "\n";

            string emptyMatrixNull = NewLine + envimetGeometry.AutoGrid.EmptyMatrix("");
            string emptyMatrixZero = NewLine + envimetGeometry.AutoGrid.EmptyMatrix("0");

            string emptyMatrixSoil = NewLine + envimetGeometry.AutoGrid.EmptyMatrix("000000");

            string dbMatrix = null;
            string dbGreenMatrix = null;

            string vBuildingMatrixId = null;
            string vBuildingMatrixBottom = null;
            string vBuildingMatrixUp = null;

            string terrainflag = null;
            string terrainHeight = emptyMatrixZero;


            // calculation
            // terrain!
            List<Point3d> terrainPoints = new List<Point3d>();
            if (dem != null)
            {
                RhinoApp.WriteLine("OK");
                terrainPoints = grid.VoxelPoints(dem.TerrainMesh, tol).ToList();

                int[,,] grid3dDem = grid.VoxMatrixTerrain(terrainPoints.ToArray(), ref terrainflag);

                int[,] demTop2d = dem.DemTop2d(grid3dDem);

                terrainHeight = envimetGeometry.AutoGrid.View2dMatrix(demTop2d);

            }

            string buildingFlagAndNr = WriteINX.BuildingCalculation(building, grid, tol, terrainPoints.ToArray(), ref dbMatrix, ref vBuildingMatrixId, ref vBuildingMatrixBottom, ref vBuildingMatrixUp, ref dbGreenMatrix);

            string ID_plants1D = null;
            if (simpleplants2D != null)
            {
                ID_plants1D = NewLine + WriteINX.Element2dCalculation(simpleplants2D, grid, tol);
            }
            else
            {
                ID_plants1D = emptyMatrixNull;
            }

            string ID_soilprofile = NewLine + WriteINX.Element2dCalculation(soils2D, grid, tol);

            string ID_sources = null;
            if (sources2D != null)
            {
                ID_sources = NewLine + WriteINX.Element2dCalculation(sources2D, grid, tol);
            }
            else
            {
                ID_sources = emptyMatrixNull;
            }


            string sparseMatrixSimpleWall = null;
            if (simpleW != null)
            {
                sparseMatrixSimpleWall = NewLine + WriteINX.SimpleWallHorizontalOverhangCalculation(simpleW, grid, tol);
            }
            else
            {
                sparseMatrixSimpleWall = NewLine;
            }

            // start with xml

            XmlTextWriter xWriter = new XmlTextWriter(path, Encoding.UTF8);
            xWriter.WriteStartElement("ENVI-MET_Datafile");
            xWriter.WriteString(NewLine + " ");

            string[] empty = { };

            // section Header
            string headerTitle = "Header";
            string[] headerTag = new string[] { "filetype", "version", "revisiondate", "remark", "encryptionlevel" };
            string[] headerValue = new string[] { "INPX ENVI-met Area Input File", "401", revisionDate, "Created with lb_envimet", "0" };

            WriteINX.CreateXmlSection(xWriter, headerTitle, headerTag, headerValue, 0, empty);

            // section baseData
            string baseDataTitle = "baseData";
            string[] baseDataTag = new string[] { "modelDescription", "modelAuthor" };
            string[] baseDataValue = new string[] { " A brave new area ", " DragonFly envimet " };

            WriteINX.CreateXmlSection(xWriter, baseDataTitle, baseDataTag, baseDataValue, 0, empty);

            // section modelGeometry
            // preparation telescope / splitted grid

            string useSplitting = null;
            string verticalStretch = null;
            string startStretch = null;
            string grids3DK = null;
            string gridsZ = null;
            string useTelescoping = null;
            string gridsI = (envimetGeometry.AutoGrid.NumX + 1).ToString();
            string gridsJ = (envimetGeometry.AutoGrid.NumY + 1).ToString();
            string[] attribute2dElements = { "matrix-data", gridsI, gridsJ };
            string dx = grid.DimX.ToString("n5");
            string dy = grid.DimY.ToString("n5");
            string dz = grid.DimZ.ToString("n5");

            if (grid.telescope.HasValue)
            {
                useTelescoping = "1";
                useSplitting = "0";
                verticalStretch = grid.telescope.ToString();
                startStretch = grid.StartTelescopeHeight.ToString();
                grids3DK = gridsZ = grid.ZGrids.ToString();
            }
            else
            {
                useTelescoping = "0";
                useSplitting = "1";
                verticalStretch = "0";
                startStretch = "0";
                gridsZ = (grid.ZGrids - 4).ToString();
                grids3DK = (grid.ZGrids).ToString();
            }

            string modelGeometryTitle = "modelGeometry";
            string[] modelGeometryTag = new string[] { "grids-I", "grids-J", "grids-Z", "dx", "dy", "dz-base", "useTelescoping_grid", "useSplitting", "verticalStretch", "startStretch", "has3DModel", "isFull3DDesign" };
            string[] modelGeometryValue = new string[] { gridsI, gridsJ, gridsZ, dx, dy, dz, useTelescoping, useSplitting, verticalStretch, startStretch, "1", "1" };

            WriteINX.CreateXmlSection(xWriter, modelGeometryTitle, modelGeometryTag, modelGeometryValue, 0, empty);


            // section nestingArea
            string nestingAreaTitle = "nestingArea";
            string[] nestingAreaTag = new string[] { "numberNestinggrids", "soilProfileA", "soilProfileB" };
            string[] nestingAreaValue = new string[] { nestingGrid.NumNestingGrid.ToString(), nestingGrid.SoilProfileA, nestingGrid.SoilProfileB };

            WriteINX.CreateXmlSection(xWriter, nestingAreaTitle, nestingAreaTag, nestingAreaValue, 0, empty);


            // section locationData
            string locationDataTitle = "locationData";
            string[] locationDataTag = new string[] { "modelRotation", "projectionSystem", "realworldLowerLeft_X", "realworldLowerLeft_Y", "locationName", "location_Longitude", "location_Latitude", "locationTimeZone_Name", "locationTimeZone_Longitude" };
            string[] locationDataValue = new string[] { location.ModelRotation.ToString("n5"), " ", " 0.00000 ", " 0.00000 ", location.LocationName, location.Longitude.ToString(), location.Latitude.ToString(), location.TimeZone, " 15.00000 " };

            WriteINX.CreateXmlSection(xWriter, locationDataTitle, locationDataTag, locationDataValue, 0, empty);


            // section defaultSettings
            string defaultSettingsTitle = "defaultSettings";
            string[] defaultSettingsTag = new string[] { "commonWallMaterial", "commonRoofMaterial" };
            string[] defaultSettingsValue = new string[] { building.CommonWallMaterial, building.CommonRoofMaterial };

            WriteINX.CreateXmlSection(xWriter, defaultSettingsTitle, defaultSettingsTag, defaultSettingsValue, 0, empty);


            // section buildings2D
            string buildings2DTitle = "buildings2D";
            string[] buildings2DTag = new string[] { "zTop", "zBottom", "buildingNr", "fixedheight" };
            string[] buildings2DValue = new string[] { NewLine + vBuildingMatrixUp, NewLine + vBuildingMatrixBottom, NewLine + vBuildingMatrixId, emptyMatrixZero };

            WriteINX.CreateXmlSection(xWriter, buildings2DTitle, buildings2DTag, buildings2DValue, 1, attribute2dElements);


            // section simpleplants2D
            string simpleplants2DTitle = "simpleplants2D";
            string[] simpleplants2DTag = new string[] { "ID_plants1D" };
            string[] simpleplants2DValue = new string[] { ID_plants1D };

            WriteINX.CreateXmlSection(xWriter, simpleplants2DTitle, simpleplants2DTag, simpleplants2DValue, 1, attribute2dElements);


            // section plant3d
            if (plants3D != null)
            {
                WriteINX.Three3dCalculation(plants3D, grid, tol);

                for (int i = 0; i < plants3D.PropertiesTree.Count; i++)
                {

                    string plants3DTitle = "3Dplants";
                    string[] plants3DTag = new string[] { "rootcell_i", "rootcell_j", "rootcell_k", "plantID", "name", "observe" };
                    string[] plants3DValue = new string[] { plants3D.PropertiesTree[i][0], plants3D.PropertiesTree[i][1], plants3D.PropertiesTree[i][2], plants3D.PropertiesTree[i][3], plants3D.PropertiesTree[i][4], plants3D.PropertiesTree[i][5] };

                    WriteINX.CreateXmlSection(xWriter, plants3DTitle, plants3DTag, plants3DValue, 0, empty);
                }
            }


            // section soils2D
            string soils2DTitle = "soils2D";
            string[] soils2DTag = new string[] { "ID_soilprofile" };
            string[] soils2DValue = new string[] { ID_soilprofile };

            WriteINX.CreateXmlSection(xWriter, soils2DTitle, soils2DTag, soils2DValue, 1, attribute2dElements);


            // section dem (next release)
            string demTitle = "dem";
            string[] demDTag = new string[] { "terrainheight" };

            string[] demValue = new string[] { terrainHeight };
            WriteINX.CreateXmlSection(xWriter, demTitle, demDTag, demValue, 1, attribute2dElements);


            // section sources2D
            string sources2DTitle = "sources2D";
            string[] sources2DTag = new string[] { "ID_sources" };
            string[] sources2DValue = new string[] { ID_sources };

            WriteINX.CreateXmlSection(xWriter, sources2DTitle, sources2DTag, sources2DValue, 1, attribute2dElements);


            // section receptors2D (next release)
            string receptors2DTitle = "receptors2D";
            string[] receptors2DTag = new string[] { "ID_receptors" };
            string[] receptors2DValue = new string[] { emptyMatrixNull };

            WriteINX.CreateXmlSection(xWriter, receptors2DTitle, receptors2DTag, receptors2DValue, 1, attribute2dElements);


            // section additionalData (maybe next release)
            string additionalDataTitle = "additionalData";
            string[] additionalDataTag = new string[] { "db_link_point", "db_link_area" };
            string[] additionalDataValue = new string[] { emptyMatrixNull, emptyMatrixNull };

            WriteINX.CreateXmlSection(xWriter, additionalDataTitle, additionalDataTag, additionalDataValue, 1, attribute2dElements);

            // section greenins part 1
            /////////////////////////////////////////
            if (building.GreenIdBuildings.Count > 0)
            {
                for (int i = 0; i < building.GreenIdBuildings.Count; i++)
                {

                    string buildinginfoTitle = "Buildinginfo";
                    string[] buildinginfoTag = new string[] { "BuildingInternalNr", "BuildingName", "BuildingWallMaterial", "BuildingRoofMaterial", "BuildingFacadeGreening", "BuildingRoofGreening" };
                    string[] buildinginfoValue = new string[] { (building.GreenIdBuildings[i] + 1).ToString(), " ", building.selectedWallMaterialGreenings[i], building.selectedRoofMaterialGreenings[i], building.greenWallMaterial[i], building.greenRoofMaterial[i] };

                    WriteINX.CreateXmlSection(xWriter, buildinginfoTitle, buildinginfoTag, buildinginfoValue, 0, empty);
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

            WriteINX.CreateXmlSection(xWriter, modelGeometry3DTitle, modelGeometry3DTag, modelGeometry3DValue, 0, empty);


            // section buildings3D
            string buildings3DTitle = "buildings3D";
            string[] buildings3DTag = new string[] { "buildingFlagAndNr" };
            string[] buildings3DValue = new string[] { NewLine + buildingFlagAndNr };

            WriteINX.CreateXmlSection(xWriter, buildings3DTitle, buildings3DTag, buildings3DValue, 2, attribute3dElementsBuilding);


            // section dem3D (next release)
            string dem3DTitle = "dem3D";
            string[] dem3DTag = new string[] { "terrainflag" };
            string[] dem3DValue = new string[] { NewLine + terrainflag };

            WriteINX.CreateXmlSection(xWriter, dem3DTitle, dem3DTag, dem3DValue, 2, attribute3dElementsTerrain);


            // section WallDB
            string WallDBTitle = "WallDB";
            string[] WallDBTag = new string[] { "ID_wallDB" };
            string[] WallDBValue = new string[] { NewLine + dbMatrix };

            WriteINX.CreateXmlSection(xWriter, WallDBTitle, WallDBTag, WallDBValue, 2, attribute3dElementsWallDB);


            // section SingleWallDB (maybe next release)
            string SingleWallDBTitle = "SingleWallDB";
            string[] SingleWallDBTag = new string[] { "ID_singlewallDB" };
            string[] SingleWallDBValue = new string[] { sparseMatrixSimpleWall };

            WriteINX.CreateXmlSection(xWriter, SingleWallDBTitle, SingleWallDBTag, SingleWallDBValue, 2, attribute3dElementsWallDB);


            // section GreeningDB
            string GreeningDBTitle = "GreeningDB";
            string[] GreeningDBTag = new string[] { "ID_GreeningDB" };
            string[] GreeningDBValue = null;
            if (building.GreenIdBuildings.Count == 0)
            {
                GreeningDBValue = new string[] { " " };
            }
            else
            {
                GreeningDBValue = new string[] { NewLine + dbGreenMatrix };
            }

            WriteINX.CreateXmlSection(xWriter, GreeningDBTitle, GreeningDBTag, GreeningDBValue, 2, attribute3dElementsWallDB);

            xWriter.WriteEndElement();
            xWriter.Close();
        }
    }


    public class ReadEnvimet
    {
        public string Metaname { get; set; }


        private string ReadEnvimetNoBinaryFile()
        {
            string characters = @"[^\s()_<>/,\.A-Za-z0-9=""]+";
            Encoding isoLatin1 = Encoding.GetEncoding(28591); ;
            string text = System.IO.File.ReadAllText(Metaname, isoLatin1);

            Regex.Replace(characters, "", text);

            return text.Replace("&", "").Replace("<Remark for this Source Type>", "");
        }

        public string WriteReadebleEDXFile(string path, string variableName = "ENVI", string fileType = ".EDX")
        {
            string fileNameWithExtension = variableName + fileType;

            string newFile = System.IO.Path.Combine(path, fileNameWithExtension);

            // make a readible version of the xml file
            string metainfo = ReadEnvimetNoBinaryFile();

            // write file in a new destination
            System.IO.File.WriteAllText(newFile, metainfo);

            return newFile;
        }
    }
}
