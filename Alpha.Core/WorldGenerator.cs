using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Common;
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
            feedback("Generating base shapes");
            World world = new World(_dailyNotifications, _liveNotifications, _dataLock);
            Int32 width = 2000;
            Int32 height = 1000;
            world.Size = new Vector2I(width, height);
            List<VoronoiSite> sites = Generator.Create(width, height, 1000, 1, 1256);
            feedback("Creating provinces");
            foreach (VoronoiSite site in sites)
            {
                Province province;
                if (site.IsWater)
                    province = new SeaProvince(new List<Zone>(new[] { (Zone)site }));
                else
                    province = new LandProvince(new List<Zone>(new[] { (Zone)site }));
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
            feedback("Forging realms");
            for (int i = 0; i < 10; i++)
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
