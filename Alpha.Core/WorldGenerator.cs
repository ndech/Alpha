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

        public WorldGenerator(List<Notification> dailyNotifications, List<Notification> liveNotifications)
        {
            _dailyNotifications = dailyNotifications;
            _liveNotifications = liveNotifications;
        }

        IProcessableWorld IWorldGenerator.Generate(Action<String> feedback)
        {
            feedback("Generating base shapes");
            World world = new World(_dailyNotifications, _liveNotifications);
            List<VoronoiSite> sites = Generator.Create(2000, 1000, 1000, 1, 1256);
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
                    province.CreateAdjacency(new ProvinceAdjacency
                    {
                        Neighbourg = neighbourgProvince,
                        PassingPoints = new List<Vector3D>
                        { 
                            commonPoints.Aggregate(new Vector3D(), (total, vector) => total + vector)/commonPoints.Count
                        }
                    });
                }
            }
            feedback("Dividing in realms");
            for (int i = 0; i < 10; i++)
                world.RealmManager.CreateRealm(new Realm("Realm "+i));
            foreach (LandProvince province in world.ProvinceManager.LandProvinces)
                world.RealmManager.Realms.RandomItem().AddProvince(province);
            feedback("Launching the fleets");
            foreach (Realm realm in world.RealmManager.Realms)
                world.FleetManager.CreateFleet(new Fleet("Royal fleet of "+realm.Name, realm, world.ProvinceManager.SeaProvinces.RandomItem(), new List<Ship>{new Ship()}));
            feedback("Polishing");
            return world;
        }
    }
}
