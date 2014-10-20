using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Common;
using Alpha.Core.Dynamic;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using Alpha.WorldGeneration;

class ZoneCluster : List<Zone>
{
    public bool Blocked { get; set; }
}

namespace Alpha.Core
{
    public class WorldGenerator : IWorldGenerator
    {
        private readonly List<Notification> _dailyNotifications;
        private readonly List<Notification> _liveNotifications;
        private readonly DataLock _dataLock;

        public WorldGenerator(List<Notification> dailyNotifications, List<Notification> liveNotifications, DataLock dataLock)
        {
            _dailyNotifications = dailyNotifications;
            _liveNotifications = liveNotifications;
            _dataLock = dataLock;
        }

        IProcessableWorld IWorldGenerator.Generate(Action<String> feedback)
        {
            feedback("Initializing Script Engine");
            Engine.Execute<bool>("true",Engine.NewSession);
            feedback("Creating world");
            World world = new World(_dailyNotifications, _liveNotifications, _dataLock);
            Int32 width = ConfigurationManager.Config.WorldWidth;
            Int32 height = ConfigurationManager.Config.WorldHeight;
            Int32 zoneCount = ConfigurationManager.Config.NumberOfRegions;
            feedback("Generating base shapes");
            world.Size = new Vector2I(width, height);
            List<VoronoiSite> sites = Generator.Create(width, height, zoneCount, 1, 1256);
            feedback("Dividing the world");
            Dictionary<string, Zone> zones = new Dictionary<string, Zone>();
            foreach (VoronoiSite site in sites)
            {
                Zone zone = (Zone) site;
                zones.Add(zone.Id, zone);
                site.ZoneId = zone.Id;
            }
            feedback("Meeting new neighbourgs");
            foreach (VoronoiSite site in sites)
            {
                Zone zone = zones[site.ZoneId];
                foreach (VoronoiSite neighbourgSite in site.Neighbourgs)
                {
                    Zone neighbourg = zones[neighbourgSite.ZoneId];
                    List<Vector3D> commonPoints = zone.Points.Where(p => neighbourg.Points.Contains(p)).ToList();
                    if (commonPoints.Count != 2)
                        continue;
                    zone.AddAdjacency(new ZoneAdjacency(zone, neighbourg, commonPoints));
                }
            }
            feedback("Expanding provinces");
            foreach (int clusterId in sites.Where(s=>!s.IsWater).Select(s=>s.Cluster.Id).Distinct())
            {
                const int targetSize = 7;
                List<Zone> todo = sites.Where(s=>s.Cluster.Id == clusterId).Select(s=>zones[s.ZoneId]).OrderByRandom().ToList();
                int numberOfClusters = (todo.Count/targetSize)+1;
                List<ZoneCluster> clusters = new List<ZoneCluster>();
                //First clusterisation
                for (int i = 0; i < numberOfClusters; i++)
                {
                    Zone seed = todo.RandomItem();
                    clusters.Add(new ZoneCluster {seed});
                    todo.Remove(seed);
                }
                while(todo.Count!=0)
                {
                    foreach (ZoneCluster cluster in clusters)
                    {
                        Zone neighbourg =
                            todo.FirstOrDefault(
                                z => cluster.SelectMany(c => c.Adjacencies.Select(a => a.Neighbourg)).Contains(z));
                        if (neighbourg == null)
                            continue;
                        cluster.Add(neighbourg);
                        todo.Remove(neighbourg);
                    }
                }
                //Optimization to try to reach the optimal number of zones per cluster
                int optimumFunction;
                do
                {
                    optimumFunction = clusters.Sum(c => c.Count * c.Count);
                    foreach (ZoneCluster currentCluster in clusters.OrderByDescending(c=>c.Count))
                    {
                        List<ZoneCluster> neighbourgClusters = clusters
                            .Where(
                                c =>
                                    c.Any(p => currentCluster.SelectMany(x => x.Neighbourgs).Contains(p)) &&
                                    c != currentCluster)
                            .OrderBy(c => c.Count)
                            .ToList();
                        neighbourgClusters = neighbourgClusters.Where(c => c.Count < currentCluster.Count).ToList();
                        foreach (ZoneCluster neighbourgCluster in neighbourgClusters)
                        {
                            if(neighbourgCluster.Count >= currentCluster.Count)
                                continue;
                            foreach (Zone switchingZone in 
                                currentCluster
                                .Where(p => neighbourgCluster.SelectMany(x => x.Neighbourgs).Contains(p))
                                .OrderByRandom())
                            {
                                if(currentCluster.Count == 0)
                                    break;
                                if (IsContiguousCluster(currentCluster, switchingZone))
                                {
                                    neighbourgCluster.Add(switchingZone);
                                    currentCluster.Remove(switchingZone);
                                    break;
                                }
                            }
                        }
                    }
                } while (optimumFunction != clusters.Sum(c => c.Count * c.Count));
                //Actual creation of the provinces
                foreach (ZoneCluster cluster in clusters)
                    world.ProvinceManager.CreateProvince(new LandProvince(world, cluster));
            }
            feedback("Observing the waves");
            foreach (VoronoiSite site in sites.Where(s => s.IsWater))
                world.ProvinceManager.CreateProvince(new SeaProvince(world, new List<Zone>{zones[site.ZoneId]}));
            feedback("Planting fields");
            foreach (Settlement settlement in world.ProvinceManager.LandProvinces.SelectMany(p=>p.AllSettlements))
            {
                int numberOfResources = RandomGenerator.Get(1, 4);
                for (int i = 0; i < numberOfResources; i++)
                    settlement.AddResource(
                        world.ProvinceManager.ResourceTypes
                        .Except(settlement.Resources.Select(r=>r.Type))
                        .RandomWeightedItem(r=>r.Probability.For(settlement)));
            }
            feedback("Forging realms");
            for (int i = 0; i < 11; i++)
                world.RealmManager.CreateRealm(new Realm("Realm " + i));
            feedback("Spreading realms");
            List<Tuple<Vector3D, Realm>> realmPosition = 
                world.ProvinceManager.LandProvinces.OrderByRandom()
                .Take(world.RealmManager.Realms.Count())
                .Select(p => p.Center)
                .Zip(world.RealmManager.Realms, (v, r) => new Tuple<Vector3D, Realm>(v, r)).ToList();
            foreach (LandProvince province in world.ProvinceManager.LandProvinces)
                realmPosition.MinBy(t=> Vector3D.Distance(province.Center, t.Item1)).Item2.AddProvince(province);
            feedback("Launching the fleets");
            foreach (Realm realm in world.RealmManager.Realms)
                world.FleetManager.CreateFleet(new Fleet(world, "Royal fleet of "+realm.Name, realm, world.ProvinceManager.SeaProvinces.RandomItem().Zones.RandomItem(), new List<Ship>{new Ship()}));
            feedback("Polishing");
            return world;
        }

        private bool IsContiguousCluster(IEnumerable<Zone> currentCluster, Zone switchingZone)
        {
            List<Zone> newCluster = new List<Zone>(currentCluster);
            newCluster.Remove(switchingZone);
            if (newCluster.Count == 0)
                return false;
            List<Zone> check = new List<Zone> { newCluster.RandomItem() };
            while (true)
            {
                var x = check.SelectMany(p => p.Neighbourgs)
                    .Where(newCluster.Contains).Where(b => !check.Contains(b)).Distinct().ToList();
                if (!x.Any())
                    break;
                check.AddRange(x);
            }
            return check.Count == newCluster.Count;
        }
    }
}
