using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;
using Alpha.WorldGeneration;

namespace Alpha.Core.Provinces
{
    public class Zone
    {
        public Vector3D Center { get; private set; }
        public List<Vector3D> Points { get; private set; }
        
        public Zone(Vector3D center, IEnumerable<Vector3D> points)
        {
            Points = points.ToList();
            Center = center;
        }

        public static explicit operator Zone(VoronoiSite site)
        {
            return new Zone(site.Center, site.Points);
        }
    }
}
