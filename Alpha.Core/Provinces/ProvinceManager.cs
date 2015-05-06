using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Buildings;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class ProvinceManager : Manager
    {
        public IReadOnlyCollection<Province> Provinces { get; private set; }
        public IReadOnlyCollection<SeaProvince> SeaProvinces { get; private set; }
        public IReadOnlyCollection<LandProvince> LandProvinces { get; private set; }

        internal List<BaseSettlementType> BaseSettlementTypes { get; private set; }
        internal List<ResourceLevel> ResourceLevels { get; private set; } 

        internal ProvinceManager(World world) : base(world)
        {
            ResourceTypes.Initialize();
            BuildingStatuses.Initialize();
            BuildingTypes.Initialize();

            BaseSettlementTypes = XDocument.Load(@"Data\Settlements\Settlements.xml")
                .Descendants("baseSettlements").Descendants("settlement").Select(BaseSettlementType.Create).ToList();
            
            ResourceLevels = XDocument.Load(@"Data\Resources\ResourceLevels.xml").Descendants("resourceLevel").Select(x => new ResourceLevel(x)).ToList();
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            Provinces.DayUpdate(dataLock);
        }

        public void LoadProvinces(IEnumerable<Province> provinces)
        {
            Provinces = provinces.ToReadOnly();
            SeaProvinces = Provinces.OfType<SeaProvince>().ToReadOnly();
            LandProvinces = Provinces.OfType<LandProvince>().ToReadOnly();
            ProvincePicker.Initialize(Provinces.First().Zones.First());
        }
    }
}
