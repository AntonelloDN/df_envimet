using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using MorphoEnvimetLibrary.Settings;
using MorphoEnvimetLibrary.Geometry;

namespace MorphoEnvimetLibrary.IO
{
    public class Preparation
    {
        public Dictionary<string, object> EnvimetPart { get; set; }

        // keys: 
        public Preparation(Grid grid, Location location)
        {
            grid.CalcGridXY(new List<Mesh>() { grid.Surface });
            grid.CalcGzDimension();
            Matrix2d empty2dMatrix = grid.CreateBase2dMatrix();

            EnvimetPart = new Dictionary<string, object>();

            string envimetZeroMatrix = Grid.CreateTextMatrix(empty2dMatrix, "0");
            string envimetEmptyMatrix = Grid.CreateTextMatrix(empty2dMatrix, "");
            string emptySoilMatrix = Grid.CreateTextMatrix(empty2dMatrix, Material.CommonSoilMaterial);
            EnvimetPart.Add("envimetZeroMatrix", envimetZeroMatrix);
            EnvimetPart.Add("envimetEmptyMatrix", envimetEmptyMatrix);
            EnvimetPart.Add("emptySoilMatrix", emptySoilMatrix);

            // default building
            Tuple<Dictionary<string, string>, List<string[]>> defaultBuilding;
            Dictionary<string, string> defaultBuildingDict = new Dictionary<string, string>
            {
                { "buildingNumberMatrix", "" },
                { "wallDBMatrix", "" },
                { "greenDBMatrix", "\n" },
                { "topMatrix", envimetZeroMatrix },
                { "bottomMatrix", envimetZeroMatrix },
                { "fixedMatrix", envimetZeroMatrix },
                { "idMatrix", envimetZeroMatrix }
            };

            defaultBuilding = new Tuple<Dictionary<string, string>, List<string[]>>(defaultBuildingDict, new List<string[]>());

            // default dem
            Tuple<Point3d[], string, string> defaultDem = GetDemMatrix(null, grid);

            EnvimetPart.Add("grid", grid);                          // Ok
            EnvimetPart.Add("building", defaultBuilding);           // OK
            EnvimetPart.Add("dem", defaultDem);                     // OK
            EnvimetPart.Add("plant3d", new List<string[]>());       // OK
            EnvimetPart.Add("receptors", new List<string[]>());     // wip
            EnvimetPart.Add("nestingGrid", new NestingGrid());      // Ok
            EnvimetPart.Add("simpleplants2D", envimetEmptyMatrix);  // Ok
            EnvimetPart.Add("soils2D", emptySoilMatrix);            // Ok
            EnvimetPart.Add("sources2D", envimetEmptyMatrix);       // Ok
            EnvimetPart.Add("simpleW", "\n");                       // Ok
            EnvimetPart.Add("location", location);                  // Ok

        }


        public static string Get2dObjectMatrix(Grid grid, List<Object2d> objects, string baseMaterial)
        {
            List<Material> materials = new List<Material>();
            List<Matrix2d> nestedMatrix = new List<Matrix2d>();

            foreach (Object2d o in objects)
            {
                Matrix2d matrix = o.Create2DMatrixPerObj(grid, objects.IndexOf(o) + 1);
                Material mat = o.GetMaterial();
                nestedMatrix.Add(matrix);
                materials.Add(mat);
            }

            string result = Object2d.Merge2dMatrixWithMaterial(nestedMatrix, materials, baseMaterial);

            return result;
        }

        public static Tuple<Point3d[], string, string> GetDemMatrix(Terrain terrain, Grid grid)
        {
            string defaultMatrix = Grid.CreateTextMatrix(new Matrix2d(grid.NumX, grid.NumY), "0");
            Tuple<Point3d[], string, string> result = new Tuple<Point3d[], string, string>(null, "", defaultMatrix);

            if (terrain != null)
            {
                Point3d[] demVoxel = Building.VoxelPoints(terrain.GetMesh(), grid, tolerance: Building.TOLERANCE);
                terrain.CreateVoxMatrixTerrain(demVoxel, grid);
                Matrix2d maxDem = Terrain.DemTop2d(terrain.GetMatrix(), grid);
                string dem2d = Grid.CreateTextMatrix(maxDem);
                result = new Tuple<Point3d[], string, string>(demVoxel, terrain.TerrainflagMatrix, dem2d);
            }

            return result;
        }

        public static Tuple<Dictionary<string, string>, List<string[]>> GetBuildingPreparationMatrix(Grid grid,
            List<Building> buildings,
            Terrain terrain,
            Point3d[] demVoxel)
        {
            string buildingNumberMatrix = "";
            List<Matrix3d> listMatrixNormalMaterial = new List<Matrix3d>();
            List<Matrix3d> listMatrixGreenMaterial = new List<Matrix3d>();
            List<string> hightMatrixBuilding = new List<string>();
            List<Material> materials = new List<Material>();
            List<string[]> greenInfo = new List<string[]>();

            Dictionary<string, string> buildingDict = new Dictionary<string, string>();
            buildingDict.Add("greenDBMatrix", "\n");

            Mesh union = new Mesh();
            
            buildings.ForEach(b => union.Append(b.GetMesh()));
            if (!CheckDistanceFromBorder(union, grid))
            {
                return null;
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                //union.Append(buildings[i].GetMesh());
                Mesh tempMesh = buildings[i].GetMesh();

                if (terrain != null)
                {
                    tempMesh = Building.MoveBuildingsUp(buildings[i].GetMesh(), terrain.GetSrfMesh());
                }
                if (buildings[i].SimplifiedCalculation)
                {
                    Point3d[] buildingVoxel = Building.VoxelPoints(tempMesh, grid);
                    buildings[i].CreateVoxMatrixBuilding(buildingVoxel, demVoxel, grid, i + 1);
                }
                else
                {
                    Point3d[] buildingVoxel = Building.VoxelPoints(tempMesh, grid, tolerance: Building.TOLERANCE);
                    buildings[i].CreateVoxMatrixBuilding(buildingVoxel, demVoxel, grid, i + 1);
                }

                listMatrixNormalMaterial.Add(buildings[i].GetMatrix3d());
                hightMatrixBuilding.Add(buildings[i].BuildingFlagAndNr);
                materials.Add(buildings[i].GetMaterial());

                buildingNumberMatrix += buildings[i].BuildingFlagAndNr;

                string greenWall = buildings[i].GetMaterial().GreenWallMaterial;
                string greenRoof = buildings[i].GetMaterial().GreenRoofMaterial;
                if (greenWall != Material.DefaultGreenWallMaterial || greenRoof != Material.DefaultGreenRoofMaterial)
                {
                    if (greenWall == Material.DefaultGreenWallMaterial)
                        greenWall = " ";
                        
                    if (greenRoof == Material.DefaultGreenRoofMaterial)
                        greenRoof = " ";

                    listMatrixGreenMaterial.Add(buildings[i].GetMatrix3d());
                    string[] greenBld = new string[] { (i + 1).ToString(), " ", buildings[i].GetMaterial().WallMaterial, buildings[i].GetMaterial().RoofMaterial, greenWall, greenRoof };
                    greenInfo.Add(greenBld);
                }
                buildings[i].Dispose();
                tempMesh.Dispose();
            }

            buildingDict.Add("buildingNumberMatrix", buildingNumberMatrix);

            // Building Matrix
            Matrix3d uniqueMatrix = Matrix3d.Union(listMatrixNormalMaterial);
            buildingDict.Add("wallDBMatrix", Building.SetMaterials(uniqueMatrix, materials));

            if (listMatrixGreenMaterial.Count > 0)
            {
                Matrix3d uniqueGreenMatrix = Matrix3d.Union(listMatrixGreenMaterial);
                buildingDict["greenDBMatrix"] = Building.SetMaterials(uniqueGreenMatrix, materials, green: true);
            }

            // 2d part
            // Top Matrix
            Matrix2d temp = Building.GetMatrixFromRayShoot(grid, union, top: true);
            buildingDict.Add("topMatrix", Grid.CreateTextMatrix(temp));

            // Bottom Matrix
            temp = Building.GetMatrixFromRayShoot(grid, union, top: false);
            buildingDict.Add("bottomMatrix", Grid.CreateTextMatrix(temp));

            // Fixed Matrix
            temp = Building.BuildingFixed2d(uniqueMatrix, grid);
            buildingDict.Add("fixedMatrix", Grid.CreateTextMatrix(matrix: temp));

            // Id Matrix
            temp = Building.BuildingId2d(uniqueMatrix);
            buildingDict.Add("idMatrix", Grid.CreateTextMatrix(matrix: temp));


            return new Tuple<Dictionary<string, string>, List<string[]>>(buildingDict, greenInfo);
        }

        public static List<string[]> GetPlant3dMatrix(Grid grid, List<Plant3d> plants)
        {
            List<string[]> plantMatrix = new List<string[]>();

            foreach (Plant3d plant in plants)
            {
                Matrix2d matrix = plant.Create2DMatrixPerObj(grid, plants.IndexOf(plant) + 1);
                List<string[]> stringForPreparation = plant.CreateAttributes(matrix);
                stringForPreparation.ForEach(l => plantMatrix.Add(l));
            }

            return plantMatrix;
        }

        public static List<string[]> GetReceptorsMatrix(Grid grid, List<Receptor> receptor)
        {
            List<string[]> receptorMatrix = new List<string[]>();

            foreach (Receptor rec in receptor)
            {
                Matrix2d matrix = rec.Create2DMatrixPerObj(grid, receptor.IndexOf(rec) + 1);
                List<string[]> stringForPreparation = rec.CreateAttributes(matrix);
                stringForPreparation.ForEach(l => receptorMatrix.Add(l));
            }

            return receptorMatrix;
        }

        public static string GetSimpleWallZMatrix(Grid grid, List<SimpleWall> shadings)
        {
            string xmlContent = "";
            shadings.ForEach(shd => xmlContent += shd.SimpleWallStringCalcZdir(grid));
            return xmlContent;
        }

        private static bool CheckDistanceFromBorder(Mesh buiding, Grid grid)
        {
            Plane planeWorldXY = Plane.WorldXY;
            List<Point3d> points = grid.GridXY();
            Point3d pMin = points.Min();
            Point3d pMax = points.Max();

            Point3d ptMin = new Point3d(pMin.X + (grid.DimX * Grid.MIN_NUM_BORDER_CELLS), pMin.Y + (grid.DimY * Grid.MIN_NUM_BORDER_CELLS), 0);
            Point3d ptMax = new Point3d(pMax.X - (grid.DimX * Grid.MIN_NUM_BORDER_CELLS), pMax.Y - (grid.DimY * Grid.MIN_NUM_BORDER_CELLS), 0);
            NurbsCurve border = new Rectangle3d(planeWorldXY, ptMin, ptMax).ToNurbsCurve();

            Brep bbox = buiding.GetBoundingBox(true).ToBrep();
            Transform xprj = Transform.PlanarProjection(planeWorldXY);
            bbox.Transform(xprj);

            foreach (BrepVertex v in bbox.Vertices)
            {
                if (border.Contains(v.Location, planeWorldXY, tolerance: 0.01) == PointContainment.Outside)
                {
                    return false;
                }
            }
            return true;


        }
    }
}
