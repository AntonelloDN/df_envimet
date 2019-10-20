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

            Point3d point = new Point3d();
            Ray3d ray = new Ray3d();
            for (int i = 0; i < grid.NumX; i++)
                for (int j = 0; j < grid.NumY; j++)
                {
                    grid2d[i, j] = 0;
                    point = new Point3d((i * grid.DimX) + grid.MinX, (j * grid.DimY) + grid.MinY, 0);
                    if (points.Contains(point))
                    {
                        ray = new Ray3d(point, Vector3d.ZAxis);
                        Plane plane = new Plane(new Point3d(0, 0, grid.Height[0]), Vector3d.ZAxis);
                        Transform xprj = Transform.PlanarProjection(plane);
                        GetMesh().Transform(xprj);

                        var intersection = Rhino.Geometry.Intersect.Intersection.MeshRay(_geometry, ray);
                        if (intersection != -1.0)
                            grid2d[i, j] = index;
                    }
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
