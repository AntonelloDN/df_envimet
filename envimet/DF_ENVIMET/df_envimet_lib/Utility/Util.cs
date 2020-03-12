using System;
using System.Collections.Generic;
using System.Linq;

namespace df_envimet_lib.Utility
{
    class Util
    {
        public static IEnumerable<double> Accumulate(IEnumerable<double> sequence)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += item;
                yield return sum;
            }
        }

        public static List<double> ConvertToNumber(string cell)
        {
            var spacing = cell.Split(',')
              .Select(v => Convert.ToDouble(v))
              .ToList();

            return spacing;
        }
    }
}
