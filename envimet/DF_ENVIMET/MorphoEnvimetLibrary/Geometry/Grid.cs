using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;


namespace MorphoEnvimetLibrary.Geometry
{
    public class Grid
    {
        public const int MAX_NUM_Z = 999;
        public const int MIN_NUM_BORDER_CELLS = 2;

        private const int CENTROID = 1;
        private const long SPACE_LIMIT = 100000000L;
        public const string APPROXIMATION = "0.00";
        public const int FIRST_CELL_COMBINED_GRID = 4;
        private double[] _height;

        // input from other assembly
        public Mesh Surface { get; set; }
        public double DimX { get; set; }
        public double DimY { get; set; }
        public double DimZ { get; set; }
        public int ExtLeftXgrid { get; set; }
        public int ExtRightXgrid { get; set; }
        public int ExtUpYgrid { get; set; }
        public int ExtDownYgrid { get; set; }
        public double Telescope { get; set; }
        public double StartTelescopeHeight { get; set; }
        public bool CombineGridType { get; set; }


        public double[] Height {
            get { return _height; }
        }
        public int NumX { get; private set; }
        public int NumY { get; private set; }
        public int NumZ { get; set; }

        public double MinX { get; private set; }
        public double MinY { get; private set; }
        private double MaxX { get; set; }
        private double MaxY { get; set; }
        
        public Grid()
        {
            NumZ = 15;
            DimX = 3.0;
            DimY = 3.0;
            DimZ = 3.0;
            StartTelescopeHeight = 5.0;
            ExtLeftXgrid = MIN_NUM_BORDER_CELLS;
            ExtRightXgrid = MIN_NUM_BORDER_CELLS;
            ExtUpYgrid = MIN_NUM_BORDER_CELLS;
            ExtDownYgrid = MIN_NUM_BORDER_CELLS;
            Telescope = 0.0;
            NumX = 0;
            NumY = 0;
            MinX = 0.0;
            MinY = 0.0;
            MaxX = 0.0;
            MaxY = 0.0;

            // add cell if equidistant
            IfTelescope();
            _height = new double[NumZ];

            ValidateAdditionalCells();
        }

        // private methods
        private void IfTelescope()
        {
            if (Telescope == 0.0)
                NumZ += 4;
        }

        private void ValidateAdditionalCells()
        {
            if (ExtLeftXgrid < MIN_NUM_BORDER_CELLS)
                ExtLeftXgrid = MIN_NUM_BORDER_CELLS;
            if (ExtRightXgrid < MIN_NUM_BORDER_CELLS)
                ExtRightXgrid = MIN_NUM_BORDER_CELLS;
            if (ExtUpYgrid < MIN_NUM_BORDER_CELLS)
                ExtUpYgrid = MIN_NUM_BORDER_CELLS;
            if (ExtDownYgrid < MIN_NUM_BORDER_CELLS)
                ExtDownYgrid = MIN_NUM_BORDER_CELLS;
        }

        public double CastingPrecision(double value)
        {
            List<string> values = _height.ToList<double>().Select(v => v.ToString(APPROXIMATION))
                                .ToList();
            return values.IndexOf(value.ToString(APPROXIMATION));
        }

        // init
        public void CalcGridXY(List<Mesh> geometries)
        {
            double distLeft = ExtLeftXgrid * DimX;
            double distRight = ExtRightXgrid * DimX;
            double distUp = ExtUpYgrid * DimY;
            double distDown = ExtDownYgrid * DimY;

            MinX = MinY = SPACE_LIMIT;
            MaxX = MaxY = -1 * SPACE_LIMIT;
            double maxZ = -1 * SPACE_LIMIT;

            foreach (Mesh geo in geometries)
            {
                BoundingBox BB1 = geo.GetBoundingBox(true);
                if (MinX > BB1.Min.X)
                    MinX = BB1.Min.X;
                if (MaxX < BB1.Max.X)
                    MaxX = BB1.Max.X;
                if (MinY > BB1.Min.Y)
                    MinY = BB1.Min.Y;
                if (MaxY < BB1.Max.Y)
                    MaxY = BB1.Max.Y;
                if (maxZ < BB1.Max.Z)
                    maxZ = BB1.Max.Z;
            }

            // Geometry BoundingBox limits NETO
            MinX = MinX - distLeft;
            MinY = MinY - distDown;
            MaxX = MaxX + distRight;
            MaxY = MaxY + distUp;

            // Required height -- Twice the heighest building
            double reqHeight = maxZ * MIN_NUM_BORDER_CELLS;

            double domX = MaxX - MinX;
            double domY = MaxY - MinY;

            // Calculate NumX, NumY and shift by one. Work with centroid instead of VOIDS
            NumX = (int)Math.Floor((domX / DimX)) + CENTROID;
            NumY = (int)Math.Floor((domY / DimY)) + CENTROID;

            Rhino.RhinoApp.WriteLine("Dimensione griglia: {0}, {1}", NumX, NumY);

            // Reccalculate maxX/Y just for the bounding box fit the grid size/length
            MaxX = MinX + (NumX * DimX);
            MaxY = MinY + (NumY * DimY);

        }

        public void CalcGzDimension()
        {

            // Calculate z info
            // Preparation
            double[] gZ = new double[NumZ];
            double dimZ = this.DimZ;
            double firstGrid = dimZ / 5;
            double grid = 0;

            // Calculate
            for (int i = 1; i < NumZ + 1; i++)
            {
                if (Telescope == 0.0)
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
                    if (i == 1 || grid <= StartTelescopeHeight)
                    {
                        grid = (i * dimZ) - (dimZ / 2);
                    }
                    else
                    {
                        double gz = dimZ;
                        dimZ = dimZ + (dimZ * (double)Telescope / 100);
                        grid = grid + (dimZ + gz) / 2;
                    }
                    gZ[i - 1] = grid;
                }
            }

            SetGridZcells(gZ);
        }

        private void SetGridZcells(double[] gZ)
        {
            List<double> tempHeight = new List<double>();

            if (CombineGridType && Telescope != 0.0)
            {
                double delta = (gZ[1] - gZ[0]) / FIRST_CELL_COMBINED_GRID;
                double adjust = gZ[0] - delta;

                gZ.ToList().ForEach(el =>
                {
                    if (el > gZ.First())
                    {
                        tempHeight.Add(el);
                    }
                    else
                    {
                        double firstNumber = gZ.First();
                        tempHeight.Add(firstNumber);

                        for (int j = 1; j < 4; j++)
                        {
                            firstNumber += delta;
                            tempHeight.Add(firstNumber);
                        }
                    }
                });

                // move centroids to ground
                _height = tempHeight.Select(el => el - (adjust + (adjust / 2))).ToArray();
            }
            else
            {
                tempHeight.AddRange(gZ);
                _height = tempHeight.ToArray();
            }
        }

        private Point3d PointXZ(int ix, int iz)
        {
            return new Point3d((ix * this.DimX) + this.MinX, this.MaxY, _height[iz]);
        }

        private Point3d PointYZ(int iy, int iz)
        {
            return new Point3d(this.MaxX, (iy * this.DimY) + this.MinY, _height[iz]);
        }

        private Point3d PointXY(int ix, int iy)
        {
            return new Point3d((ix * this.DimX) + this.MinX, (iy * this.DimY) + this.MinY, _height[0]);
        }

        private int GetNumberOfZcellsByCombinedGrid()
        {
            return (CombineGridType && Telescope != 0.0) ? NumZ + (FIRST_CELL_COMBINED_GRID - 1) : NumZ;
        }

        // public methods
        public List<Point3d> GridXY()
        {
            List<Point3d> gridPoints = new List<Point3d>();
            // XY Grid
            for (int ix = 0; ix < NumX ; ix++)
                for (int iy = 0; iy < NumY; iy++)
                    gridPoints.Add(PointXY(ix, iy));
            return gridPoints;
        }

        public List<Point3d> GridXZ()
        {
            int zCount = GetNumberOfZcellsByCombinedGrid();
            List<Point3d> gridPoints = new List<Point3d>();
            // XZ Grid
            for (int ix = 0; ix < NumX; ix++)
                for (int iz = 0; iz < zCount; iz++)
                    gridPoints.Add(PointXZ(ix, iz));

            return gridPoints;
        }

        public List<Point3d> GridYZ()
        {
            int zCount = GetNumberOfZcellsByCombinedGrid();
            List<Point3d> gridPoints = new List<Point3d>();
            // YZ Grid
            for (int iy = 0; iy < NumY; iy++)
                for (int iz = 0; iz < zCount; iz++)
                    gridPoints.Add(PointYZ(iy, iz));

            return gridPoints;
        }

        public static string CreateTextMatrix(Matrix2d matrix, string element = null)
        {
            // flip and traspose
            string text = string.Empty;

            for (int j = matrix.GetLengthY() - 1; j >= 0; j--)
            {
                string[] line = new string[matrix.GetLengthX()];
                for (int i = 0; i < matrix.GetLengthX(); i++)
                {
                    line[i] = (element == null) ? matrix[i, j].ToString() : element;
                }
                string row = String.Join(",", line);
                row += "\n";
                text += row;
            }
            return text;
        }

        public Matrix3d CreateBase3dMatrix()
        {
            return new Matrix3d(NumX, NumY, NumZ);
        }

        public Matrix2d CreateBase2dMatrix()
        {
            return new Matrix2d(NumX, NumY);
        }
    }


    public class NestingGrid
    {
        public int NumNestingGrid { get; set; }
        public string SoilProfileA { get; set; }
        public string SoilProfileB { get; set; }

        public NestingGrid()
        {
            NumNestingGrid = 3;
            SoilProfileA = "0000LO";
            SoilProfileB = "0000LO";
        }
    }        

}
