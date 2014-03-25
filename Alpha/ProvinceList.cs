namespace Alpha
{
    using System.Collections.Generic;
    using System.Xml;
    interface IProvinceList : IService
    {
        IEnumerable<Province> Provinces { get; }  
    }
    class ProvinceList : GameComponent, ISavable, IProvinceList
    {
        IEnumerable<Province> IProvinceList.Provinces { get { return Provinces; } } 
        private ICollection<Province> Provinces { get; set; }

        public ProvinceList(Game game) 
            : base(game, 0)
        {
            Provinces = new List<Province>();
            game.Services.AddService<IProvinceList>(this);

            for (int i = 0; i < 10; i++)
                Provinces.Add(new Province());
        }

        #region Savable
        public int SaveOrder
        {
            get { return 0; }
        }

        public string SaveName
        {
            get { return "Provinces"; }
        }

        public void Save(XmlWriter writer)
        {
            foreach (Province province in Provinces)
                province.Save(writer);
        }

        public void Load(SaveGame save)
        {
            save.LoadCollection(Provinces, Province.FromXml, "Province");
        }
        #endregion

        public override void Update(double delta)
        {

        }

        public override void Dispose()
        { }
    }
}
