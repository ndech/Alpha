using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Alpha.Scripting;
using Alpha.Toolkit;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Alpha.Events
{
    interface IEventManager : IService
    {
        IList<Event<T>> LoadEvents<T>() where T : IEventable;
    }
    class EventManager : GameComponent, IEventManager
    {
        private readonly ScriptEngine _engine;
        private ScriptContext _scriptContext;
        private ScriptContext ScriptContext
        {
            get { return _scriptContext ?? (_scriptContext = new ScriptContext(Game.Services.GetService<ICalendar>())); }
        }
    
        private Session NewSession { 
            get 
            { 
                Session session = Session.Create(ScriptContext);
                session.AddReference(typeof(ScriptContext).Assembly);
                return session;
            } 
        }

        public EventManager(IGame game) : base(game)
        {
            _engine = new ScriptEngine();
        }
        
        public IList<Event<T>> LoadEvents<T>() where T : IEventable
        {
            IList<Event<T>> list = new List<Event<T>>();

            foreach (String file in Directory.GetFiles("Data/Events/"))
            {
                XDocument document = XDocument.Load(file);
                foreach (XElement xmlEvent in document.Descendants("event"))
                {
                    String id = (String) xmlEvent.Attribute("id").Mandatory("An event without id is defined in file "+file);
                    XElement xMtth = xmlEvent.Element("meanTimeToHappen").Mandatory("No mean time to happen defined for event " + id + " in file " + file);
                    XElement xOutcomes = xmlEvent.Element("outcomes").Mandatory("No outcomes defined for event " + id + " in file " + file);
                    if (xOutcomes.Elements("outcome").All(element => element.Element("conditions") != null))
                        throw new InvalidEventDataException("At least one outcome with no condition must be defined for event " + id + " in file " + file);
                    //Prepare session
                    Session sharedSession = NewSession;
                    _engine.Execute("using Alpha.Scripting;", sharedSession);
                    XElement xParameters = xOutcomes.Element("parameters");
                    if (xParameters != null) _engine.Execute(xParameters.Value, sharedSession);
                    //Create the new event
                    list.Add(new Event<T>(id)
                    {
                        IsTriggeredOnly = xmlEvent.Attribute("triggered-only") != null &&
                                          xmlEvent.Attribute("triggered-only").Value.Equals("true"),
                        LabelFunc =
                            LoadStringGenerator<T>(xmlEvent.Element("label"), NewSession,
                                "No label defined for event " + id + " in file " + file),
                        Modifiers = LoadModifiers<T>(xMtth.Element("modifiers"), NewSession),
                        BaseMeanTimeToHappen = TimeSpanParser.Parse(xMtth.Element("base").Mandatory("No base mean time to happen defined for event "+id+" in file "+file).Value),
                        Conditions = LoadConditions<T>(xmlEvent.Element("conditions"), NewSession),
                        PreExecute = LoadPreExecute<T>(xmlEvent.Element("preExecute"), sharedSession),
                        Outcomes = xOutcomes.Elements("outcome").Select(xmlOutcome => new Outcome<T>
                        {
                            LabelFunc =
                                LoadStringGenerator<T>(xmlOutcome.Element("label"), sharedSession,
                                    "No label defined for an outcome of event " + id + " in file " + file),
                            TooltîpFunc = LoadStringGenerator<T>(xmlOutcome.Element("tooltip"), sharedSession),
                            PreExecute = LoadPreExecute<T>(xmlOutcome.Element("preExecute"), sharedSession),
                            Conditions = LoadConditions<T>(xmlOutcome.Element("conditions"), sharedSession),
                            Effects = LoadEffects<T>(xmlOutcome.Element("effects"), sharedSession)
                        }).ToList()
                    });
                }
            }
            return list;
        }

        private IList<Action<T>> LoadEffects<T>(XElement element, Session sharedSession)
        {
            IList<Action<T>> effects = new List<Action<T>>();
            if (element != null)
                foreach (XElement xmlEffect in element.Elements("effect"))
                    effects.Add(_engine.Execute<Action<T>>("(Realm) => " + xmlEffect.Value, sharedSession));
            return effects;
        }

        private Action<T> LoadPreExecute<T>(XElement element, Session session)
        {
            if (element == null) return null;
            return _engine.Execute<Action<T>>("(Realm) => " + element.Value, session);
        }

        private IList<Func<T, bool>> LoadConditions<T>(XElement element, Session session)
        {
            IList<Func<T,Boolean>> conditions = new List<Func<T, bool>>();
            if (element != null)
                foreach (XElement xmlCondition in element.Elements("condition"))
                    conditions.Add(_engine.Execute<Func<T, Boolean>>("(Realm) => " + xmlCondition.Value, session));
            return conditions;
        }


        private Func<T, string> LoadStringGenerator<T>(XElement xString, Session session, String exceptionMessage = null) where T : IEventable
        {
            if (exceptionMessage == null && xString == null)
                return null;
            String label = (String) xString.Mandatory(exceptionMessage);
            return _engine.Execute<Func<T, String>>("(Realm) => { return \"" + label.Replace("{", "\"+").Replace("}", "+\"") + "\";}", session);
        }

        private IList<IModifier<T>> LoadModifiers<T>(XElement xModifiers, Session session) where T : IEventable
        {
            if (xModifiers == null) return null;
            IList<IModifier<T>> modifiers = new List<IModifier<T>>();
            foreach (XElement xmlModifier in xModifiers.Elements())
            {
                ModifierType type;
                if (xmlModifier.Name == "multiplier")
                    type = ModifierType.Multiplier;
                else if (xmlModifier.Name == "reducer")
                    type = ModifierType.Reducer;
                else
                    continue;

                if (xmlModifier.Attribute("factor") == null)
                    modifiers.Add(new DynamicModifier<T>(
                        _engine.Execute<Func<T, Double>>("(Realm) => " + xmlModifier.Value, session), type));
                else
                    modifiers.Add(new StaticModifier<T>(Double.Parse(xmlModifier.Attribute("factor").Value),
                        _engine.Execute<Func<T, Boolean>>("(Realm) => " + xmlModifier.Value, session), type));
            }
            return modifiers;
        }

        public override void Initialize()
        {}

        public override void Update(double delta)
        {}
        public override void Dispose()
        {}

        public void RegisterAsService()
        {
            Game.Services.AddService<IEventManager>(this);
        }
    }
}
