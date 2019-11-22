using Rhino.Geometry;
using System.Collections.Generic;

namespace MorphoEnvimetLibrary.Geometry
{
    public class Receptor: Object2d
    {
        public Receptor(Mesh geometry)
        : base(geometry, new Material()) { }

        public List<string[]> CreateAttributes(Matrix2d matrix)
        {
            List<string[]> stringForXml = new List<string[]>();
            for (int i = 0; i < matrix.GetLengthX(); i++)
            {
                for (int j = 0; j < matrix.GetLengthY(); j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        // cellx, celly, ID
                        stringForXml.Add(new string[] { (i).ToString(), (j).ToString(), (i + 1).ToString() + "X" + (j + 1).ToString() });
                    }
                }
            }
            return stringForXml;
        }
    }
}
