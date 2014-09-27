using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Fleets;
using Alpha.Core.Movement;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public class ProvinceManager : Manager
    {

        private readonly List<Province> _provinces = new List<Province>();
        public IEnumerable<Province> Provinces { get { return _provinces; } }
        public IEnumerable<SeaProvince> SeaProvinces { get { return _provinces.OfType<SeaProvince>(); } }
        public IEnumerable<LandProvince> LandProvinces { get { return _provinces.OfType<LandProvince>(); } }

        public Province GetById(String id)
        {
            return _provinces.Single(p => p.Id.Equals(id));
        }

        internal ProvinceManager(World world) : base(world)
        {
            
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            _provinces.ForEach(province => dataLock.Write(() => ((IDailyUpdatableItem)province).DayUpdate()));
        }

        internal void CreateProvince(Province province)
        {
            _provinces.Add(province);
        }

        public List<Step> CalculatePath(Fleet fleet, Province destination)
        {
            return CalculatePath((IMovable) fleet, destination);
        }

        private List<Step> CalculatePath(IMovable movable, Province destination)
        {
            if (movable.Location == destination || !movable.CanCross(destination))
                return null;
            List<Step> steps = new List<Step>();
            //Calculate path using A* algorithm
            SortedSet<PathfindingNode> openList = new SortedSet<PathfindingNode>(
                Comparer<PathfindingNode>.Create((a, b) => a.CompareTo(b)));
            HashSet<Province> closedList = new HashSet<Province>();
            openList.Add(new PathfindingNode(movable.Location, destination.DistanceWith(movable.Location)));
            bool pathFound = destination == movable.Location;
            while (!pathFound)
            {
                if (openList.Count == 0)
                    break;
                PathfindingNode currentNode = openList.First();
                foreach (Province neighbourg in currentNode.Province.Adjacencies.Select(a => a.Neighbourg).Where(s => movable.CanCross(s)))
                {
                    if (closedList.Contains(neighbourg) || openList.Any(n => n.Province == neighbourg))
                        continue;
                    openList.Add(new PathfindingNode(neighbourg, destination.DistanceWith(neighbourg), currentNode));
                    if (neighbourg == destination) // Path found !
                    {
                        pathFound = true;
                        steps.Add(new Step(currentNode.Province, destination));
                        while (currentNode.Parent != null)
                        {
                            steps.Add(new Step(currentNode.Parent.Province, currentNode.Province));
                            currentNode = currentNode.Parent;
                        }
                        steps.Reverse();
                        break;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode.Province);
            }
            if (steps.Count == 0)
                return steps;
            return steps;
        }

        private Province _currentSearchProvince;
        public Province ClosestProvince(Vector3D position)
        {
            if (_currentSearchProvince == null)
                _currentSearchProvince = _provinces.First();
            while (true)
            {
                Province closestNeighbourgOrSelf =
                    _currentSearchProvince.Adjacencies.Select(a => a.Neighbourg)
                        .Union(_currentSearchProvince)
                        .MinBy(p => Vector3D.Distance(p.Center, position));
                if (closestNeighbourgOrSelf == _currentSearchProvince)
                    return _currentSearchProvince;
                else
                    _currentSearchProvince = closestNeighbourgOrSelf;
            }
        }

        internal override void Initialize()
        { }
    }
}
