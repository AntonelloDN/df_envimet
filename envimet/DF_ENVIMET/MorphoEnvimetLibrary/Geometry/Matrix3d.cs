using System.Collections.Generic;

namespace MorphoEnvimetLibrary.Geometry
{
    public class Matrix3d
    {
        private readonly int[,,] _values;

        public int this[int x, int y, int z] //matrix[0,0,0] = 29
        {
            get { return _values[x, y, z]; }
            set { _values[x, y, z] = value; }
        }

        public Matrix3d(int x, int y, int z) // Matrix3d matrix = new Matrix3d(5,5,6)
        {
            _values = new int[x, y, z];
        }

        public int GetLengthX()
        {
            return _values.GetLength(0);
        }

        public int GetLengthY()
        {
            return _values.GetLength(1);
        }

        public int GetLengthZ()
        {
            return _values.GetLength(2);
        }

        public static Matrix3d Union(List<Matrix3d> matrix)
        {
            int dimX = matrix[0].GetLengthX();
            int dimY = matrix[0].GetLengthY();
            int dimZ = matrix[0].GetLengthZ();

            Matrix3d unionMatrix = new Matrix3d(dimX, dimY, dimZ);

            foreach (Matrix3d m in matrix)
            {
                for (int i = 0; i < m.GetLengthX() ; i++)
                    for (int j = 0; j < m.GetLengthY(); j++)
                        for (int k = 0; k < m.GetLengthZ(); k++)
                        {
                            // Add cell togheter and overlap workaround
                            unionMatrix[i, j, k] += m[i, j, k];
                            if (unionMatrix[i, j, k] >= matrix.Count + 1)
                                unionMatrix[i, j, k] = m[i, j, k]; // Before 0
                        }
            }

            return unionMatrix;
        }

    }
}
