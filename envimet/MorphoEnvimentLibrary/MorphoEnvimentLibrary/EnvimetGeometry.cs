using System;
using System.Linq;
using Rhino.Geometry;
using System.Collections.Generic;


namespace envimetGeometry
{
    public class AutoGrid
    {
        public double? telescope;

        public int ZGrids { get; set; }
        public double DimX { get; set; }
        public double DimY { get; set; }
        public double DimZ { get; set; }
        public double StartTelescopeHeight { get; set; }
        public int ExtLeftXgrid { get; set; }
        public int ExtRightXgrid { get; set; }
        public int ExtUpYgrid { get; set; }
        public int ExtDownYgrid { get; set; }

        public Mesh Surface { get; set; }

        public static int NumX;
        public static int NumY;
        public static double[] ZNumbers;

        public double MinX { get; private set; }
        public double MaxX { get; private set; }
        public double MinY { get; private set; }
        public double MaxY { get; private set; }
        public int MaxZGrid { get; }

        private const string Approximation = "0.00";
        private const string TerrainDefaultString = "1.00000";
        private const int AvoidWarningZone = 2;
        private const int HugeBoundaryDimension = 10000000;

        public string[] CheckZ
        {
            get
            {
                string[] checkZ = new string[ZNumbers.Length];

                for (int i = 0; i < ZNumbers.Length; i++)
                {
                    checkZ[i] = ZNumbers[i].ToString(Approximation);
                }

                return checkZ;

            }
        }


        public AutoGrid()
        {
            this.ZGrids = 15;
            this.DimX = 3.0;
            this.DimY = 3.0;
            this.DimZ = 3.0;
            this.StartTelescopeHeight = 5.0;
            this.ExtLeftXgrid = 2;
            this.ExtRightXgrid = 2;
            this.ExtUpYgrid = 2;
            this.ExtDownYgrid = 2;
            this.MaxZGrid = 999;
            this.MinX = 0;
            this.MaxX = 0;
            this.MinY = 0;
            this.MaxY = 0;
            this.telescope = null;

            if (!this.telescope.HasValue)
                this.ZGrids += 4;

            if (this.ExtLeftXgrid < AvoidWarningZone)
                this.ExtLeftXgrid = AvoidWarningZone;
            if (this.ExtRightXgrid < AvoidWarningZone)
                this.ExtRightXgrid = AvoidWarningZone;
            if (this.ExtUpYgrid < AvoidWarningZone)
                this.ExtUpYgrid = AvoidWarningZone;
            if (this.ExtDownYgrid < AvoidWarningZone)
                this.ExtDownYgrid = AvoidWarningZone;
            if (this.ExtDownYgrid < AvoidWarningZone)
                this.ExtDownYgrid = AvoidWarningZone;
        }


        public void CalcGridXY(List<Mesh> buildings)
        {
            double distLeft = this.ExtLeftXgrid * this.DimX;
            double distRight = this.ExtRightXgrid * this.DimX;
            double distUp = this.ExtUpYgrid * this.DimY;
            double distDown = this.ExtDownYgrid * this.DimY;

            this.MinX = this.MinY = HugeBoundaryDimension;
            this.MaxX = this.MaxY = -1 * HugeBoundaryDimension;
            double maxZ = -1 * HugeBoundaryDimension;

            foreach (Mesh geo in buildings)
            {
                BoundingBox BB1 = geo.GetBoundingBox(true);
                if (this.MinX > BB1.Min.X)
                    this.MinX = BB1.Min.X;
                if (this.MaxX < BB1.Max.X)
                    this.MaxX = BB1.Max.X;
                if (this.MinY > BB1.Min.Y)
                    this.MinY = BB1.Min.Y;
                if (this.MaxY < BB1.Max.Y)
                    this.MaxY = BB1.Max.Y;
                if (maxZ < BB1.Max.Z)
                    maxZ = BB1.Max.Z;
            }

            // Geometry BoundingBox limits NETO
            this.MinX = this.MinX - distLeft;
            this.MinY = this.MinY - distDown;
            this.MaxX = this.MaxX + distRight;
            this.MaxY = this.MaxY + distUp;

            // Required height -- Twice the heighest building
            double reqHeight = maxZ * AvoidWarningZone;

            double domX = this.MaxX - this.MinX;
            double domY = this.MaxY - this.MinY;
            NumX = (int)(domX / this.DimX);
            NumY = (int)(domY / this.DimY);

            // Reccalculate maxX/Y just for the bounding box fit the grid size/length
            this.MaxX = this.MinX + (NumX * this.DimX);
            this.MaxY = this.MinY + (NumY * this.DimY);

        }


        public void CalcGzDimension()
        {

            // Calculate z info
            // Preparation
            double[] gZ = new double[this.ZGrids];
            double dimZ = this.DimZ;
            double firstGrid = dimZ / 5;
            double grid = 0;

            // Calculate
            for (int i = 1; i < this.ZGrids + 1; i++)
            {
                if (this.telescope == null)
                {
                    switch (i)
                    {
                        case 1:
                            gZ[i - 1] = firstGrid / 2;
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            gZ[i - 1] = (i * firstGrid) - (firstGrid / 2);
                            break;
                        default:
                            gZ[i - 1] = ((i - 4) * dimZ) - (dimZ / 2);
                            break;
                    }
                }
                else
                {
                    if (i == 1 || grid <= this.StartTelescopeHeight)
                    {
                        grid = (i * dimZ) - (dimZ / 2);
                    }
                    else
                    {
                        double g1 = grid;
                        double gz = dimZ;
                        dimZ = dimZ + (dimZ * (double)this.telescope / 100);
                        grid = grid + (dimZ + gz) / 2;
                    }
                    gZ[i - 1] = grid;
                }
            }
            ZNumbers = gZ;
        }


        public List<Point3d> GridXY()
        {
            List<Point3d> gridPoints = new List<Point3d>();
            // XY Grid
            for (int ix = 0; ix < NumX + 1; ix++)
                for (int iy = 0; iy < NumY + 1; iy++)
                    gridPoints.Add(new Point3d((ix * this.DimX) + this.MinX, (iy * this.DimY) + this.MinY, ZNumbers[0]));
            return gridPoints;
        }


        public List<Point3d> GridXZ()
        {
            List<Point3d> gridPoints = new List<Point3d>();
            // XZ Grid
            for (int ix = 0; ix < NumX + 1; ix++)
                for (int iz = 0; iz < this.ZGrids; iz++)
                    gridPoints.Add(new Point3d((ix * this.DimX) + this.MinX, this.MaxY, ZNumbers[iz]));

            return gridPoints;
        }


        public List<Point3d> GridYZ()
        {
            List<Point3d> gridPoints = new List<Point3d>();
            // YZ Grid
            for (int iy = 0; iy < NumY + 1; iy++)
                for (int iz = 0; iz < this.ZGrids; iz++)
                    gridPoints.Add(new Point3d(this.MaxX, (iy * this.DimY) + this.MinY, ZNumbers[iz]));

            return gridPoints;
        }


        public Point3d[] VoxelPoints(Mesh M, double T)
        {
            List<Plane> planes = new List<Plane>();

            for (int i = 0; i < ZNumbers.Length; i++)
            {
                Plane pl = new Plane(new Point3d(0, 0, ZNumbers[i]), Vector3d.ZAxis);
                planes.Add(pl);
            }

            // internal method
            List<Point3d> gridXY = this.GridXY();

            Polyline[] polilinee = Rhino.Geometry.Intersect.Intersection.MeshPlane(M, planes);

            List<Curve> polilineeList = new List<Curve>();
            foreach (Polyline cr in polilinee)
                polilineeList.Add(cr.ToPolylineCurve());

            Brep[] superfici = Rhino.Geometry.Brep.CreatePlanarBreps(polilineeList, T);

            // BBox
            BoundingBox bbox = M.GetBoundingBox(false);

            List<Point3d> pointForProjection = new List<Point3d>();

            // small scaled grid
            foreach (Point3d pt in gridXY)
                if (pt.X >= bbox.Min.X - this.DimX && pt.X <= bbox.Max.X + this.DimX)
                    if (pt.Y >= bbox.Min.Y - this.DimY && pt.Y <= bbox.Max.Y + this.DimY)
                        pointForProjection.Add(new Point3d(pt.X, pt.Y, 0));

            // from array to list
            List<Brep> volume = new List<Brep>(superfici);

            Point3d[] voxelPoints = Rhino.Geometry.Intersect.Intersection.ProjectPointsToBreps(volume, pointForProjection, Vector3d.ZAxis, T);

            return voxelPoints;
        }


        public int[,,] VoxMatrixBuilding(Point3d[] buildingPoints, Point3d[] terrainPoints, int index, ref string buildingFlagAndNr)
        {
            // bulk rect array
            int[,,] grid3d = new int[NumX + 1, NumY + 1, ZNumbers.Length];

            foreach (Point3d pt in buildingPoints)
            {
                int valX = (int)Math.Round(((pt.X - this.MinX) / this.DimX), 0);
                int valY = (int)Math.Round(((pt.Y - this.MinY) / this.DimY), 0);

                if (!terrainPoints.Contains(pt))
                {

                    int valZ = Array.IndexOf(CheckZ, pt.Z.ToString(Approximation));
                    grid3d[valX, valY, valZ] = index;
                    buildingFlagAndNr += String.Format("{0},{1},{2},{3},{4}\n", valX, valY, valZ, 1, index);

                }
            }

            return grid3d;
        }


        public int[,,] VoxMatrixTerrain(Point3d[] terrainPoints, ref string terrainflag)
        {
            // bulk rect array
            int[,,] grid3d = new int[NumX + 1, NumY + 1, ZNumbers.Length];
            int index = 1;

            foreach (Point3d pt in terrainPoints)
            {
                int valX = (int)Math.Round(((pt.X - this.MinX) / this.DimX), 0);
                int valY = (int)Math.Round(((pt.Y - this.MinY) / this.DimY), 0);

                int valZ = Array.IndexOf(CheckZ, pt.Z.ToString(Approximation));
                terrainflag += String.Format("{0},{1},{2},{3}\n", valX, valY, valZ, TerrainDefaultString);
                grid3d[valX, valY, valZ] = index;

            }

            return grid3d;
        }


        public int[,] BasePoints2d(Brep Geo, int index, double T)
        {

            int[,] grid2d = new int[NumX + 1, NumY + 1];

            for (int i = 0; i < NumX + 1; i++)
                for (int j = 0; j < NumY + 1; j++)
                {
                    Point3d point = new Point3d((i * this.DimX) + this.MinX, (j * this.DimY) + this.MinY, 0);
                    Line ln = new Line(point, Rhino.Geometry.Vector3d.ZAxis, this.DimX * 2);

                    // projection
                    Transform projection = Rhino.Geometry.Transform.PlanarProjection(Rhino.Geometry.Plane.WorldXY);
                    Geo.Transform(projection);

                    Point3d[] intersection_points;
                    Curve[] overlap_curves;

                    if (Rhino.Geometry.Intersect.Intersection.CurveBrep(ln.ToNurbsCurve(), Geo, T, out overlap_curves, out intersection_points))
                        if (overlap_curves.Length > 0 || intersection_points.Length > 0)
                        {
                            grid2d[i, j] = index;
                        }
                        else
                        {
                            grid2d[i, j] = 0;
                        }
                }
            return grid2d;
        }

        public int[,] mergeMatrix2d(List<int[,]> arrayList)
        {
            int[,] integerMatrix = new int[NumX + 1, NumY + 1];

            for (int ii = 0; ii < arrayList.Count; ii++)
            {
                int[,] list = arrayList[ii];
                for (int i = 0; i < NumX + 1; i++)
                {
                    for (int j = 0; j < NumY + 1; j++)
                    {
                        // Add cell togheter and overlap workaround, last arrived win ;P
                        integerMatrix[i, j] += list[i, j];
                        if (integerMatrix[i, j] >= arrayList.Count)
                        {
                            integerMatrix[i, j] = list[i, j];
                        }
                    }
                }
            }

            return integerMatrix;
        }

        public int[,,] mergeMatrix(List<int[,,]> arrayList)
        {
            int[,,] integerMatrix = new int[NumX + 1, NumY + 1, ZNumbers.Length];

            for (int ii = 0; ii < arrayList.Count; ii++)
            {
                int[,,] list = arrayList[ii];
                for (int i = 0; i < NumX + 1; i++)
                    for (int j = 0; j < NumY + 1; j++)
                        for (int k = 0; k < ZNumbers.Length; k++)
                        {
                            // Add cell togheter and overlap workaround
                            integerMatrix[i, j, k] += list[i, j, k];
                            if (integerMatrix[i, j, k] >= arrayList.Count + 1)
                                integerMatrix[i, j, k] = 0;
                        }
            };

            return integerMatrix;
        }

        public static string View2dMatrix(int[,] Data)
        {
            // flip and traspose
            string visualMatrix = string.Empty;

            for (int j = NumY; j >= 0; j--)
            {
                string[] line = new string[NumX + 1];
                for (int i = 0; i < NumX + 1; i++)
                {
                    line[i] = Data[i, j].ToString();
                }
                string row = String.Join(",", line);
                row += "\n";
                visualMatrix += row;
            }
            return visualMatrix;
        }

        public static string EmptyMatrix(string element)
        {
            // flip and traspose
            string visualMatrix = string.Empty;

            for (int j = NumY; j >= 0; j--)
            {
                string[] line = new string[NumX + 1];
                for (int i = 0; i < NumX + 1; i++)
                {
                    line[i] = "";
                }
                string row = String.Join(",", line);
                row += "\n";
                visualMatrix += row;
            }
            return visualMatrix;
        }

    }

    /**********************************************************
      ENVI_MET Buildings
    ***********************************************************/

    public class BuildingMatrix : AutoGrid
    {

        private const int MaxLimit = 99999999;
        private const int MinLimit = 0;

        private List<string> wallMaterial = new List<string>();
        private List<string> roofMaterial = new List<string>();
        public readonly List<string> greenWallMaterial = new List<string>();
        public readonly List<string> greenRoofMaterial = new List<string>();

        public string CommonWallMaterial { get; set; }
        public string CommonRoofMaterial { get; set; }
        // GREEN material
        private const string greenNullMaterial = " ";
        public readonly List<string> selectedWallMaterialGreenings = new List<string>();
        public readonly List<string> selectedRoofMaterialGreenings = new List<string>();

        public List<Mesh> Buildings { get; set; }
        public List<int> GreenIdBuildings { get; set; }

        public delegate int[,] BuildingMatrix2d(int[,,] Data);

        public BuildingMatrix(List<Mesh> buildings, List<string> wall, List<string> roof, string commonWallMaterial, string commonRoofMaterial, List<int> greenBuildingsId, List<string> greenWall, List<string> greenRoof)
        {
            this.Buildings = buildings;
            this.GreenIdBuildings = greenBuildingsId;

            this.CommonWallMaterial = commonWallMaterial;
            this.CommonRoofMaterial = commonRoofMaterial;

            // Building normal materials
            if (buildings.Count == wall.Count && buildings.Count == roof.Count)
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    this.wallMaterial.Add(wall[i]);
                    this.roofMaterial.Add(roof[i]);
                }
            }
            else if (roof.Count == 1 && buildings.Count == wall.Count)
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    this.wallMaterial.Add(wall[i]);
                    this.roofMaterial.Add(this.CommonRoofMaterial);
                }
            }
            else if (wall.Count == 1 && buildings.Count == roof.Count)
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    this.wallMaterial.Add(this.CommonWallMaterial);
                    this.roofMaterial.Add(roof[i]);
                }
            }
            else
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    this.wallMaterial.Add(this.CommonWallMaterial);
                    this.roofMaterial.Add(this.CommonRoofMaterial);
                }
            }

            // Green materials
            if (greenBuildingsId.Count != 0)
            {
                // normal material for buildings
                foreach (int index in greenBuildingsId)
                {
                    this.selectedWallMaterialGreenings.Add(this.wallMaterial[index]);
                    this.selectedRoofMaterialGreenings.Add(this.roofMaterial[index]);
                }

                if (greenBuildingsId.Count == greenWall.Count && greenBuildingsId.Count == greenRoof.Count)
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenWall[i]);
                        this.greenRoofMaterial.Add(greenRoof[i]);
                    }
                else if (greenRoof.Count == 0 && greenBuildingsId.Count == greenWall.Count)
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenWall[i]);
                        this.greenRoofMaterial.Add(greenNullMaterial);
                    }
                }
                else if (greenWall.Count == 0 && greenBuildingsId.Count == greenRoof.Count)
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenNullMaterial);
                        this.greenRoofMaterial.Add(greenRoof[i]);
                    }
                }
                else if (greenWall.Count == 0 && greenRoof.Count == 1)
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenNullMaterial);
                        this.greenRoofMaterial.Add(greenRoof[0]);
                    }
                }
                else if (greenRoof.Count == 0 && greenWall.Count == 1)
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenWall[0]);
                        this.greenRoofMaterial.Add(greenNullMaterial);
                    }
                }
                else if (greenWall.Count == 1 && greenRoof.Count == 1)
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenWall[0]);
                        this.greenRoofMaterial.Add(greenRoof[0]);
                    }
                }
                else
                {
                    for (int i = 0; i < greenBuildingsId.Count; i++)
                    {
                        this.greenWallMaterial.Add(greenNullMaterial);
                        this.greenRoofMaterial.Add(greenNullMaterial);
                    }
                }
            }
        }

        private string SparseMatrix(int[,,] pts, List<string> w, List<string> r)
        {
            string matrice = string.Empty;
            for (int i = 0; i < NumX + 1; i++)
                for (int j = 0; j < NumY + 1; j++)
                    for (int k = 0; k < ZNumbers.Length; k++)
                    {
                        string line = string.Empty;
                        int index = 0;
                        int indexW = 0;
                        int indexR = 0;
                        if (pts[i, j, k] != 0)
                        {
                            index = pts[i, j, k] - 1;
                            try
                            {
                                if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], w[index], r[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], "", r[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[index], r[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], w[index], "");
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], "", "");
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", r[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[index], "");
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], w[index], r[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], "", r[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[index], r[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", r[index]);
                                }
                            }
                        }
                        else
                        {
                            if (i != 0 && j != 0)
                                try
                                {
                                    if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] == 0)
                                    {
                                        index = pts[i - 1, j, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], "", "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[index], "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                    {
                                        index = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", r[index]);
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], w[index], "");
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i - 1, j, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[indexW], "", r[indexR]);
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i, j - 1, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[indexW], r[indexR]);
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i, j - 1, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[indexW], w[indexW], r[indexR]);
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && k == 0)
                                    {
                                        index = pts[i - 1, j, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], "", "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && k == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", w[index], "");
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && k == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, w[index], w[index], "");
                                    }
                                }
                        }
                        matrice += line;
                    }
            return matrice;
        }

        public string NormalSparseMatrix(int[,,] pts)
        {
            string matrice = this.SparseMatrix(pts, this.wallMaterial, this.roofMaterial);
            return matrice;
        }

        public string GreenSparseMatrix(int[,,] pts)
        {
            string matrice = this.SparseMatrix(pts, this.greenWallMaterial, this.greenRoofMaterial);
            return matrice;
        }

        // Bulding matrix implementation
        public int[,] BuildingId2d(int[,,] Data)
        {

            int[,] grid2d = new int[NumX + 1, NumY + 1];

            for (int i = 0; i < NumX + 1; i++)
            {
                int[] id = new int[ZNumbers.Length];
                for (int j = 0; j < NumY + 1; j++)
                {
                    for (int k = 0; k < ZNumbers.Length; k++)
                    {
                        id[k] = Data[i, j, k];
                    }
                    grid2d[i, j] = id.Max();
                }
            }

            return grid2d;
        }

        public int[,] BuildingBottom2d(int[,,] Data)
        {

            int[,] grid2d = new int[NumX + 1, NumY + 1];

            for (int i = 0; i < NumX + 1; i++)
            {
                int[] bottom = new int[ZNumbers.Length];
                for (int j = 0; j < NumY + 1; j++)
                {
                    for (int k = 0; k < ZNumbers.Length; k++)
                    {
                        bottom[k] = (Data[i, j, k] != 0) ? k : MaxLimit;
                    }
                    int min = bottom.Min();
                    grid2d[i, j] = (min != MaxLimit) ? (int)Math.Round(ZNumbers[min], MinLimit) : MinLimit;
                }
            }

            return grid2d;
        }

        public int[,] BuildingTop2d(int[,,] Data)
        {

            int[,] grid2d = new int[NumX + 1, NumY + 1];


            for (int i = 0; i < NumX + 1; i++)
            {
                int[] up = new int[ZNumbers.Length];
                for (int j = 0; j < NumY + 1; j++)
                {
                    for (int k = 0; k < ZNumbers.Length; k++)
                    {
                        up[k] = (Data[i, j, k] != 0) ? (int)Math.Round(ZNumbers[k], MinLimit) : MinLimit;
                    }
                    int max = up.Max();
                    grid2d[i, j] = max;
                }
            }

            return grid2d;
        }


        public static Mesh MoveBuildingsUp(Mesh mesh, Mesh terrain)
        {

            Transform xmoveTerrain;
            Transform xmoveCentroid;

            const int UnitBox = 1;
            const double noIntersection = 0.0;

            try
            {

                // first traslation
                Point3d center = AreaMassProperties.Compute(mesh).Centroid;

                Ray3d r = new Ray3d(center, Vector3d.ZAxis);

                var intersec = Rhino.Geometry.Intersect.Intersection.MeshRay(terrain, r);

                Point3d pt = r.PointAt(intersec);

                if (intersec != noIntersection)
                {
                    Vector3d vecCentroid = new Vector3d(0, 0, pt.Z - center.Z);
                    xmoveCentroid = Transform.Translation(vecCentroid);
                    mesh.Transform(xmoveCentroid);
                }


                // move to terrain
                BoundingBox BBox = mesh.GetBoundingBox(true);
                Mesh meshBox = Mesh.CreateFromBox(BBox, UnitBox, UnitBox, UnitBox);

                Line[] lines = Rhino.Geometry.Intersect.Intersection.MeshMeshFast(terrain, mesh);

                Point3d minBBox = BBox.Min;

                // dimension
                double start = minBBox.Z;
                double end = lines.Min(l => l.From.Z);


                Vector3d vecTerrain = new Vector3d(0, 0, end - start);
                xmoveTerrain = Transform.Translation(vecTerrain);
            }
            catch
            {
                xmoveTerrain = Transform.Translation(Vector3d.Zero);
            }

            mesh.Transform(xmoveTerrain);


            return mesh;
        }
    }

    /**********************************************************
      ENVI_MET DEM
    ***********************************************************/
    public class Dem : AutoGrid
    {
        public Mesh TerrainMesh { get; set; }

        private const int ZeroValue = 0;

        public static Mesh CreateClosedMeshTerrain(Mesh surfTerrainMesh)
        {
            // bulk mesh
            Mesh finalMesh = new Mesh();

            // borders
            Polyline[] arrayCrv1 = surfTerrainMesh.GetOutlines(Rhino.Geometry.Plane.WorldXY);
            Polyline[] arrayCrv2 = surfTerrainMesh.GetNakedEdges();

            Mesh baseMesh = Rhino.Geometry.Mesh.CreateFromClosedPolyline(arrayCrv1[0]);

            // casting
            Curve crv1 = arrayCrv1[0].ToNurbsCurve();
            Curve crv2 = arrayCrv2[0].ToNurbsCurve();

            // direction check
            if (!Curve.DoDirectionsMatch(crv1, crv2))
            {
                crv1.Reverse();
            }

            // reset seam
            double param;
            crv1.ClosestPoint(crv2.PointAtStart, out param);
            crv1.ChangeClosedCurveSeam(param);

            Brep sideGeo = Brep.CreateFromLoft(new List<Curve>() { crv1, crv2 }, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];

            var default_mesh_params = MeshingParameters.QualityRenderMesh;
            var sideGeoMesh = Mesh.CreateFromBrep(sideGeo, default_mesh_params)[0];

            finalMesh.Append(sideGeoMesh);
            finalMesh.Append(surfTerrainMesh);
            finalMesh.Append(baseMesh);

            return finalMesh;
        }


        public int[,] DemTop2d(int[,,] Data)
        {

            int[,] grid2d = new int[NumX + 1, NumY + 1];


            for (int i = 0; i < NumX + 1; i++)
            {
                int[] up = new int[ZNumbers.Length];
                for (int j = 0; j < NumY + 1; j++)
                {
                    for (int k = 0; k < ZNumbers.Length; k++)
                    {
                        up[k] = (Data[i, j, k] != ZeroValue) ? (int)Math.Round(ZNumbers[k], ZeroValue) : ZeroValue;
                    }
                    int max = up.Max();
                    grid2d[i, j] = max;
                }
            }

            return grid2d;
        }

    }



    /**********************************************************
        ENVI_MET 2d elements
    ***********************************************************/

    public class Element2dMatrix : AutoGrid
    {

        // variabili di istanza
        protected List<string> customMaterial = new List<string>();
        public List<Brep> Geometries { get; set; }
        public string DefaultMaterial { get; set; }

        public Element2dMatrix(string defaultMaterial, List<string> customMaterialList, List<Brep> geometries)
        {
            this.DefaultMaterial = defaultMaterial;
            this.Geometries = geometries;

            if (geometries.Count == customMaterialList.Count)
            {
                for (int i = 0; i < geometries.Count; i++)
                {
                    this.customMaterial.Add(customMaterialList[i]); // custom materials
                }
            }
            else if (geometries.Count != customMaterialList.Count && customMaterialList.Count == 1)
            {
                for (int i = 0; i < geometries.Count; i++)
                {
                    this.customMaterial.Add(customMaterialList[0]); // only one custom material ALL
                }
            }
            else
            {
                for (int i = 0; i < geometries.Count; i++)
                {
                    this.customMaterial.Add(this.DefaultMaterial); // default material ALL
                }
            }
        }


        public string MatrixWithMaterials(int[,] Data)
        {
            // flip and traspose
            string visualMatrix = string.Empty;

            for (int j = NumY; j >= 0; j--)
            {
                string[] line = new string[NumX + 1];
                for (int i = 0; i < NumX + 1; i++)
                {
                    string valore = string.Empty;
                    if (Data[i, j] != 0)
                    {
                        valore = this.customMaterial[Data[i, j] - 1];
                    }
                    else
                    {
                        valore = this.DefaultMaterial;
                    }
                    line[i] = valore;
                }
                string row = String.Join(",", line);
                row += "\n";
                visualMatrix += row;
            }
            return visualMatrix;
        }
    }

    public class ThreeDimensionalPlants : Element2dMatrix
    {

        public List<Brep> Threes { get; set; }

        public ThreeDimensionalPlants(string defaultMaterial, List<string> customMaterialList, List<Brep> geometries) : base(defaultMaterial, customMaterialList, geometries)
        {
            this.Threes = geometries;
        }

        public List<string[]> GenerateLists(int[,] Data)
        {
            List<string[]> propertiesTree = new List<string[]>();

            for (int i = 0; i < NumX + 1; i++)
            {
                for (int j = NumY; j > 0; j--)
                {
                    if (Data[i, j] != 0)
                    {
                        string[] idAndDescription = this.customMaterial[Data[i, j] - 1].Split(',');
                        propertiesTree.Add(new string[] { (i + 1).ToString(), (j + 1).ToString(), "0", idAndDescription[0], idAndDescription[1], "0" });
                    }
                }
            }
            return propertiesTree;
        }

    }

    public class NestingGrid
    {

        public int NumNestingGrid { get; set; }
        public string SoilProfileA { get; set; }
        public string SoilProfileB { get; set; }
        public string Name { get; set; }

        public NestingGrid()
        {
            this.NumNestingGrid = 3;
            this.SoilProfileA = "0000LO";
            this.SoilProfileB = "0000LO";
            this.Name = "NestingGrid";
        }
    }


    /**********************************************************
          ENVI_MET 2d Simple Wall 
      ***********************************************************/

    public class SimpleWall : Element2dMatrix
    {

        // They are managed as 2d z element for now
        public double[] MinValueZ { get; private set; }

        public SimpleWall(string defMat, List<string> customMat, List<Brep> geometry) : base(defMat, customMat, geometry)
        {
            this.MinValueZ = GenerateMinValueZ(geometry);
        }

        protected double[] GenerateMinValueZ(List<Brep> geometry)
        {
            double[] zMinValueGeo = new double[geometry.Count];

            for (int i = 0; i < geometry.Count; i++)
            {
                // get bbox
                BoundingBox bboxSrf = geometry[i].GetBoundingBox(true);

                // take min point
                Point3d minPoint = bboxSrf.Min;
                double zValueGeo = minPoint.Z;

                zMinValueGeo[i] = DiffArrayZdir(zValueGeo);

            }
            return zMinValueGeo;
        }


        protected int DiffArrayZdir(double num)
        {
            double[] newArr = new double[ZNumbers.Length];

            for (int i = 0; i < newArr.Length; i++)
            {
                newArr[i] = Math.Abs(num - ZNumbers[i]);
            }

            int minIndex = Array.IndexOf(newArr, newArr.Min());

            return minIndex;
        }


        public void SimpleWallStringCalcZdir(Brep Geo, int index, double T, envimetGeometry.AutoGrid grid, ref string contentXml)
        {

            for (int i = 0; i < NumX + 1; i++)
                for (int j = 0; j < NumY + 1; j++)
                {
                    Point3d point = new Point3d((i * grid.DimX) + grid.MinX, (j * grid.DimY) + grid.MinY, 0);
                    Line ln = new Line(point, Vector3d.ZAxis, this.DimX * 2);

                    // projection
                    Transform projection = Transform.PlanarProjection(Plane.WorldXY);
                    Geo.Transform(projection);

                    Point3d[] intersection_points;
                    Curve[] overlap_curves;

                    if (Rhino.Geometry.Intersect.Intersection.CurveBrep(ln.ToNurbsCurve(), Geo, T, out overlap_curves, out intersection_points))
                        if (overlap_curves.Length > 0 || intersection_points.Length > 0)
                        {
                            contentXml += String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, this.MinValueZ[index], "", "", this.customMaterial[index]);
                        }
                }
        }

    }

}


