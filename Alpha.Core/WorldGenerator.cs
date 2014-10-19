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

class ProvinceCluster : List<Province>
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
            feedback("Creating provinces");
            foreach (VoronoiSite site in sites)
            {
                Province province;
                if (site.IsWater)
                    province = new SeaProvince(world, new List<Zone>(new[] { (Zone)site }), site.Cluster);
                else
                    province = new LandProvince(world, new List<Zone>(new[] { (Zone)site }), site.Cluster);
                world.ProvinceManager.CreateProvince(province);
                site.ProvinceId = province.Id;
            }
            feedback("Calculating province adjacencies");
            foreach (VoronoiSite site in sites)
            {
                Province province = world.ProvinceManager.GetById(site.ProvinceId);
                foreach (VoronoiSite neighbourg in site.Neighbourgs)
                {
                    Province neighbourgProvince = world.ProvinceManager.GetById(neighbourg.ProvinceId);
                    List<Vector3D> commonPoints = province.Zones.SelectMany(z => z.Points).
                        Where(p => neighbourgProvince.Zones.SelectMany(z => z.Points).Contains(p)).ToList();
                    if (commonPoints.Count != 2)
                        continue;
                    province.CreateAdjacency(new ProvinceAdjacency(province, neighbourgProvince, commonPoints));
                }
            }
            feedback("Processing clusters");
            int minLimit = 6;
            int maxLimit = 9;
            foreach (Cluster cluster in world.ProvinceManager.Provinces.Select(p=>p.Cluster).Distinct().ToList())
            
            {
                List<Province> todo = new List<Province>(world.ProvinceManager.Provinces.Where(p=>p.Cluster == cluster).ToList());
                if(todo.Any(p=>p is SeaProvince))
                    continue;
                int todoCount = todo.Count;
                int numberOfClusters = (todoCount/7)+1;
                List<ProvinceCluster> clusters = new List<ProvinceCluster>();
                //First clusterisation
                for (int i = 0; i < numberOfClusters; i++)
                {
                    ProvinceCluster subList = new ProvinceCluster();
                    subList.Add(todo.RandomItem());
                    todo.Remove(subList.First());
                    while (subList.Count < 6)
                    {
                        var neighbourgs = subList.SelectMany(p => p.Adjacencies.Select(a => a.Neighbourg)).Where(todo.Contains).ToList();
                        if (neighbourgs.Count == 0)
                            break;
                        Province newProvince = neighbourgs.RandomItem();
                        subList.Add(newProvince);
                        todo.Remove(newProvince);
                    }
                    clusters.Add(subList);
                }
                //Map provinces not associated to an cluster => first step of the iteration
                while (todo.Count > 0)
                {
                    Province newProvince = todo.RandomItem();
                    if(!newProvince.Adjacencies.Any(a => clusters.SelectMany(c=>c.Select(l=>l)).Contains(a.Neighbourg)))
                        continue;
                    todo.Remove(newProvince);
                    List<ProvinceCluster> subsetClusters = clusters.Where(c => newProvince.Adjacencies.Select(a => a.Neighbourg).Any(c.Contains)).Distinct().ToList();
                    subsetClusters.RandomItem().Add(newProvince);
                }
                bool Changed = false;
                int OptimumFunction = clusters.Sum(c => c.Count*c.Count);
                do
                {
                    Changed = false;
                    OptimumFunction = clusters.Sum(c => c.Count * c.Count);
                    foreach (ProvinceCluster currentCluster in clusters.OrderByDescending(c=>c.Count))
                    {
                        List<ProvinceCluster> neighbourgClusters = clusters
                            .Where(
                                c =>
                                    c.Any(p => currentCluster.SelectMany(x => x.Neighbourgs).Contains(p)) &&
                                    c != currentCluster)
                            .OrderBy(c => c.Count)
                            .ToList();
                        neighbourgClusters = neighbourgClusters.Where(c => c.Count < currentCluster.Count).ToList();
                        foreach (ProvinceCluster neighbourgCluster in neighbourgClusters)
                        {
                            if(neighbourgCluster.Count >= currentCluster.Count)
                                continue;
                            foreach (Province switchingProvince in 
                                currentCluster
                                .Where(p => neighbourgCluster.SelectMany(x => x.Neighbourgs).Contains(p))
                                .OrderBy(c=>RandomGenerator.GetDouble(0,1)))
                            {
                                if(currentCluster.Count == 0)
                                    break;
                                if (IsContiguousCluster(currentCluster, switchingProvince))
                                {
                                    Changed = true;
                                    neighbourgCluster.Add(switchingProvince);
                                    currentCluster.Remove(switchingProvince);
                                    break;
                                }
                            }
                        }
                    }
                } while (OptimumFunction != clusters.Sum(c => c.Count * c.Count));
                Changed = false;
            }
            feedback("Planting fields");
            foreach (LandProvince province in world.ProvinceManager.LandProvinces)
            {
                int numberOfResources = RandomGenerator.Get(1, 3);
                List<Tuple<double, ResourceType>> resourceTuples =
                    world.ProvinceManager.ResourceTypes.Select(
                        r => new Tuple<double, ResourceType>(r.Probability.For(province), r)).ToList();
                for (; numberOfResources > 0; numberOfResources--)
                {
                    double cumulativeProbability = resourceTuples.Sum(r => r.Item1);
                    double position = RandomGenerator.GetDouble(0, cumulativeProbability);
                    Tuple<double, ResourceType> value = null;
                    double cursor = 0;
                    foreach (Tuple<double, ResourceType> resourceTuple in resourceTuples)
                    {
                        if (position >= cursor && position <= cursor + resourceTuple.Item1)
                        {
                            value = resourceTuple;
                            break;
                        }
                        cursor += resourceTuple.Item1;
                    }
                    value = value ?? resourceTuples.Last();
                    resourceTuples.Remove(value);
                    province.AddResource(value.Item2);
                }
            }
            feedback("Forging realms");
            for (int i = 0; i < 20; i++)
                world.RealmManager.CreateRealm(new Realm("Realm " + i));
            feedback("Spreading realms");
            List<Tuple<Vector3D, Realm>> realmPosition = 
                world.ProvinceManager.LandProvinces.OrderBy(r => RandomGenerator.GetDouble(0, 1000))
                .Take(world.RealmManager.Realms.Count())
                .Select(p => p.Center)
                .Zip(world.RealmManager.Realms, (v, r) => new Tuple<Vector3D, Realm>(v, r)).ToList();
            foreach (LandProvince province in world.ProvinceManager.LandProvinces)
                realmPosition.MinBy(t=> Vector3D.Distance(province.Center, t.Item1)).Item2.AddProvince(province);
            feedback("Launching the fleets");
            foreach (Realm realm in world.RealmManager.Realms)
                world.FleetManager.CreateFleet(new Fleet(world, "Royal fleet of "+realm.Name, realm, world.ProvinceManager.SeaProvinces.RandomItem(), new List<Ship>{new Ship()}));
            feedback("Polishing");
            return world;
        }

        private bool IsContiguousCluster(ProvinceCluster currentCluster, Province switchingProvince)
        {
            List<Province> newCluster = new List<Province>(currentCluster);
            newCluster.Remove(switchingProvince);
            if (newCluster.Count == 0)
                return false;
            List<Province> check = new List<Province>()
                    {
                        newCluster.RandomItem()
                    };
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
