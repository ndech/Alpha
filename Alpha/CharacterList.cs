namespace Alpha
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    class CharacterList : GameComponent, ISavable
    {
        public List<Character> Characters { get; set; }

        public CharacterList(Game game) 
            : base(game, 0)
        {
            Characters = new List<Character>();

            for (int i = 0; i < 10; i++)
                Characters.Add(new Character());
        }

        #region Savable
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

        public void Load(XmlReader reader)
        {
            SaveGame.LoadCollection(reader, Characters, Character.FromXml, "Character");
            if (Characters.GroupBy(x => x.Id).Count(g => g.Count() > 1) > 0)
                throw new InvalidOperationException("Duplicates character id in save");
        }
        #endregion


        public override void Update(double delta)
        {

        }

        public override void Dispose()
        { }
    }
}
