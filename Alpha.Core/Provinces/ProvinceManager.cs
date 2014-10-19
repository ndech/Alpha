using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

        internal List<SettlementType> SettlementTypes { get; private set; }
        internal List<ResourceType> ResourceTypes { get; private set; }
        internal List<Building> Buildings { get; private set; } 



        internal ProvinceManager(World world) : base(world)
        {
            SettlementTypes = new List<SettlementType>
            {
                new SettlementType("sea_settlement", "Coastal settlement", "Living off the bounties of the sea, " +
                                                                           "this group of people may become the fishermen who will feed your realm" +
                                                                           "or the sailors who will discover plentiful lands.", 60)
            };
            ResourceTypes = XDocument.Load(@"Data\Resources\Resources.xml").Descendants("resource").Select(x => new ResourceType(x)).ToList();
            Buildings = XDocument.Load(@"Data\Buildings\Buildings.xml").Descendants("building").Select(x => new Building(x)).ToList();
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            _provinces.DayUpdate(dataLock);
        }

        internal void CreateProvince(Province province)
        {
            _provinces.Add(province);
            foreach (Zone zone in province.Zones)
                zone.Province = province;
        }

        public List<Step> CalculatePath(Fleet fleet, Zone destination)
        {
            return CalculatePath((IMovable) fleet, destination);
        }

        private List<Step> CalculatePath(IMovable movable, Zone destination)
        {
            if (movable.Location == destination || !movable.CanCross(destination))
                return null;
            List<Step> steps = new List<Step>();
            //Calculate path using A* algorithm
            SortedSet<PathfindingNode> openList = new SortedSet<PathfindingNode>(
                Comparer<PathfindingNode>.Create((a, b) => a.CompareTo(b)));
            HashSet<Zone> closedList = new HashSet<Zone>();
            openList.Add(new PathfindingNode(movable.Location, destination.DistanceWith(movable.Location)));
            bool pathFound = destination == movable.Location;
            while (!pathFound)
            {
                if (openList.Count == 0)
                    break;
                PathfindingNode currentNode = openList.First();
                foreach (Zone neighbourg in currentNode.Zone.Adjacencies.Select(a => a.Neighbourg).Where(s => movable.CanCross(s)))
                {
                    if (closedList.Contains(neighbourg) || openList.Any(n => n.Zone == neighbourg))
                        continue;
                    openList.Add(new PathfindingNode(neighbourg, destination.DistanceWith(neighbourg), currentNode));
                    if (neighbourg == destination) // Path found !
                    {
                        pathFound = true;
                        steps.Add(new Step(currentNode.Zone, destination));
                        while (currentNode.Parent != null)
                        {
                            steps.Add(new Step(currentNode.Parent.Zone, currentNode.Zone));
                            currentNode = currentNode.Parent;
                        }
                        steps.Reverse();
                        break;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode.Zone);
            }
            if (steps.Count == 0)
                return steps;
            return steps;
        }

        private Zone _currentSearchZone;
        public Province ClosestProvince(Vector3D position)
        {
            if (_currentSearchZone == null)
                _currentSearchZone = _provinces.First().Zones.First();
            while (true)
            {
                Zone closestNeighbourgOrSelf =
                    _currentSearchZone.Adjacencies.Select(a => a.Neighbourg)
                        .Union(_currentSearchZone)
                        .MinBy(p => Vector3D.Distance(p.Center, position));
                if (closestNeighbourgOrSelf == _currentSearchZone)
                    return _currentSearchZone.Province;
                _currentSearchZone = closestNeighbourgOrSelf;
            }
        }

        internal override void Initialize()
        { }
    }
}
