using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Events;
using Alpha.Scripting;

namespace Alpha
{
    interface IRealmManager : IService
    {
        Realm PlayerRealm { get; }
        IList<Realm> Realms { get; }
    }
    class RealmManager : GameComponent, IRealmManager, IDailyUpdatable
    {
        public Realm PlayerRealm { get; private set; }
        public IList<Realm> Realms { get; private set; }
        public IList<Event<IScriptableRealm>> Events { get; private set; }

        public RealmManager(IGame game) : base(game, 3)
        {
            Realms = new List<Realm>();
        }

        public void RegisterAsService()
        {
            Game.Services.AddService<IRealmManager>(this);
        }

        public override void Initialize()
        {
            IEnumerable<Character> characters = Game.Services.GetService<ICharacterManager>().Characters;
            Realms.Add(new Realm("Belgium", characters.Where((c)=> c.Realm == null).RandomItem()));
            PlayerRealm = Realms[0];
            Realms.Add(new Realm("France", characters.Where((c) => c.Realm == null).RandomItem()));
            Realms.Add(new Realm("Romania", characters.Where((c) => c.Realm == null).RandomItem()));
            Realms.Add(new Realm("Spain", characters.Where((c) => c.Realm == null).RandomItem()));
            Realms.Add(new Realm("Italy", characters.Where((c) => c.Realm == null).RandomItem()));
            Realms.Add(new Realm("Croatia", characters.Where((c) => c.Realm == null).RandomItem()));
            Realms[1].Liege = Realms[0];
            Realms[4].Liege = Realms[0];
            Realms[5].Liege = Realms[0];
            int i = 0;
            foreach (Province province in Game.Services.GetService<IProvinceManager>().Provinces)
            {
                if(i< Realms.Count)
                    Realms[i].Demesne.Add(province);
                else
                    Realms.RandomItem().Demesne.Add(province);
                i++;
            }
            foreach (Realm realm in Realms)
                realm.TaxRate = (float)Random.Generator.Get(10, 40)/100;
            Events = Game.Services.GetService<IEventManager>().LoadEvents<IScriptableRealm>(Realm.ScriptIdentifier);
        }

        public override void Update(double delta)
        { }

        public override void Dispose()
        { }

        public void DayUpdate()
        {
            foreach (Realm realm in Realms)
                realm.DayUpdate();
            foreach (Realm realm in Realms)
                foreach (Event<IScriptableRealm> @event in Events)
                {
                    Console.Write(@event.LabelFunc.Invoke(realm));
                    if (@event.ConditionsValid(realm))
                    {
                        Console.Write("\tValid");
                        Console.Write("\t {0}", @event.MeanTimeToHappen(realm));
                    }
                    else
                        Console.Write("\tInvalid");
                    Console.WriteLine();
                    if (@event.ConditionsValid(realm))
                        @event.Process(realm);
                }
            Console.WriteLine();
        }
    }
}
