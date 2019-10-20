using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Rhino.Geometry;
using System.Collections.Generic;
using MorphoEnvimetLibrary.Geometry;

namespace editEnvimetObject
{
    public class PostProcessing : Grid
    {
        private const double Tol = 0.01;

        enum CellDirection
        {
            ValueInX = 3,
            ValueInY,
            ValueInZ
        }

        public List<Point3d> GeneratePoints(List<string> rows, Grid grid)
        {
            List<Point3d> pts = new List<Point3d>();

            foreach (string row in rows)
            {
                string[] selString = row.Split(',');
                int zIndex = Convert.ToInt32(selString[2]);
                double xVal = Convert.ToDouble(selString[0]);
                double yVal = Convert.ToInt32(selString[1]);

                pts.Add(new Point3d((xVal * grid.DimX) + grid.MinX, (yVal * grid.DimY) + grid.MinY, grid.Height[zIndex]));
            }

            return pts;
        }

        public List<Point3d> ExtractPointFromINX(string INXfileAddress, Grid grid, string keyWord, ref List<string> rowData)
        {

            List<Point3d> pts = new List<Point3d>();
            Grid newGrid;


            // Handle invalid tags within INX
            string myFile = File.ReadAllText(INXfileAddress);
            myFile = myFile.Replace("3Dplants", "threeDplants");


            // Parse XML
            XDocument doc = XDocument.Parse(myFile);
            var descendants = doc.Descendants(keyWord);

            // get dimensions
            double dimX = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dx").Value);
            double dimY = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dy").Value);
            double dimZ = Convert.ToDouble(doc.Root.Element("modelGeometry").Element("dz-base").Value);

            // TODO: read external files
            if (grid == null)
            {
                newGrid = new PostProcessing()
                {
                    DimZ = dimZ
                };
                newGrid.CalcGzDimension();
            }
            else
            {
                newGrid = grid;
            }


            foreach (var item in descendants)
            {
                string itemString = item.Value;

                string[] listItem = itemString.Split('\n').Skip(1).ToArray();

                rowData = listItem.ToList();
                rowData.Remove("");
            }

            return GeneratePoints(rowData, newGrid);
        }

        public string RestoreInvalidTag(string filetext)
        {
            filetext = filetext.Replace("threeDplants", "3Dplants");
            return filetext;
        }

        public static bool PointContainmentCheck(Point3d pt, Curve crv)
        {
            // project point to world XY
            Transform xprj = Transform.PlanarProjection(Plane.WorldXY);
            pt.Transform(xprj);

            var coitainmentValue = crv.Contains(pt, Plane.WorldXY, Tol);
            bool val = (coitainmentValue == PointContainment.Inside) ? true : false;

            return val;
        }

        public Mesh UnionMeshUtility(List<Mesh> meshes)
        {
            Mesh myMesh = new Mesh();

            foreach (Mesh m in meshes)
            {
                myMesh.Append(m);
            }

            return myMesh;
        }

        public bool PointsInShapesCheck(Mesh meshUnion, Point3d testPoint)
        {
            return meshUnion.IsPointInside(testPoint, Tol, false);
        }

        public string UpdateRowTextMaterial(string textRow, CellMaterial newMaterial)
        {
            string[] stringValues = textRow.Split(',');

            UpdateMaterial(stringValues, (int)CellDirection.ValueInX, newMaterial.MaterialX);
            UpdateMaterial(stringValues, (int)CellDirection.ValueInY, newMaterial.MaterialY);
            UpdateMaterial(stringValues, (int)CellDirection.ValueInZ, newMaterial.MaterialZ);

            return String.Join(",", stringValues);
        }

        protected void UpdateMaterial(string[] rowText, int index, string materialDir)
        {
            if (materialDir != null && rowText[index] != String.Empty)
            {
                rowText[index] = materialDir;
            }
        }

    }

    public struct CellMaterial
    {
        public string MaterialX { set; get; }
        public string MaterialY { set; get; }
        public string MaterialZ { set; get; }
    }

}
