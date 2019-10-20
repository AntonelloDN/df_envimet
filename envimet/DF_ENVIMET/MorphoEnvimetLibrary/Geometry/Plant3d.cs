using Rhino.Geometry;
using System.Collections.Generic;

namespace MorphoEnvimetLibrary.Geometry
{
    public class Plant3d : Object2d
    {
        public Plant3d(Material material, Mesh geometry)
           : base(geometry, material) { }

        public List<string[]> CreateAttributes(Matrix2d matrix)
        {
            List<string[]> stringForXml = new List<string[]>();
            for (int i = 0; i < matrix.GetLengthX(); i++)
            {
                for (int j = 0; j < matrix.GetLengthY(); j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        string[] idAndDescription = _material.Plant3dMaterial.Split(',');
                        stringForXml.Add(new string[] { (i + 1).ToString(), (j + 1).ToString(), "0", idAndDescription[0], idAndDescription[1], "0" });
                    }
                }
            }
            return stringForXml;
        }

    }
}
