using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public class ProvinceAdjacency
    {
        public ProvinceAdjacency(Province province, Province neighbourg, List<Vector3D> commonPoints)
        {
            Province = province;
            Neighbourg = neighbourg;
            CommonPoints = commonPoints;
            PassingPoints = new List<Vector3D>
            {
                commonPoints.Aggregate(new Vector3D(), (total, vector) => total + vector)/commonPoints.Count
            };
        }

        public Province Province { get; private set; }
        public Province Neighbourg { get; private set; }
        public List<Vector3D> PassingPoints { get; private set; }
        public List<Vector3D> CommonPoints { get; private set; } 
    }
}
