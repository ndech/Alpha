using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    [ScriptName("Province")]
    public class LandProvince : Province, IScriptLandProvinceForResourcesGeneration
    {
        public LandProvince(World world, Zone zone) : base(world, zone)
        {
            Name = NameGenerator.GetRandomProvinceName();
            Color = CustomColor.Random;
            Capital = new Settlement(world, this);
        }
        
        protected override void DayUpdate()
        {
            Capital.DayUpdate();
            Resources.DayUpdate();
        }

        public override sealed string Name { get; internal set; }
        protected override string IdPrefix => "land_province";
        public Realm Owner { get; internal set; }
        public CustomColor Color { get; internal set; }
        public Settlement Capital { get; }

        private readonly List<Resource> _resources = new List<Resource>(); 
        public IEnumerable<Resource> Resources => _resources;

        internal void AddResource(ResourceType type, ResourceLevel level)
        {
            _resources.Add(new Resource(type, level));
        }
        
        public int FoodPotential()
        {
            return Resources.Where(r => r.Type.Category == ResourceType.ResourceCategory.Food)
                .Sum(r => r.Level.Value);
        }

        public bool HasResource(string key) => Resources.Any(r => r.Type.Id == key);
        public bool IsCoastal => Zones.SelectMany(z => z.Neighbourgs).Any(z => z.IsWater);
        public int Surface => (int)Zones.Sum(z => z.Surface);
    }
}