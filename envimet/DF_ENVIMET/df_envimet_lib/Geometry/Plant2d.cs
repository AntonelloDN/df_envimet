using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace df_envimet_lib.Geometry
{
    public class Plant2d : Object2d
    {
        public Plant2d(Mesh geometry, Material material) : base(geometry, material)
        {
        }
    }
}
