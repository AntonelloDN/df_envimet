using Rhino.Geometry;
using System;

namespace MorphoEnvimetLibrary.Geometry
{
    public abstract class Entity : IDisposable
    {
        private bool disposed = false;
        protected Material _material;
        protected Mesh _geometry;
        protected const int MAX_LIMIT = 9999;
        protected const int MIN_LIMIT = 0;

        public Material GetMaterial()
        {
            return _material;
        }

        public virtual Mesh GetMesh()
        {
            return _geometry;
        }

        public Entity(Mesh geometry, Material material)
        {
            _geometry = geometry;
            _material = material;
        }

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

        ~Entity()
        {
            Dispose(false);
        }
    }

    public struct Material
    {
        public static string CommonSoilMaterial = "000000";
        public static string DefaultEmpty = "";
        public static string CommonPlant2dMaterial = "0000XX";
        public static string CommonWallMaterial = "000000";
        public static string DefaultGreenWallMaterial = "";
        public static string CommonRoofMaterial = "000000";
        public static string DefaultGreenRoofMaterial = "";
        public static string CommonSourceMaterial = "0000FT";
        public static string CommonSimpleWall = "000001";
        public static string CommonPlant3dMaterial = "0000C2,.PINETREE";

        public string WallMaterial { get; set; }
        public string RoofMaterial { get; set; }
        public string GreenWallMaterial { get; set; }
        public string GreenRoofMaterial { get; set; }
        public string Custom2dMaterial { get; set; }
        public string Plant3dMaterial { get; set; }
        public string CustomSimpleWallMaterial { get; set; }

        public static void ResetValue()
        {
            CommonSoilMaterial = "000000";
            DefaultEmpty = "";
            CommonPlant2dMaterial = "0000XX";
            CommonWallMaterial = "000000";
            DefaultGreenWallMaterial = "";
            CommonRoofMaterial = "000000";
            DefaultGreenRoofMaterial = "";
            CommonSourceMaterial = "0000FT";
            CommonSimpleWall = "000001";
            CommonPlant3dMaterial = "0000C2,.PINETREE";
        }
    }
}
