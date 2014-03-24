using System.Collections.Generic;
using System.Xml;

namespace Alpha
{
    interface IProvinceList : IService
    {
        ICollection<Province> Provinces { get; }  
    }
    class ProvinceList : GameComponent, ISavable, IProvinceList
    {
        public ICollection<Province> Provinces { get; private set; }

        public ProvinceList(Game game) 
            : base(game, 0)
        {
            Provinces = new List<Province>();
            game.Services.AddService(typeof(IProvinceList), this);

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

        public void Load(XmlReader reader)
        {
            //SaveGame.LoadCollection(reader, Provinces, Province.FromXml);
        }
        #endregion


        public override void Update(double delta)
        {

        }

        public override void Dispose()
        { }
    }
}
