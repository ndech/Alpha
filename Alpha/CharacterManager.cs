namespace Alpha
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    interface ICharacterManager : IService
    {
        IEnumerable<Character> Characters { get; }
    }
    class CharacterManager : GameComponent, ISavable, ICharacterManager
    {
        IEnumerable<Character> ICharacterManager.Characters { get { return Characters; } }
        private ICollection<Character> Characters { get; set; }

        public CharacterManager(IGame game) 
            : base(game, 1, false)
        {
            Characters = new List<Character>();
        }

        #region ISavable
        public int SaveOrder { get { return 0; } }

        public string SaveName { get { return "Characters"; } }

        public void Save(XmlWriter writer)
        {
            foreach (Character character in Characters)
                character.Save(writer);
        }

        public void PreLoading()
        {
            Characters.Clear();
        }

        public void Load(SaveGame save)
        {
            save.LoadCollection(Characters, Character.FromXml, "Character");
        }

        public void PostLoading()
        {
            if (Characters.GroupBy(x => x.Id).Count(g => g.Count() > 1) > 0)
                throw new InvalidOperationException("Duplicates character id in save");
        }
        #endregion

        public override void Initialize(Action<string> feedback)
        {
            feedback.Invoke("Loading characters...");
            for (int i = 0; i < 10; i++)
                Characters.Add(new Character());
        }

        public override void Update(double delta)
        { }

        public override void Dispose()
        { }

        public void RegisterAsService()
        {
            Game.Services.Register<ICharacterManager>(this);
        }
    }
}
