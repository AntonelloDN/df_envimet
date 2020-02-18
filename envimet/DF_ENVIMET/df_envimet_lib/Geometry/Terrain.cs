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

            Mesh baseMesh = Rhino.Geometry.Mesh.CreateFromClosedPolyline(arrayCrv1[0]);

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
            TerrainflagMatrix = "";
            int numberZpoint = (grid.CombineGridType && grid.Telescope > 0) ? grid.NumZ + Grid.FIRST_CELL_COMBINED_GRID : grid.NumZ;
            _matrix = new Matrix3d(grid.NumX, grid.NumY, numberZpoint);

            foreach (Point3d pt in terrainPoints)
            {
                try
                {
                    int valX = (int)Math.Round(((pt.X - grid.MinX) / grid.DimX), 0);
                    int valY = (int)Math.Round(((pt.Y - grid.MinY) / grid.DimY), 0);
                    int valZ = (int)Math.Round(grid.CastingPrecision(pt.Z), 0);

                    _matrix[valX, valY, valZ] = 1;
                    TerrainflagMatrix += String.Format("{0},{1},{2},1.00000\n", valX, valY, valZ);
                }
                catch (IndexOutOfRangeException)
                {
                    // terrain is bigger than grid
                    continue;
                }
            }
        }

        public static Matrix2d DemTop2d(Matrix3d matrix, Grid grid)
        {

            Matrix2d grid2d = new Matrix2d(matrix.GetLengthX(), matrix.GetLengthY());


            for (int i = 0; i < matrix.GetLengthX(); i++)
            {
                int[] up = new int[matrix.GetLengthZ()];
                for (int j = 0; j < matrix.GetLengthY(); j++)
                {
                    for (int k = 0; k < matrix.GetLengthZ(); k++)
                    {
                        up[k] = (matrix[i, j, k] != 0) ? (int)Math.Round(grid.Height[k], MIN_LIMIT) : MIN_LIMIT;
                    }
                    int max = up.Max();
                    grid2d[i, j] = max;
                }
            }

            return grid2d;
        }
    }
}
