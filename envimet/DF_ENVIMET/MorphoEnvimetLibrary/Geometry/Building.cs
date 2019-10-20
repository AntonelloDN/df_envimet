using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorphoEnvimetLibrary.Geometry
{
    public class Building : Entity
    {
        public const double TOLERANCE = 0.01d;

        private Matrix3d _matrix;

        public string BuildingFlagAndNr { get; private set; }

        public bool SimplifiedCalculation { get; set; }

        public Matrix3d GetMatrix3d()
        {
            return _matrix;
        }

        public Building(Mesh geometry, Material material)
            : base(geometry, material) { }

        public void CreateVoxMatrixBuilding(Point3d[] buildingPoints, Point3d[] terrainPoints, Grid grid, int index)
        {
            BuildingFlagAndNr = "";
            int numberZpoint = (grid.CombineGridType && grid.Telescope > 0) ? grid.NumZ + Grid.FIRST_CELL_COMBINED_GRID : grid.NumZ;
            _matrix = new Matrix3d(grid.NumX, grid.NumY, numberZpoint);

            foreach (Point3d pt in buildingPoints)
            {
                int valX = (int) Math.Round(((pt.X - grid.MinX) / grid.DimX), 0);
                int valY = (int) Math.Round(((pt.Y - grid.MinY) / grid.DimY), 0);

                if (!terrainPoints.Contains(pt))
                {
                    int valZ = (int) Math.Round(grid.CastingPrecision(pt.Z),0);

                    _matrix[valX, valY, valZ] = index;
                    BuildingFlagAndNr += String.Format("{0},{1},{2},{3},{4}\n", valX, valY, valZ, 1, index);
                }
            }
        }

        // static methods
        public static Point3d[] VoxelPoints(Mesh mesh, Grid grid, double tolerance = TOLERANCE)
        {
            List<Point3d> pointForProjection = GetMinGridForProjection(mesh, grid);

            List<Plane> planes = new List<Plane>();
            grid.Height.ToList().ForEach(v => planes.Add(new Plane(new Point3d(0, 0, v), Vector3d.ZAxis)));
            IEnumerable<Curve> polylines = Rhino.Geometry.Intersect.Intersection.MeshPlane(mesh, planes)
                                    .Select(p => p.ToPolylineCurve());
            // from array to list
            List<Brep> volume = new List<Brep>(Rhino.Geometry.Brep.CreatePlanarBreps(polylines, tolerance));

            Point3d[] voxelPoints = Rhino.Geometry.Intersect.Intersection.ProjectPointsToBreps(volume, pointForProjection, Vector3d.ZAxis, tolerance);

            return voxelPoints;
        }

        public static Point3d[] VoxelPoints(Mesh mesh, Grid grid)
        {
            List<Point3d> pointForProjection = GetMinGridForProjection(mesh, grid);

            BoundingBox bbox = mesh.GetBoundingBox(true);
            double zMax = bbox.Max.Z;

            List<double> heights = grid.Height.Where(n => n <= zMax)
                                .ToList();

            List<Point3d> voxelPoints = new List<Point3d>();
            
            Ray3d ray = new Ray3d();
            foreach (Point3d pt in pointForProjection)
            {
                ray = new Ray3d(pt, Vector3d.ZAxis);
                var intersect = Rhino.Geometry.Intersect.Intersection.MeshRay(mesh, ray);
                if (intersect != -1.0)
                {
                    foreach (double h in heights)
                        voxelPoints.Add(new Point3d(pt.X, pt.Y, h));
                }
            }
            return voxelPoints.ToArray();
        }

        public static List<Point3d> GetMinGridForProjection(Mesh mesh, Grid grid)
        {
            // internal method
            List<Point3d> gridXY = grid.GridXY();

            // BBox
            BoundingBox bbox = mesh.GetBoundingBox(false);

            List<Point3d> pointForProjection = new List<Point3d>();

            // small scaled grid
            foreach (Point3d pt in gridXY)
                if (pt.X >= bbox.Min.X - grid.DimX && pt.X <= bbox.Max.X + grid.DimX)
                    if (pt.Y >= bbox.Min.Y - grid.DimY && pt.Y <= bbox.Max.Y + grid.DimY)
                        pointForProjection.Add(new Point3d(pt.X, pt.Y, 0));
            return pointForProjection;
        }

        public static string SetMaterials(Matrix3d pts, List<Material> materials, bool green = false)
        {
            string matrice = string.Empty;
            List<string> wall = new List<string>();
            List<string> roof = new List<string>();

            if (green)
            {
                wall = materials.Select(m => m.GreenWallMaterial)
                    .ToList();
                roof = materials.Select(m => m.GreenRoofMaterial)
                                    .ToList();
            }
            else
            {
                wall = materials.Select(m => m.WallMaterial)
                                    .ToList();
                roof = materials.Select(m => m.RoofMaterial)
                                    .ToList();
            }

            for (int i = 0; i < pts.GetLengthX(); i++)
                for (int j = 0; j < pts.GetLengthY(); j++)
                    for (int k = 0; k < pts.GetLengthZ(); k++)
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
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], wall[index], roof[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], "", roof[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[index], roof[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], wall[index], "");
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], "", "");
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", roof[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[index], "");
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], wall[index], roof[index]);
                                }
                                else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], "", roof[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[index], roof[index]);
                                }
                                else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && k == 0)
                                {
                                    line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", roof[index]);
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
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], "", "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[index], "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                    {
                                        index = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", "", roof[index]);
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], wall[index], "");
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i - 1, j, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[indexW], "", roof[indexR]);
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i, j - 1, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[indexW], roof[indexR]);
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && pts[i, j, k - 1] != 0)
                                    {
                                        indexW = pts[i, j - 1, k] - 1;
                                        indexR = pts[i, j, k - 1] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[indexW], wall[indexW], roof[indexR]);
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] == 0 && k == 0)
                                    {
                                        index = pts[i - 1, j, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], "", "");
                                    }
                                    else if (pts[i - 1, j, k] == 0 && pts[i, j - 1, k] != 0 && k == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, "", wall[index], "");
                                    }
                                    else if (pts[i - 1, j, k] != 0 && pts[i, j - 1, k] != 0 && k == 0)
                                    {
                                        index = pts[i, j - 1, k] - 1;
                                        line = String.Format("{0},{1},{2},{3},{4},{5}\n", i, j, k, wall[index], wall[index], "");
                                    }
                                }
                        }
                        matrice += line;
                    }
            return matrice;
        }

        public static Matrix2d BuildingFixed2d(Matrix3d matrix, Grid grid)
        {

            Matrix2d grid2d = new Matrix2d(matrix.GetLengthX(), matrix.GetLengthY());

            for (int i = 0; i < matrix.GetLengthX(); i++)
            {
                int[] bottom = new int[matrix.GetLengthZ()];
                for (int j = 0; j < matrix.GetLengthY(); j++)
                {
                    for (int k = 0; k < matrix.GetLengthZ(); k++)
                    {
                        bottom[k] = (matrix[i, j, k] != 0) ? k : MAX_LIMIT;
                    }
                    int min = bottom.Min();
                    if (min == 0)
                    {
                        grid2d[i, j] = 0;
                    }
                    else
                    {
                        grid2d[i, j] = (min != MAX_LIMIT) ? (int) Math.Round(grid.Height[min], 0) : MIN_LIMIT;
                    }
                }
            }

            return grid2d;
        }

        public static Matrix2d BuildingId2d(Matrix3d matrix)
        {

            Matrix2d grid2d = new Matrix2d(matrix.GetLengthX(), matrix.GetLengthY());

            for (int i = 0; i < matrix.GetLengthX(); i++)
            {
                int[] id = new int[matrix.GetLengthZ()];
                for (int j = 0; j < matrix.GetLengthY(); j++)
                {
                    for (int k = 0; k < matrix.GetLengthZ(); k++)
                    {
                        id[k] = matrix[i, j, k];
                    }
                    grid2d[i, j] = id.Max();
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

            Mesh newMesh = mesh.DuplicateMesh();

            try
            {

                // first traslation
                Point3d center = AreaMassProperties.Compute(newMesh).Centroid;

                Ray3d r = new Ray3d(center, Vector3d.ZAxis);

                var intersec = Rhino.Geometry.Intersect.Intersection.MeshRay(terrain, r);

                Point3d pt = r.PointAt(intersec);

                if (intersec != noIntersection)
                {
                    Vector3d vecCentroid = new Vector3d(0, 0, pt.Z - center.Z);
                    xmoveCentroid = Transform.Translation(vecCentroid);
                    newMesh.Transform(xmoveCentroid);
                }


                // move to terrain
                BoundingBox BBox = newMesh.GetBoundingBox(true);
                Mesh meshBox = Mesh.CreateFromBox(BBox, UnitBox, UnitBox, UnitBox);

                Line[] lines = Rhino.Geometry.Intersect.Intersection.MeshMeshFast(terrain, newMesh);

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

            newMesh.Transform(xmoveTerrain);

            return newMesh;
        }

        public static Matrix2d GetMatrixFromRayShoot(Grid grid, Mesh union, bool top = false)
        {
            Matrix2d grid2d = new Matrix2d(grid.NumX, grid.NumY);

            double zDim = -MAX_LIMIT;
            Vector3d axis = Vector3d.ZAxis;
            if (top)
            {
                zDim = MAX_LIMIT;
                axis = -Vector3d.ZAxis;
            }

            for (int i = 0; i < grid.NumX; i++)
                for (int j = 0; j < grid.NumY; j++)
                {
                    Point3d point = new Point3d((i * grid.DimX) + grid.MinX, (j * grid.DimY) + grid.MinY, zDim);
                    Ray3d ray = new Ray3d(point, axis);

                    var intersection = Rhino.Geometry.Intersect.Intersection.MeshRay(union, ray);

                    if (intersection != -1.0)
                    {
                        int value = (int) Math.Floor(ray.PointAt(intersection).Z);
                        if (value < 0)
                            value = 0;
                        grid2d[i, j] = value;
                    }
                }
            return grid2d;

        }

    }

}
