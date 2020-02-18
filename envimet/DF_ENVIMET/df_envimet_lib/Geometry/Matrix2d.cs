using System.Collections.Generic;

namespace df_envimet_lib.Geometry
{
    public class Matrix2d
    {
        private readonly int[,] _values;

        public int this[int x, int y] //matrix[0,0] = 29
        {
            get { return _values[x, y]; }
            set { _values[x, y] = value; }
        }

        public Matrix2d(int x, int y) // Matrix2d matrix = new Matrix2d(5,5)
        {
            _values = new int[x, y];
        }

        public int GetLengthX()
        {
            return _values.GetLength(0);
        }

        public int GetLengthY()
        {
            return _values.GetLength(1);
        }

        public static Matrix2d Union(List<Matrix2d> matrix)
        {
            int dimX = matrix[0].GetLengthX();
            int dimY = matrix[0].GetLengthY();

            Matrix2d unionMatrix = new Matrix2d(dimX , dimY);

            foreach (Matrix2d m in matrix)
            {
                for (int i = 0; i < m.GetLengthX(); i++)
                    for (int j = 0; j < m.GetLengthY(); j++)
                    {
                        // Add cell togheter and overlap workaround, last arrived win ;P
                        unionMatrix[i, j] += m[i, j];
                        if (unionMatrix[i, j] >= matrix.Count)
                        {
                            unionMatrix[i, j] = m[i, j];
                        }
                    }
            }

            return unionMatrix;
        }

    }
}
