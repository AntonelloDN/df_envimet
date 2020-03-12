using df_envimet_lib.Geometry;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace df_envimet_lib.IO
{

    #region FaceDirection Enum
    public enum FaceDirection
    {
        X,
        Y,
        Z
    }
    #endregion

    #region CellMaterial Struct
    public struct CellMaterial
    {
        public string MaterialX { set; get; }
        public string MaterialY { set; get; }
        public string MaterialZ { set; get; }
    }
    #endregion

    #region Pixel Struct
    public struct PixelCoordinate
    {
        public double I { set; get; }
        public double J { set; get; }
        public int K { set; get; }
    }
    #endregion

    public class Facade
    {
        public const string TREE_WRONG_TAG = "3Dplants";
        public const string TEMP_TREE_TAG = "plant3D";
        public const string FACADE_TAG = "ID_wallDB";
        public const int INCREASE_COUNT_BY_FOUR = 4;
        private bool disposed = false;
        private const string WALL_DB = "WallDB";
        private const int SHIFT = 3;

        public string Id { get; set; }
        public Mesh Geometry { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;
        }

        ~Facade()
        {
            Dispose(false);
        }

        public static IEnumerable<string> GetFacadeSparseMatrix(string inx)
        {
            string cleanFile = GetXmlWithoutWrongTag(inx);
            XDocument doc = XDocument.Parse(cleanFile);

            var innerTextData = doc.Root.Element(WALL_DB).Element(FACADE_TAG).Value;

            List<string> rows = innerTextData.Split(new char[] { '\n' }).ToList();
            rows.RemoveAt(0);
            rows.RemoveAt(rows.Count - 1);

            rows = rows.OrderBy(s => Convert.ToInt32(s.Split(',')[(int)FaceDirection.Y])).ToList();
            rows = rows.OrderBy(s => Convert.ToInt32(s.Split(',')[(int)FaceDirection.Z])).ToList();

            return rows;
        }

        public static Dictionary<string, double> GetGridDetail(string inx)
        {
            string cleanFile = GetXmlWithoutWrongTag(inx);

            Dictionary<string, double> modelgeometry = new Dictionary<string, double>();

            XDocument doc = XDocument.Parse(cleanFile);

            double numX = Convert.ToInt32(doc.Root.Element("modelGeometry").Element("grids-I").Value);
            double numY = Convert.ToInt32(doc.Root.Element("modelGeometry").Element("grids-J").Value);
            double numZ = Convert.ToInt32(doc.Root.Element("modelGeometry3D").Element("grids3D-K").Value);

            double dimX = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dx").Value);
            double dimY = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dy").Value);
            double dimZ = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dz-base").Value);

            double telescope = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("useTelescoping_grid").Value);
            double splitting = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("useSplitting").Value);
            double verticalStreatch = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("verticalStretch").Value);
            double startStreatch = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("startStretch").Value);

            modelgeometry["grids-I"] = numX;
            modelgeometry["grids-J"] = numY;
            modelgeometry["grids-Z"] = numZ;
            modelgeometry["dx"] = dimX;
            modelgeometry["dy"] = dimY;
            modelgeometry["dz-base"] = dimZ;
            modelgeometry["useTelescoping_grid"] = telescope;
            modelgeometry["useSplitting"] = splitting;
            modelgeometry["verticalStretch"] = verticalStreatch;
            modelgeometry["startStretch"] = startStreatch;

            return modelgeometry;
        }

        public static IEnumerable<Facade> GenerateFacadeByDirection(List<string> rows, Grid grid, FaceDirection direction, bool bake = false)
        {
            Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;

            List<Facade> result = new List<Facade>();

            for (int i = 0; i < rows.Count; i++)
            {
                Facade face = new Facade();

                string[] row = rows[i].Split(',');
                double x = Convert.ToInt32(row[(int)FaceDirection.X]);
                double y = Convert.ToInt32(row[(int)FaceDirection.Y]);
                int z = Convert.ToInt32(row[(int)FaceDirection.Z]);

                string matX = row[3];
                string matY = row[4];
                string matZ = row[5];

                PixelCoordinate pix = new PixelCoordinate() { I = x, J = y, K = z };

                if (direction == FaceDirection.X && matX.Length > 2)
                {
                    face = GenerateXfacade(grid, pix);
                    result.Add(face);
                }
                else if (direction == FaceDirection.Y && matY.Length > 2)
                {
                    face = GenerateYfacade(grid, pix);
                    result.Add(face);
                }
                else if (direction == FaceDirection.Z && matZ.Length > 2 && z != 0)
                {
                    face = GenerateZfacade(grid, pix);
                    result.Add(face);
                }

            }

            if (bake)
            {
                foreach (Facade f in result)
                {
                    Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes { Name = f.Id };
                    ot.AddMesh(f.Geometry, att);
                }
            }

            return result;
        }

        protected static Facade GenerateXfacade(Grid grid, PixelCoordinate pix)
        {
            var sequence = grid.Sequence;
            var position = grid.Height;

            var point = new Point3d((pix.I * grid.DimX) + grid.MinX, (pix.J * grid.DimY) + grid.MinY, position[pix.K]);
            var plane = new Plane(point, Vector3d.XAxis);
            var dimY = grid.DimY;
            var mesh = Mesh.CreateFromPlane(plane, new Interval(0, dimY), new Interval(-sequence[pix.K] / 2, sequence[pix.K] / 2), 1, 1);

            Facade face = new Facade() { Id = String.Format("X:{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh };

            return face;
        }

        protected static Facade GenerateYfacade(Grid grid, PixelCoordinate pix)
        {
            var sequence = grid.Sequence;
            var position = grid.Height;

            var point = new Point3d((pix.I * grid.DimX) + grid.MinX, (pix.J * grid.DimY) + grid.MinY, position[pix.K]);
            var plane = new Plane(point, Vector3d.YAxis);
            var dimZ = sequence[pix.K] / 2;
            var dimX = grid.DimX;
            var mesh = Mesh.CreateFromPlane(plane, new Interval(-dimZ, dimZ), new Interval(0, dimX), 1, 1);

            Facade face = new Facade() { Id = String.Format("Y:{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh };

            return face;
        }

        protected static Facade GenerateZfacade(Grid grid, PixelCoordinate pix)
        {

            var sequence = grid.Sequence;
            var position = grid.Height;

            var point = new Point3d((pix.I * grid.DimX) + grid.MinX, (pix.J * grid.DimY) + grid.MinY, position[pix.K] - (sequence[pix.K] / 2));
            var plane = new Plane(point, Vector3d.ZAxis);
            var dimX = grid.DimX;
            var dimY = grid.DimY;
            var mesh = Mesh.CreateFromPlane(plane, new Interval(0, dimX), new Interval(0, dimY), 1, 1);

            Facade face = new Facade() { Id = String.Format("Z::{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh };

            return face;
        }


        public Mesh GetAnalysisMesh(List<Facade> facade)
        {
            Mesh mesh = new Mesh();
            foreach (Facade f in facade) mesh.Append(f.Geometry);
            return mesh;
        }

        private static string GetXmlWithoutWrongTag(string inx)
        {
            string file = File.ReadAllText(inx);
            string correctFile = file.Replace(TREE_WRONG_TAG, TEMP_TREE_TAG);
            return correctFile;
        }

        public static string GetXmlWithWrongTag(string filetext)
        {
            filetext = filetext.Replace(TEMP_TREE_TAG, TREE_WRONG_TAG);
            return filetext;
        }

        public static string UpdateRowMaterial(string row, CellMaterial material)
        {
            string[] values = row.Split(',');

            UpdateMaterial(values, (int)FaceDirection.X + SHIFT, material.MaterialX);
            UpdateMaterial(values, (int)FaceDirection.Y + SHIFT, material.MaterialY);
            UpdateMaterial(values, (int)FaceDirection.Z + SHIFT, material.MaterialZ);

            return String.Join(",", values);
        }

        protected static void UpdateMaterial(string[] row, int index, string materialDir)
        {
            if (materialDir != null && row[index] != String.Empty)
            {
                row[index] = materialDir;
            }
        }

    }
}
