using System.Linq;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public static class ProvincePicker
    {
        internal static void Initialize(Zone seed)
        {
            _currentSearchZone = seed;
        }

        private static Zone _currentSearchZone;
        public static Province ClosestProvince(Vector3D position)
        {
            return ClosestZone(position).Province;
        }

        public static Zone ClosestZone(Vector3D position)
        {
            while (true)
            {
                Zone closestNeighbourgOrSelf =
                    _currentSearchZone.Adjacencies.Select(a => a.Neighbourg)
                        .Union(_currentSearchZone)
                        .MinBy(p => Vector3D.Distance(p.Center, position));
                if (closestNeighbourgOrSelf == _currentSearchZone)
                    return _currentSearchZone;
                _currentSearchZone = closestNeighbourgOrSelf;
            }
        }
    }
}
