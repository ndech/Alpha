namespace Alpha
{
    using System.Collections.Generic;
    using System.Xml;
    interface IProvinceList : IService
    {
        IList<Province> Provinces { get; }  
    }
    class ProvinceList : GameComponent, ISavable, IProvinceList, IDailyUpdatable
    {
        public IList<Province> Provinces { get; protected set; }
        private IList<ProvinceEvent> Events; 

        public ProvinceList(IGame game) 
            : base(game, 0)
        {
            Provinces = new List<Province>();
            Events = new List<ProvinceEvent>();
        }

        #region Savable
        public int SaveOrder { get { return 1; } }
        public string SaveName { get { return "Provinces"; } }

        public void Save(XmlWriter writer)
        {
            foreach (Province province in Provinces)
                province.Save(writer);
        }

        public void Load(SaveGame save)
        {
            save.LoadCollection(Provinces, Province.FromXml, "Province");
        }

        public void PreLoading()
        {
            Provinces.Clear();
        }

        public void PostLoading() { }
        #endregion

        public override void Initialize()
        {
            for (int i = 0; i < 10; i++)
                Provinces.Add(new Province());
            Events.Add(new ProvinceEvent());
        }

        public override void Update(double delta)
        {}

        public void DayUpdate()
        {
            foreach (Province province in Provinces)
            {
                province.DayUpdate();
                foreach (ProvinceEvent provinceEvent in Events)
                    provinceEvent.Process(province);
            }
        }

        public override void Dispose()
        { }

        public void RegisterAsService()
        {
            Game.Services.AddService<IProvinceList>(this);
        }
    }
}
