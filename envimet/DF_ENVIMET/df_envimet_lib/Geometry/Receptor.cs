using Rhino.Geometry;
using System.Collections.Generic;

namespace df_envimet_lib.Geometry
{
    public class Receptor : Object2d
    {
        public string Name {get; private set;}

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
                        Name = (i + 1).ToString() + "X" + (j + 1).ToString();
                        stringForXml.Add(new string[] { (i).ToString(), (j).ToString(), Name });
                    }
                }
            }
            return stringForXml;
        }
    }
}
