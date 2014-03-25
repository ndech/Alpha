namespace Alpha
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    interface ICharacterList : IService
    {
        IEnumerable<Character> Characters { get; }
    }
    class CharacterList : GameComponent, ISavable, ICharacterList
    {

        IEnumerable<Character> ICharacterList.Characters { get { return Characters; } }
        private ICollection<Character> Characters { get; set; }

        public CharacterList(Game game) 
            : base(game, 0)
        {
            Characters = new List<Character>();
            game.Services.AddService<ICharacterList>(this);

            for (int i = 0; i < 10; i++)
                Characters.Add(new Character());
        }

        #region ISavable
        public int SaveOrder
        {
            get { return 0; }
        }

        public string SaveName
        {
            get { return "Characters"; }
        }

        public void Save(XmlWriter writer)
        {
            foreach (Character character in Characters)
                character.Save(writer);
        }

        public void Load(SaveGame save)
        {
            save.LoadCollection(Characters, Character.FromXml, "Character");
            if (Characters.GroupBy(x => x.Id).Count(g => g.Count() > 1) > 0)
                throw new InvalidOperationException("Duplicates character id in save");
        }
        #endregion

        public override void Update(double delta)
        { }

        public override void Dispose()
        { }
    }
}
