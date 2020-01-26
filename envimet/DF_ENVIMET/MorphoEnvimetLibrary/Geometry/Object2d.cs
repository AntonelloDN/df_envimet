using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MorphoEnvimetLibrary.Geometry
{
    public class Object2d : Entity
    {
        public Object2d(Mesh geometry, Material material)
            : base(geometry, material) { }


        public Matrix2d Create2DMatrixPerObj(Grid grid, int index)
        {
            Matrix2d grid2d = new Matrix2d(grid.NumX, grid.NumY);
            List<Point3d> points = Building.GetMinGridForProjection(GetMesh(), grid);
            Ray3d ray = new Ray3d();

            foreach (Point3d pt in points)
            {
                int valX = (int)Math.Round(((pt.X - grid.MinX) / grid.DimX), 0);
                int valY = (int)Math.Round(((pt.Y - grid.MinY) / grid.DimY), 0);

                ray = new Ray3d(pt, Vector3d.ZAxis);

                var intersection = Rhino.Geometry.Intersect.Intersection.MeshRay(_geometry, ray);
                if (intersection != -1.0)
                    grid2d[valX, valY] = index;
            }

            return grid2d;

        }


        public static string Merge2dMatrixWithMaterial(List<Matrix2d> multiMatrix, List<Material> materials, string defaultMaterial)
        {
            // flip and traspose
            string visualMatrix = string.Empty;

            Matrix2d matrix = Matrix2d.Union(multiMatrix);
            
            string text = string.Empty;

            for (int j = matrix.GetLengthY() - 1; j >= 0; j--)
            {
                string[] line = new string[matrix.GetLengthX()];
                for (int i = 0; i < matrix.GetLengthX(); i++)
                {
                    line[i] = (matrix[i, j] != 0) ? materials[matrix[i, j] - 1].Custom2dMaterial : defaultMaterial;
                }
                string row = String.Join(",", line);
                row += "\n";
                text += row;
            }

            return text;
        }

    }
}
