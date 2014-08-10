using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;

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

        //public static explicit operator Zone(VoronoiSite site)
        //{
        //    return new Zone(new Vector3((float)site.Center[0], 0.0f, (float)site.Center[1]),
        //                                site.Points.Select(p => new Vector3((float)p[0], 0.0f, (float)p[1])));
        //}
    }
}
