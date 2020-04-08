using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace df_envimet_lib.Geometry
{
    public class Terrain : Entity
    {
        private Matrix3d _matrix;
        private Mesh _closedMesh;

        public string TerrainflagMatrix { get; set; }

        public Terrain(Mesh geometry, Material material)
            : base(geometry, material)
        {
            _closedMesh = geometry;
            if (!geometry.IsClosed)
            {
                _closedMesh = CreateClosedMeshTerrain(geometry);
            }
        }

        public Matrix3d GetMatrix()
        {
            return _matrix;
        }

        public override Mesh GetMesh()
        {
            return _closedMesh;
        }

        public Mesh GetSrfMesh()
        {
            return _geometry;
        }

        private Mesh CreateClosedMeshTerrain(Mesh surfTerrainMesh)
        {
            // bulk mesh
            Mesh finalMesh = new Mesh();

            // borders
            Polyline[] arrayCrv1 = surfTerrainMesh.GetOutlines(Rhino.Geometry.Plane.WorldXY);
            Polyline[] arrayCrv2 = surfTerrainMesh.GetNakedEdges();

            Mesh baseMesh = Mesh.CreateFromClosedPolyline(arrayCrv1[0]);

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

        public void CreateVoxMatrixTerrain(Point3d[] terrainPoints, Grid grid)
        {
            TerrainflagMatrix = String.Empty;

            List<string> tempTerrainFlag = new List<string>();

            int numberZpoint = (grid.CombineGridType && grid.Telescope > 0) ? grid.NumZ + Grid.FIRST_CELL_COMBINED_GRID : grid.NumZ;

            foreach (Point3d pt in terrainPoints)
            {
                try
                {
                    int valX = (int)Math.Round(((pt.X - grid.MinX) / grid.DimX), 0);
                    int valY = (int)Math.Round(((pt.Y - grid.MinY) / grid.DimY), 0);
                    int valZ = (int)Math.Round(grid.CastingPrecision(pt.Z), 0);

                    tempTerrainFlag.Add(String.Format("{0},{1},{2},1.00000", valX, valY, valZ));
                }
                catch (IndexOutOfRangeException)
                {
                    // terrain is bigger than grid
                    continue;
                }
            }
            TerrainflagMatrix = String.Join("\n", tempTerrainFlag) + "\n";
        }

        public static List<Point3d> GetMatrixFromRayShoot(Grid grid, Mesh union, ref Matrix2d matrix)
        {

            List<Point3d> points = new List<Point3d>();

            int zDim = Building.MAX_LIMIT;
            Vector3d axis = -Vector3d.ZAxis;

            for (int i = 0; i < grid.NumX; i++)
                for (int j = 0; j < grid.NumY; j++)
                {
                    Point3d point = new Point3d((i * grid.DimX) + grid.MinX, (j * grid.DimY) + grid.MinY, zDim);
                    Ray3d ray = new Ray3d(point, axis);

                    var intersection = Rhino.Geometry.Intersect.Intersection.MeshRay(union, ray);
                    
                    if (intersection != -1.0)
                    {
                        double value = ray.PointAt(intersection).Z;

                        if (value < 0)
                            value = 0;
                        Grid.FilterListBasedOnNumber(grid.Height.ToList(), value).ForEach(v =>
                            {
                                points.Add(new Point3d(point.X, point.Y, v));
                            });
                        matrix[i, j] = (int)Math.Round((float)value, MidpointRounding.AwayFromZero);
                    }
                }

            return points;

        }

    }
}
