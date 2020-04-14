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

    #region Pixel Struct
    public struct PixelCoordinate
    {
        public double I { set; get; }
        public double J { set; get; }
        public int K { set; get; }
        public string MaterialX { set; get; }
        public string MaterialY { set; get; }
        public string MaterialZ { set; get; }
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
        private const double TOLERANCE = 0.001;

        public string Id { get; set; }
        public Mesh Geometry { get; set; }
        public Point3d Centroid { get; set; }

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
            double numZ = 0;
            try
            {
                numZ = Convert.ToInt32(doc.Root.Element("modelGeometry3D").Element("grids3D-K").Value);
            }
            catch
            {
                //TODO: 3D only for now, extend to 2.5D
                return null;
            }

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


        public static IEnumerable<Facade> GenerateFacadeByDirection(List<string> rows, Grid grid, FaceDirection direction)
        {
            List<Facade> result = new List<Facade>();

            List<PixelCoordinate> pixels = GetPixelFromSparseMatrix(rows).ToList();

            foreach (PixelCoordinate pix in pixels)
            {
                Facade face = new Facade();

                if (direction == FaceDirection.X && pix.MaterialX.Length >= 2)
                {
                    face = GenerateXfacade(grid, pix);
                    result.Add(face);
                }
                else if (direction == FaceDirection.Y && pix.MaterialY.Length >= 2)
                {
                    face = GenerateYfacade(grid, pix);
                    result.Add(face);
                }
                else if (direction == FaceDirection.Z && pix.MaterialZ.Length >= 2 && pix.K != 0)
                {
                    face = GenerateZfacade(grid, pix);
                    result.Add(face);
                }
            }

            return result;
        }


        public static void BakeFacades(IEnumerable<Facade> faces, bool bake = false, Curve crv = null)
        {
            Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;

            foreach (Facade face in faces)
            {
                Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes { Name = face.Id };
                if (crv != null)
                {
                    if (PointContainmentCheck(face.Centroid, crv))
                    {
                        ot.AddMesh(face.Geometry, att);
                    }
                }
                else
                {
                    ot.AddMesh(face.Geometry, att);
                }
            }
        }


        public static IEnumerable<PixelCoordinate> GetPixelFromSparseMatrix(List<string> rows)
        {
            List<PixelCoordinate> pixels = new List<PixelCoordinate>();

            for (int i = 0; i < rows.Count; i++)
            {
                Facade face = new Facade();

                string[] row = rows[i].Replace(" ", "").Split(',');
                double x = Convert.ToInt32(row[(int)FaceDirection.X]);
                double y = Convert.ToInt32(row[(int)FaceDirection.Y]);
                int z = Convert.ToInt32(row[(int)FaceDirection.Z]);

                string matX = row[3];
                string matY = row[4];
                string matZ = row[5];

                PixelCoordinate pix = new PixelCoordinate() { I = x, J = y, K = z, MaterialX = matX, MaterialY = matY, MaterialZ = matZ };

                pixels.Add(pix);
            }
            return pixels;
        }


        protected static Facade GenerateXfacade(Grid grid, PixelCoordinate pix)
        {
            var sequence = grid.Sequence;
            var position = grid.Height;

            var point = new Point3d((pix.I * grid.DimX) + grid.MinX - (grid.DimX / 2), (pix.J * grid.DimY) + grid.MinY, position[pix.K]);
            var plane = new Plane(point, Vector3d.XAxis);
            var dimY = grid.DimY;
            var mesh = Mesh.CreateFromPlane(plane, new Interval(-dimY/2, dimY/2), new Interval(-sequence[pix.K] / 2, sequence[pix.K] / 2), 1, 1);

            Facade face = new Facade() { Id = String.Format("X:{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh, Centroid = point };

            return face;
        }

        protected static Facade GenerateYfacade(Grid grid, PixelCoordinate pix)
        {
            var sequence = grid.Sequence;
            var position = grid.Height;

            var point = new Point3d((pix.I * grid.DimX) + grid.MinX, (pix.J * grid.DimY) + grid.MinY - (grid.DimY/2), position[pix.K]);
            var plane = new Plane(point, Vector3d.YAxis);
            var dimZ = sequence[pix.K] / 2;
            var dimX = grid.DimX;
            var mesh = Mesh.CreateFromPlane(plane, new Interval(-dimZ, dimZ), new Interval(-dimX/2, dimX/2), 1, 1);

            Facade face = new Facade() { Id = String.Format("Y:{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh, Centroid = point };

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
            var mesh = Mesh.CreateFromPlane(plane, new Interval(-dimX/2, dimX/2), new Interval(-dimY/2, dimY/2), 1, 1);

            Facade face = new Facade() { Id = String.Format("Z:{0}:{1}:{2}", pix.I, pix.J, pix.K), Geometry = mesh, Centroid = point };

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

        public static IEnumerable<PixelCoordinate> GetUniqueSelectedPixels(List<PixelCoordinate> pixels)
        {
            List<PixelCoordinate> uniquePixels = new List<PixelCoordinate>();
            List<string> series = new List<string>();

            foreach (PixelCoordinate pix in pixels)
            {
                string line = String.Join(",", new[] { pix.I, pix.J, pix.K });
                series.Add(line);
            }

            series = series.Distinct().ToList();

            for (int i = 0; i < series.Count; i++)
            {
                string[] elements = series[i].Split(',');
                int ci = Convert.ToInt32(elements[0]);
                int cj = Convert.ToInt32(elements[1]);
                int ck = Convert.ToInt32(elements[2]);

                var pix = new PixelCoordinate() { I = ci, J = cj, K = ck };

                foreach (PixelCoordinate pixelSel in pixels)
                {
                    if ((pixelSel.I == pix.I) && (pixelSel.J == pix.J) && (pixelSel.K == pix.K))
                    {
                        if (pixelSel.MaterialX != null)
                            pix.MaterialX = pixelSel.MaterialX;
                        if (pixelSel.MaterialY != null)
                            pix.MaterialY = pixelSel.MaterialY;
                        if (pixelSel.MaterialZ != null)
                            pix.MaterialZ = pixelSel.MaterialZ;
                    }
                }
                uniquePixels.Add(pix);
            }
            return uniquePixels;
        }

        public static IEnumerable<PixelCoordinate> UpdatePixelMaterial(List<PixelCoordinate> pixelsInx, List<PixelCoordinate> pixelSelection)
        {
            List<PixelCoordinate> pixels = new List<PixelCoordinate>();
            for (int i = 0; i < pixelsInx.Count; i++)
            {
                var pix = pixelsInx[i];
                foreach (PixelCoordinate pixelSel in pixelSelection)
                {
                    if ((pix.I == pixelSel.I) && (pix.J == pixelSel.J) && (pix.K == pixelSel.K))
                    {
                        if (pixelSel.MaterialX != null)
                            pix.MaterialX = pixelSel.MaterialX;
                        if (pixelSel.MaterialY != null)
                            pix.MaterialY = pixelSel.MaterialY;
                        if (pixelSel.MaterialZ != null)
                            pix.MaterialZ = pixelSel.MaterialZ;
                    }
                }
                pixels.Add(pix);
            }
            return pixels;
        }

        public static string GetFacadeSparseMatrixFromPixels(IEnumerable<PixelCoordinate> pixels)
        {
            List<string> lines = new List<string>();
            foreach (PixelCoordinate pix in pixels)
            {
                lines.Add(String.Join(",", new string[] { pix.I.ToString(), pix.J.ToString(), pix.K.ToString(), pix.MaterialX, pix.MaterialY, pix.MaterialZ }));
            }
            return String.Join("\n", lines);
        }

        public static IEnumerable<PixelCoordinate> GetPixelFromSelection(List<Rhino.DocObjects.RhinoObject> rhinoObjects, string material)
        {
            List<PixelCoordinate> pixels = new List<PixelCoordinate>();
            foreach (Rhino.DocObjects.RhinoObject rhobj in rhinoObjects)
            {
                if (rhobj.Name.Contains("X:") || rhobj.Name.Contains("Y:") || rhobj.Name.Contains("Z:"))
                {
                    PixelCoordinate pix = GetPixelFromRhinoObject(rhobj.Name, material);
                    pixels.Add(pix);
                }
            }

            return pixels;
        }

        private static PixelCoordinate GetPixelFromRhinoObject(string name, string material)
        {
            string[] pixelInfo = name.Split(':');

            int i = Convert.ToInt32(pixelInfo[1]);
            int j = Convert.ToInt32(pixelInfo[2]);
            int k = Convert.ToInt32(pixelInfo[3]);

            PixelCoordinate pix = new PixelCoordinate() { I = i, J = j, K = k };

            switch (pixelInfo[0])
            {
                case "X":
                    pix.MaterialX = material;
                    break;
                case "Y":
                    pix.MaterialY = material;
                    break;
                default:
                    pix.MaterialZ = material;
                    break;
            }

            return pix;
        }

        public static bool PointContainmentCheck(Point3d pt, Curve crv)
        {
            // project point to world XY
            Transform xprj = Transform.PlanarProjection(Plane.WorldXY);
            pt.Transform(xprj);

            var coitainmentValue = crv.Contains(pt, Plane.WorldXY, TOLERANCE);
            bool val = (coitainmentValue == PointContainment.Inside) ? true : false;

            return val;
        }

    }
}
