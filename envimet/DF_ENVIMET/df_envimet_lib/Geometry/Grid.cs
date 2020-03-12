using System;
using System.Collections.Generic;
using System.Linq;
using df_envimet_lib.Utility;
using Rhino.Geometry;


namespace df_envimet_lib.Geometry
{
    public class Grid
    {
        public const int MAX_NUM_Z = 999;
        public const int MIN_NUM_BORDER_CELLS = 2;
        private const int CENTROID = 1;
        private const long SPACE_LIMIT = 100000000L;
        public const string APPROXIMATION = "0.00";
        public const int FIRST_CELL_COMBINED_GRID = 4;

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

        public double[] Height { get; private set; }
        public double[] Sequence { get; private set; }
        public double[] Accumulated { get; private set; }

        public int NumX { get; set; }
        public int NumY { get; set; }
        public int NumZ { get; set; }

        public double MinX { get; private set; }
        public double MinY { get; private set; }
        private double MaxX { get; set; }
        private double MaxY { get; set; }
        
        public Grid()
        {
            NumZ = 15;
            DimX = DimY = DimZ = 3.0;
            StartTelescopeHeight = 5.0;
            ExtLeftXgrid = ExtRightXgrid = ExtUpYgrid = ExtDownYgrid = MIN_NUM_BORDER_CELLS;
            NumX = NumY = 0;
            MinX = MinY = MaxX = MaxY = 0;
            CombineGridType = false;
        }

        public void CalculateHeight()
        {
            if (CombineGridType && Telescope > 0.0)
                Sequence = GetTelescopeEqSeqZ(Telescope, StartTelescopeHeight);
            else if (CombineGridType == false && Telescope > 0.0)
                Sequence = GetTelescopeSeqZ(Telescope, StartTelescopeHeight);
            else
                Sequence = GetEquidistantSeqZ();

            Accumulated = Util.Accumulate(Sequence)
                .ToArray();
            Height = Accumulated.Zip(Sequence, (a, b) => a - (b / 2))
                .ToArray();
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
            List<string> values = Height.ToList<double>().Select(v => v.ToString(APPROXIMATION))
                                .ToList();
            return values.IndexOf(value.ToString(APPROXIMATION));
        }

        public void CalcGridXY(List<Mesh> geometries)
        {
            ValidateAdditionalCells();

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

            MinX = MinX - distLeft;
            MinY = MinY - distDown;
            MaxX = MaxX + distRight;
            MaxY = MaxY + distUp;

            double reqHeight = maxZ * MIN_NUM_BORDER_CELLS;

            double domX = MaxX - MinX;
            double domY = MaxY - MinY;

            NumX = (int)Math.Floor((domX / DimX)) + CENTROID;
            NumY = (int)Math.Floor((domY / DimY)) + CENTROID;

            MaxX = MinX + (NumX * DimX);
            MaxY = MinY + (NumY * DimY);
        }

        #region Sequence
        private double[] GetEquidistantSeqZ()
        {
            var baseCell = DimZ / 5;
            var cell = DimZ;

            double[] sequence = new double[NumZ];
        
            for (int k = 0; k < sequence.Length; k++)
            {
                if (k < 5)
                    sequence[k] = baseCell;
                else
                    sequence[k] = cell;
            }

            return sequence;
        }

        private double[] GetTelescopeSeqZ(double telescope, double start)
        {
            var cell = DimZ;

            double[] sequence = new double[NumZ];

            double val = cell;

            for (int k = 0; k < sequence.Length; k++)
            {
                if (val * k < start)
                {
                    sequence[k] = cell;
                }
                else
                {
                    sequence[k] = val + (val * telescope / 100);
                    val = sequence[k];
                }
            }

            return sequence;
        }

        private double[] GetTelescopeEqSeqZ(double telescope, double start)
        {
            var cell = DimZ;
            var baseCell = DimZ / 5;
            double val = cell;

            double[] firstSequence = new double[5];
            double[] sequence = new double[NumZ - 1];

            for (int k = 0; k < 5; k++)
                firstSequence[k] = baseCell;

            for (int k = 0; k < sequence.Length; k++)
            {
                if (val * (k + 1) < start)
                {
                    sequence[k] = cell;
                }
                else
                {
                    sequence[k] = val + (val * telescope / 100);
                    val = sequence[k];
                }
            }

            double[] completeSequence = new double[sequence.Length + firstSequence.Length];

            firstSequence.CopyTo(completeSequence, 0);
            sequence.CopyTo(completeSequence, firstSequence.Length);
            Array.Resize(ref completeSequence, sequence.Length + 1);

            return completeSequence;
        }
        #endregion

        #region Grid Point
        private Point3d PointXZ(int ix, int iz)
        {
            return new Point3d((ix * this.DimX) + this.MinX, this.MaxY, Height[iz]);
        }

        private Point3d PointYZ(int iy, int iz)
        {
            return new Point3d(this.MaxX, (iy * this.DimY) + this.MinY, Height[iz]);
        }

        private Point3d PointXY(int ix, int iy)
        {
            return new Point3d((ix * this.DimX) + this.MinX, (iy * this.DimY) + this.MinY, Height[0]);
        }

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
            List<Point3d> gridPoints = new List<Point3d>();
            // XZ Grid
            for (int ix = 0; ix < NumX; ix++)
                for (int iz = 0; iz < NumZ; iz++)
                    gridPoints.Add(PointXZ(ix, iz));

            return gridPoints;
        }

        public List<Point3d> GridYZ()
        {
            List<Point3d> gridPoints = new List<Point3d>();
            // YZ Grid
            for (int iy = 0; iy < NumY; iy++)
                for (int iz = 0; iz < NumZ; iz++)
                    gridPoints.Add(PointYZ(iy, iz));

            return gridPoints;
        }
        #endregion

        public static string CreateTextMatrix(Matrix2d matrix, string element = null)
        {
            // flip and traspose
            string text = string.Empty;
            List<string> rows = new List<string>();

            for (int j = matrix.GetLengthY() - 1; j >= 0; j--)
            {
                string[] line = new string[matrix.GetLengthX()];
                for (int i = 0; i < matrix.GetLengthX(); i++)
                {
                    line[i] = element ?? matrix[i, j].ToString();
                }
                rows.Add(String.Join(",", line));
            }
            text = String.Join("\n", rows) + "\n";

            return text;
        }

        public static List<double> FilterListBasedOnNumber(List<double> values, double number)
        {
            return values.FindAll(e => e < number);
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
