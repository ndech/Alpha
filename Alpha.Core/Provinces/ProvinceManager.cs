using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Buildings;
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

        internal List<BaseSettlementType> BaseSettlementTypes { get; private set; }
        private readonly List<ResourceType> _resourceTypes;
        public IEnumerable<ResourceType> ResourceTypes { get { return _resourceTypes; } }
        private readonly List<BuildingType> _buildingsTypes;
        public IEnumerable<BuildingType> BuildingTypes { get { return _buildingsTypes; } }
        internal List<ResourceLevel> ResourceLevels { get; private set; } 

        internal ProvinceManager(World world) : base(world)
        {
            BaseSettlementTypes = XDocument.Load(@"Data\Settlements\Settlements.xml")
                .Descendants("baseSettlements").Descendants("settlement").Select(BaseSettlementType.Create).ToList();
            _resourceTypes = XDocument.Load(@"Data\Resources\Resources.xml").Descendants("resource").Select(x => new ResourceType(x)).ToList();
            ResourceLevels = XDocument.Load(@"Data\Resources\ResourceLevels.xml").Descendants("resourceLevel").Select(x => new ResourceLevel(x)).ToList();
            _buildingsTypes = XDocument.Load(@"Data\Buildings\Buildings.xml").Descendants("building").Select(x => new BuildingType(x, _resourceTypes)).ToList();
            BuildingStatus.Initialize();
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
            return ClosestZone(position).Province;
        }

        public Zone ClosestZone(Vector3D position)
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
                    return _currentSearchZone;
                _currentSearchZone = closestNeighbourgOrSelf;
            }
        }

        internal override void Initialize()
        { }
    }
}
