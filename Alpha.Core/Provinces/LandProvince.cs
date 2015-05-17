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
        protected override string IdPrefix
        {
            get { return "land_province"; }
        }
        public Realm Owner { get; internal set; }
        public CustomColor Color { get; internal set; }
        public Settlement Capital { get; internal set; }

        private readonly List<Resource> _resources = new List<Resource>(); 
        public IEnumerable<Resource> Resources { get { return _resources; } }

        internal void AddResource(ResourceType type, ResourceLevel level)
        {
            _resources.Add(new Resource(type, level));
        }

        public bool HasResource(string key)
        {
            return Resources.Any(r => r.Type.Id == key);
        }

        public int FoodPotential()
        {
            return Resources.Where(r => r.Type.Category == ResourceType.ResourceCategory.Food)
                .Sum(r => r.Level.Value);
        }

        public bool IsCoastal { get { return Zones.SelectMany(z => z.Neighbourgs).Any(z => z.IsWater); } }
        public int Surface { get { return (int)Zones.Sum(z => z.Surface); } }
    }
}