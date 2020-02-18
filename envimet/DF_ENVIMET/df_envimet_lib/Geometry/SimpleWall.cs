using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace df_envimet_lib.Geometry
{
    public class SimpleWall : Entity
    {

        public SimpleWall(Mesh geometry, Material material) : 
            base(geometry, material) { }

        private int GenerateMinValueZ(Grid grid)
        {
            BoundingBox bboxGeo = GetMesh().GetBoundingBox(true);
            // Min point
            Point3d minPoint = bboxGeo.Min;
            double zValueGeo = minPoint.Z;

            return DiffArrayZdir(grid, zValueGeo);
        }

        private int DiffArrayZdir(Grid grid, double num)
        {
            List<double> values = new List<double>();
            grid.Height.ToList().ForEach(val => values.Add(Math.Abs(num - val)));
            return values.IndexOf(values.Min());
        }

        public string SimpleWallStringCalcZdir(Grid grid)
        {
            string contentXml = "";
            for (int i = 0; i < grid.NumX; i++)
                for (int j = 0; j < grid.NumY; j++)
                {
                    Point3d point = new Point3d((i * grid.DimX) + grid.MinX, (j * grid.DimY) + grid.MinY, 0);
                    Ray3d ray = new Ray3d(point, Vector3d.ZAxis);

                    var intersection = Rhino.Geometry.Intersect.Intersection.MeshRay(GetMesh(), ray);

                    if (intersection != -1.0)
                    {
                        contentXml += String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, GenerateMinValueZ(grid), "", "", this.GetMaterial().CustomSimpleWallMaterial);
                    }
                }
            return contentXml;
        }

    }
}
