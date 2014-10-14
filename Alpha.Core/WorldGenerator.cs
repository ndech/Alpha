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
            Int32 width = 2000;
            Int32 height = 1000;
            feedback("Generating base shapes");
            world.Size = new Vector2I(width, height);
            List<VoronoiSite> sites = Generator.Create(width, height, 3000, 1, 1256);
            feedback("Creating provinces");
            foreach (VoronoiSite site in sites)
            {
                Province province;
                if (site.IsWater)
                    province = new SeaProvince(world, new List<Zone>(new[] { (Zone)site }));
                else
                    province = new LandProvince(world, new List<Zone>(new[] { (Zone)site }));
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
    }
}
