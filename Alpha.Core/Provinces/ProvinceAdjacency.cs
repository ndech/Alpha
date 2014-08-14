using System.Collections.Generic;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public class ProvinceAdjacency
    {
        public Province Neighbourg { get; internal set; }
        public List<Vector3D> PassingPoints { get; internal set; } 
    }
}
