using System;
using Alpha.WorldGeneration;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace Alpha
{
    interface IProvinceManager : IService
    {
        IList<Province> Provinces { get; }
        IList<LandProvince> LandProvinces { get; }
        IList<SeaProvince> SeaProvinces { get; }
        List<MoveOrder.Step> CalculatePath(IMovable movable, Province destination);
        void ProcessWorld(List<VoronoiSite> sites);
    }
    class ProvinceManager : GameComponent, IProvinceManager, IDailyUpdatable
    {
        public IList<Province> Provinces { get; private set; }
        public IList<LandProvince> LandProvinces { get; private set; }
        public IList<SeaProvince> SeaProvinces { get; private set; }

        public ProvinceManager(IGame game) 
            : base(game, 2, false)
        {
            Provinces = new List<Province>();
        }

        #region Savable
        //public int SaveOrder { get { return 1; } }
        //public string SaveName { get { return "Provinces"; } }

        //public void Save(XmlWriter writer)
        //{
        //    foreach (Province province in Provinces)
        //        province.Save(writer);
        //}

        //public void Load(SaveGame save)
        //{
        //    save.LoadCollection(Provinces, Province.FromXml, "Province");
        //}

        //public void PreLoading()
        //{
        //    Provinces.Clear();
        //}

        //public void PostLoading() { }
        #endregion

        public override void Initialize(Action<string> feedback)
        {
            feedback("Loading provinces...");
        }

        public override void Update(double delta)
        {}

        public void DayUpdate()
        {
            foreach (Province province in Provinces)
                province.DayUpdate();
        }

        public override void Dispose()
        { }

        public void RegisterAsService()
        {
            Game.Services.Register<IProvinceManager>(this);
        }

        public List<MoveOrder.Step> CalculatePath(IMovable movable, Province destination)
        {
            if (movable.Location == destination || !movable.CanCross(destination))
                return null;
            List<MoveOrder.Step> steps = new List<MoveOrder.Step>();
            //Calculate path using A* algorithm
            SortedSet<PathfindingNode> openList = new SortedSet<PathfindingNode>(
                Comparer<PathfindingNode>.Create((a, b) => a.CompareTo(b)));
            HashSet<Province> closedList = new HashSet<Province>();
            openList.Add(new PathfindingNode(movable.Location, Vector3.Distance(destination.Center, movable.Location.Center)));
            bool pathFound = destination == movable.Location;
            while (!pathFound)
            {
                if (openList.Count == 0)
                    break;
                PathfindingNode currentNode = openList.First();
                foreach (Province neighbourg in currentNode.Province.Adjacencies.Select(a=>a.Neighbourg).Where(s => movable.CanCross(s)))
                {
                    if (closedList.Contains(neighbourg) || openList.Any(n => n.Province == neighbourg))
                        continue;
                    openList.Add(new PathfindingNode(neighbourg, Vector3.Distance(destination.Center, neighbourg.Center), currentNode));
                    if (neighbourg == destination) // Path found !
                    {
                        pathFound = true;
                        steps.Add(new MoveOrder.Step(destination, (int)(Vector3.Distance(currentNode.Province.Center, destination.Center) / movable.Speed)));
                        while (currentNode.Parent != null)
                        {
                            steps.Add(new MoveOrder.Step(currentNode.Province, (int)(Vector3.Distance(currentNode.Province.Center, currentNode.Parent.Province.Center) / movable.Speed)));
                            currentNode = currentNode.Parent;
                        }
                        steps.Reverse();
                        break;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode.Province);
            }
            return steps;
        }

        public void ProcessWorld(List<VoronoiSite> sites)
        {
            Provinces = new List<Province>();
            foreach (VoronoiSite site in sites)
            {
                Province province;
                if (site.IsWater)
                    province = new SeaProvince(new List<Zone>(new[] { (Zone)site }));
                else
                    province = new LandProvince(new List<Zone>(new[] { (Zone)site }));
                Provinces.Add(province);
                site.ProvinceId = province.Id;
            }
            foreach (VoronoiSite site in sites)
            {
                Province province = Provinces.Single(p => p.Id == site.ProvinceId);
                foreach (VoronoiSite neighbourg in site.Neighbourgs)
                {
                    Province neighbourgProvince = Provinces.Single(p => p.Id == neighbourg.ProvinceId);
                    List<Vector3> commonPoints = province.Zones.SelectMany(z => z.Points).
                        Where(p => neighbourgProvince.Zones.SelectMany(z => z.Points).Contains(p)).ToList();
                    province.Adjacencies.Add(new ProvinceAdjacency()
                    {
                        Neighbourg = neighbourgProvince,
                        PassingPoints = new List<Vector3>() 
                        { 
                            commonPoints.Aggregate(new Vector3(), (total, vector) => total + vector)/commonPoints.Count
                        }
                    });
                }
            }
            LandProvinces = Provinces.OfType<LandProvince>().ToList();
            SeaProvinces = Provinces.OfType<SeaProvince>().ToList();
        }
    }
}
