using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public class ZoneAdjacency
    {
        public ZoneAdjacency(Zone province, Zone neighbourg, List<Vector3D> commonPoints)
        {
            Zone = province;
            Neighbourg = neighbourg;
            CommonPoints = commonPoints;
            PassingPoints = new List<Vector3D>
            {
                commonPoints.Aggregate(new Vector3D(), (total, vector) => total + vector)/commonPoints.Count
            };
        }

        public Zone Zone { get; private set; }
        public Zone Neighbourg { get; private set; }
        public List<Vector3D> PassingPoints { get; private set; }
        public List<Vector3D> CommonPoints { get; private set; } 
    }
}
