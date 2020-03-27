using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Location = df_envimet_lib.Settings.Location;
using Preparation = df_envimet_lib.IO.Preparation;
using GridLib = df_envimet_lib.Geometry.Grid;
using df_envimet_lib.Geometry;

namespace df_envimet.Grasshopper.IO
{
    public class Inx : GH_Component
    {

        private const string BASE_LOC_NAME = "Site:Location,\nunknown_Location,\n0.0, !Latitude\n0.0, !Longitude\n0.0, !Time Zone\n0.0; !Elevation\n";
        private const int BASE_LOC_ROTATION = 0;

        /// <summary>
        /// Initializes a new instance of the Inx class.
        /// </summary>
        public Inx()
          : base("DF Envimet Spaces", "DFEnvimetSpaces",
              "Use this component to generate ENVI-Met v4.4.4 3D geometry models.\nAnalyze parametric models with ENVI - Met!\nSave the model in the ENVI_MET Workspace, create a simulation file with \"DF Envimet Config\" and run the simulation.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_envimetFolder", "_envimetFolder", "The folder into which you would like to write the envimet model. This should be a complete file path to the folder.", GH_ParamAccess.item);
            pManager.AddGenericParameter("_envimetLocation", "_envimetLocation", "Location data which comes from \"Dragonfly Envimet Location\" component.", GH_ParamAccess.item);

            pManager.AddGenericParameter("_envimentGrid", "_envimentGrid", "Connect the output of \"Dragonfly Envimet Grid\".", GH_ParamAccess.item);
            pManager.AddGenericParameter("nestingGrid_", "nestingGrid_", "Connect the output of \"Dragonfly Envimet Nesting Grid\".", GH_ParamAccess.item);

            pManager.AddGenericParameter("_envimentObjects_", "_envimentObjects_", "Connect envimet buildings, trees, soils, source, terrain here.", GH_ParamAccess.list);

            pManager.AddTextParameter("fileName_", "fileName_", "The file name that you would like the envimet model to be saved as. Default name is \"DragonflyEnvimet\".", GH_ParamAccess.item, "DragonflyEnvimet");
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set to \"True\" to run the component and generate the envimet model.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("viewGrid_", "viewGrid_", "Set to \"True\" to run the view grid.", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[4].DataMapping = GH_DataMapping.Flatten;
            pManager[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("ModelInfo", "ModelInfo", "Information about Model grid.", GH_ParamAccess.item);
            pManager.AddPointParameter("XYGrid", "XYGrid", "XY points.", GH_ParamAccess.list);
            pManager.AddPointParameter("XZGrid", "XZGrid", "XZ points.", GH_ParamAccess.list);
            pManager.AddPointParameter("YZGrid", "YZGrid", "YZ points.", GH_ParamAccess.list);
            pManager.AddTextParameter("INXfileAddress", "INXfileAddress", "The file path of the inx result file that has been generated on your machine.", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // output path
            string _envimetFolder = null;

            // location and grid
            Location _envimetLocation = GetLocationInit(BASE_LOC_NAME, BASE_LOC_ROTATION);
            GridLib _envimentGrid = new GridLib();
            NestingGrid nestingGrid_ = new NestingGrid();

            // list objects
            List<Entity> _envimentObjects_ = new List<Entity>();

            // path
            string fileName_ = "Morpho";

            // toggle
            bool _runIt = false;
            bool viewGrid_ = false;


            DA.GetData(0, ref _envimetFolder);
            DA.GetData(1, ref _envimetLocation);
            DA.GetData(2, ref _envimentGrid);
            DA.GetData(3, ref nestingGrid_);
            DA.GetDataList(4, _envimentObjects_);
            DA.GetData(5, ref fileName_);
            DA.GetData(6, ref _runIt);
            DA.GetData(7, ref viewGrid_);

            if (_envimetFolder == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Use DF Envimet Installation Directory to set installation folder of envimet.");
                return;
            }
            Preparation preparation = new Preparation(_envimentGrid, _envimetLocation);

            string information = String.Format("Dimension of grid: {0},{1},{2}\nNumber of nesting grid: {3}", _envimentGrid.NumX, _envimentGrid.NumY, _envimentGrid.NumZ, nestingGrid_.NumNestingGrid);
            DA.SetData(0, information);

            if (viewGrid_)
            {
                DA.SetDataList(1, _envimentGrid.GridXY());
                DA.SetDataList(2, _envimentGrid.GridXZ());
                DA.SetDataList(3, _envimentGrid.GridYZ());
            }

            if (_runIt)
            {
                // default preparation object
                List<Building> buildings = new List<Building>();
                List<Object2d> soils = new List<Object2d>();
                List<Object2d> plant2d = new List<Object2d>();
                List<Plant3d> plant3d = new List<Plant3d>();
                List<Receptor> receptors = new List<Receptor>();
                List<Object2d> sources = new List<Object2d>();
                List<Terrain> terrain = new List<Terrain>();
                List<SimpleWall> simpleW = new List<SimpleWall>();


                preparation.EnvimetPart["grid"] = _envimentGrid;
                preparation.EnvimetPart["nestingGrid"] = nestingGrid_;
                preparation.EnvimetPart["location"] = _envimetLocation;

                if (_envimentObjects_.Count > 0)
                {
                    Dictionary<string, List<Entity>> objectDict = GetObjectDivideByType(_envimentObjects_);

                    string soilMatrix = null;
                    string plant2dMatrix = null;
                    string sourceMatrix = null;
                    string simpleWallShadingsMatrix = null;
                    List<string[]> plant3dXml = new List<string[]>();
                    List<string[]> receptorsXml = new List<string[]>();

                    // building and terrain
                    if (objectDict["building"].Count > 0 && objectDict["terrain"].Count == 0)
                    {
                        buildings = objectDict["building"].ConvertAll(o => (Building)o);
                        Tuple<Dictionary<string, string>, List<string[]>> buildingComponents = Preparation.GetBuildingPreparationMatrix(_envimentGrid, buildings, null, demVoxel: new Point3d[] { });
                        if (BuildingWarning(buildingComponents))
                            return;
                        preparation.EnvimetPart["building"] = buildingComponents;
                    }
                    else if (objectDict["building"].Count == 0 && objectDict["terrain"].Count > 0)
                    {
                        terrain = objectDict["terrain"].ConvertAll(o => (Terrain)o);
                        Warning(terrain);
                        Tuple<Point3d[], string, string> terrainComponents = Preparation.GetDemMatrix(terrain[0], _envimentGrid);
                        preparation.EnvimetPart["dem"] = terrainComponents;
                    }
                    else if (objectDict["building"].Count > 0 && objectDict["terrain"].Count > 0)
                    {
                        terrain = objectDict["terrain"].ConvertAll(o => (Terrain)o);
                        buildings = objectDict["building"].ConvertAll(o => (Building)o);

                        Warning(terrain);
                        Tuple<Point3d[], string, string> terrainComponents = Preparation.GetDemMatrix(terrain[0], _envimentGrid);
                        preparation.EnvimetPart["dem"] = terrainComponents;

                        Tuple<Dictionary<string, string>, List<string[]>> buildingComponents = Preparation.GetBuildingPreparationMatrix(_envimentGrid, buildings, terrain[0], demVoxel: terrainComponents.Item1);
                        if (BuildingWarning(buildingComponents))
                            return;
                        preparation.EnvimetPart["building"] = buildingComponents;
                    }
                    
                    if (objectDict["soil"].Count > 0)
                    {
                        soils = objectDict["soil"].ConvertAll(o => (Object2d)o);
                        soilMatrix = Preparation.Get2dObjectMatrix(_envimentGrid, soils, Material.CommonSoilMaterial);
                        preparation.EnvimetPart["soils2D"] = soilMatrix;
                    }
                    if (objectDict["plant2d"].Count > 0)
                    {
                        plant2d = objectDict["plant2d"].ConvertAll(o => (Object2d)o);
                        plant2dMatrix = Preparation.Get2dObjectMatrix(_envimentGrid, plant2d, Material.DefaultEmpty);
                        preparation.EnvimetPart["simpleplants2D"] = plant2dMatrix;
                    }
                    if (objectDict["plant3d"].Count > 0)
                    {
                        plant3d = objectDict["plant3d"].ConvertAll(o => (Plant3d)o);
                        plant3dXml = Preparation.GetPlant3dMatrix(_envimentGrid, plant3d);
                        preparation.EnvimetPart["plant3d"] = plant3dXml;
                    }
                    if (objectDict["receptors"].Count > 0)
                    {
                        receptors = objectDict["receptors"].ConvertAll(o => (Receptor)o);
                        receptorsXml = Preparation.GetReceptorsMatrix(_envimentGrid, receptors);
                        preparation.EnvimetPart["receptors"] = receptorsXml;
                    }
                    if (objectDict["source"].Count > 0)
                    {
                        sources = objectDict["source"].ConvertAll(o => (Object2d)o);
                        sourceMatrix = Preparation.Get2dObjectMatrix(_envimentGrid, sources, Material.DefaultEmpty);
                        preparation.EnvimetPart["sources2D"] = sourceMatrix;
                    }
                    if (objectDict["simpleW"].Count > 0)
                    {
                        simpleW = objectDict["simpleW"].ConvertAll(o => (SimpleWall)o);
                        simpleWallShadingsMatrix = Preparation.GetSimpleWallZMatrix(_envimentGrid, simpleW);
                        preparation.EnvimetPart["simpleW"] = simpleWallShadingsMatrix;
                    }

                }

                string fileName = (fileName_ != null) ? fileName_ + ".INX" : "DragonflyEnvimet.INX";
                string fullName = System.IO.Path.Combine(_envimetFolder, fileName);

                df_envimet_lib.IO.Inx.INXwriteMethod(fullName, preparation);
                DA.SetData(4, fullName);
                CleanObject(_envimentObjects_);
            }
            
        }
        
        private bool BuildingWarning(Tuple<Dictionary<string, string>, List<string[]>> buildingComponents)
        {
            if (buildingComponents == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Make sure buildings are inside grid at 2 cells of distance from border at least.");
                return true;
            }
            return false;
        }
        

        private void Warning(List<Terrain> terrain)
        {
            if (terrain.Count > 1)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only one terrain object will be consider.");
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
                return Properties.Resources.envimetSpacesIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("21e4a9c8-20fd-4220-b5a1-468cd3ca2cca"); }
        }

        private void CleanObject(List<Entity> inputObjects)
        {
            inputObjects.ForEach(o =>
            {
                if (o != null) { o.Dispose(); }
            });
        }

        private Dictionary<string, List<Entity>> GetObjectDivideByType(List<Entity> inputObjects)
        {
            Dictionary<string, List<Entity>> objectDivideByType = new Dictionary<string, List<Entity>>();

            List<Entity> buildings = new List<Entity>();
            List<Entity> soils = new List<Entity>();
            List<Entity> plant2d = new List<Entity>();
            List<Entity> plant3d = new List<Entity>();
            List<Entity> receptors = new List<Entity>();
            List<Entity> sources = new List<Entity>();
            List<Entity> terrain = new List<Entity>();
            List<Entity> simpleWall = new List<Entity>();


            inputObjects.ForEach(o => {
                if (o is Building) { buildings.Add((Building)o); }
                else if (o is Soil) { soils.Add((Soil)o); }
                else if (o is Plant2d) { plant2d.Add((Plant2d)o); }
                else if (o is Plant3d) { plant3d.Add((Plant3d)o); }
                else if (o is Receptor) { receptors.Add((Receptor)o); }
                else if (o is Source) { sources.Add((Source)o); }
                else if (o is Terrain) { terrain.Add((Terrain)o); }
                else if (o is SimpleWall) { simpleWall.Add((SimpleWall)o); }
            });

            objectDivideByType.Add("building", buildings);
            objectDivideByType.Add("soil", soils);
            objectDivideByType.Add("plant2d", plant2d);
            objectDivideByType.Add("plant3d", plant3d);
            objectDivideByType.Add("receptors", receptors);
            objectDivideByType.Add("source", sources);
            objectDivideByType.Add("terrain", terrain);
            objectDivideByType.Add("simpleW", simpleWall);

            return objectDivideByType;
        }

        private Location GetLocationInit(string location, int modelRotation)
        {
            return new Location(location, modelRotation);
        }

    }
}