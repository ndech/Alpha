using System;

namespace Alpha
{
    using System.Collections.Generic;
    using System.Xml;
    interface IProvinceManager : IService
    {
        IList<Province> Provinces { get; }  
    }
    class ProvinceManager : GameComponent, ISavable, IProvinceManager, IDailyUpdatable
    {
        public IList<Province> Provinces { get; protected set; }

        public ProvinceManager(IGame game) 
            : base(game, 2, false)
        {
            Provinces = new List<Province>();
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

        public override void Initialize(Action<string> feedback)
        {
            feedback.Invoke("Loading provinces...");
            for (int i = 0; i < 10; i++)
                Provinces.Add(new Province());
        }

        public override void Update(double delta)
        {}

        public void DayUpdate()
        {
            foreach (Province province in Provinces)
            {
                province.DayUpdate();
            }
        }

        public override void Dispose()
        { }

        public void RegisterAsService()
        {
            Game.Services.Register<IProvinceManager>(this);
        }
    }
}
