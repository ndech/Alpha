using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpha.Core.Provinces
{
    public class ProvinceManager : IManager
    {

        private readonly List<Province> _provinces = new List<Province>();
        public IEnumerable<Province> Provinces { get { return _provinces; } }
        public IEnumerable<Province> SeaProvinces { get { return _provinces.OfType<SeaProvince>(); } }
        public IEnumerable<Province> LandProvinces { get { return _provinces.OfType<LandProvince>(); } }

        public Province GetById(String id)
        {
            return _provinces.Single(p => p.Id.Equals(id));
        }
        internal ProvinceManager()
        {
            
        }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            
        }

        void IManager.Setup()
        {
            
        }

        internal void CreateProvince(Province province)
        {
            _provinces.Add(province);
        }
    }
}
