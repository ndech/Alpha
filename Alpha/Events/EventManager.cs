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
    interface IEventManager : IService, IEventPropagator
    {
        IList<Event<T>> LoadEvents<T>(string scriptIdentifier, Action<string> feedback) where T : IEventable;
    }
    class EventManager : GameComponent, IEventManager, IDailyUpdatable
    {

        protected IDictionary<String, IEvent> Events { get; set; }
        public IList<DelayedEvent> DelayedEvents { get; protected set; } 
        private readonly ScriptEngine _engine;
        private ScriptContext _scriptContext;

        private ScriptContext ScriptContext
        {
            get
            {
                return _scriptContext ??
                       (_scriptContext =
                           new ScriptContext(
                               Game.Services.Get<ICalendar>(),
                               Game.Services.Get<IEventManager>(),
                               Game.Services.Get<IRealmManager>().PlayerRealm,
                               Game.Services.Get<IRealmManager>().Realms.Cast<IScriptableRealm>().ToList(),
                               Game.Services.Get<IFleetManager>()));
            }
        }

        private Session NewSession
        {
            get
            {
                Session session = Session.Create(ScriptContext);
                session.AddReference(typeof(ScriptContext).Assembly);
                return session;
            }
        }

        public EventManager(IGame game)
            : base(game, 0, false)
        {
            _engine = new ScriptEngine();
            DelayedEvents = new List<DelayedEvent>();
            Events = new Dictionary<string, IEvent>();
        }

        public IList<Event<T>> LoadEvents<T>(string scriptIdentifier, Action<string> feedback) where T : IEventable
        {
            IList<Event<T>> list = new List<Event<T>>();

            foreach (String file in Directory.GetFiles("Data/Events/"))
            {
                XDocument document = XDocument.Load(file);
                foreach (XElement xmlEvent in document.Descendants("event"))
                {
                    String id = (String)xmlEvent.Attribute("id").Mandatory("An event without id is defined in file " + file);
                    feedback("Compiling "+scriptIdentifier.ToLower()+" events ... ("+id+")");
                    XElement xMtth = xmlEvent.Element("meanTimeToHappen").Mandatory("No mean time to happen defined for event " + id + " in file " + file);
                    XElement xOutcomes = xmlEvent.Element("outcomes").Mandatory("No outcomes defined for event " + id + " in file " + file);
                    if (xOutcomes.Elements("outcome").All(element => element.Element("conditions") != null))
                        throw new InvalidEventDataException("At least one outcome with no condition must be defined for event " + id + " in file " + file);
                    //Prepare session
                    Session sharedSession = NewSession;
                    _engine.Execute("using Alpha.Scripting;", sharedSession);
                    _engine.Execute("using System;", sharedSession);
                    //Load variables in session
                    XElement xVariables = xOutcomes.Element("variables");
                    if (xVariables != null) _engine.Execute(xVariables.Value, sharedSession);
                    //Load parameters in session
                    XElement xInput = xmlEvent.Element("parameters");
                    if(xInput != null) 
                        xInput.Elements("parameter").ToList().ForEach((param)=>
                            _engine.Execute(param.Attribute("type").Value + " " + param.Value + ";", sharedSession));
                    //Create the new event
                    list.Add(new Event<T>(id)
                    {
                        IsTriggeredOnly = xmlEvent.Attribute("triggered-only") != null &&
                                          xmlEvent.Attribute("triggered-only").Value.Equals("true"),
                        LabelFunc =
                            LoadStringGenerator<T>(xmlEvent.Element("label"), scriptIdentifier, NewSession,
                                "No label defined for event " + id + " in file " + file),
                        Modifiers = LoadModifiers<T>(xMtth.Element("modifiers"), scriptIdentifier, NewSession),
                        BaseMeanTimeToHappen = TimeSpanParser.Parse(xMtth.Element("base").Mandatory("No base mean time to happen defined for event " + id + " in file " + file).Value),
                        Conditions = LoadConditions<T>(xmlEvent.Element("conditions"), scriptIdentifier, NewSession),
                        PreExecute = LoadPreExecute<T>(xmlEvent.Element("preExecute"), scriptIdentifier, sharedSession),
                        Initializers = LoadInitializers<T>(xInput, sharedSession),
                        Outcomes = xOutcomes.Elements("outcome").Select(xmlOutcome => new Outcome<T>
                        {
                            LabelFunc =
                                LoadStringGenerator<T>(xmlOutcome.Element("label"), scriptIdentifier, sharedSession,
                                    "No label defined for an outcome of event " + id + " in file " + file),
                            TooltîpFunc = LoadStringGenerator<T>(xmlOutcome.Element("tooltip"), scriptIdentifier, sharedSession),
                            PreExecute = LoadPreExecute<T>(xmlOutcome.Element("preExecute"), scriptIdentifier, sharedSession),
                            Conditions = LoadConditions<T>(xmlOutcome.Element("conditions"), scriptIdentifier, sharedSession),
                            Effects = LoadEffects<T>(xmlOutcome.Element("effects"), scriptIdentifier, sharedSession),
                            BaseIaAffinity = Int32.Parse(xmlOutcome.Element("iaAffinity").Mandatory("No iaAffinity defined for an outcome of event " + id + " in file " + file).Element("base").Mandatory("No base iaAffinity defined for an outcome of event " + id + " in file " + file).Value),
                            IaAffinityModifiers = LoadModifiers<T>(xmlOutcome.Element("iaAffinity").Element("modifiers"), scriptIdentifier, sharedSession)
                        }).ToList()
                    });
                }
            }
            list.ToList().ForEach((e) => Events.Add(e.Id, e));
            return list;
        }

        private IList<Action<object>> LoadInitializers<T>(XElement xInput, Session sharedSession)
        {
            IList<Action<Object>> list = new List<Action<object>>();
            if (xInput != null)
                foreach (XElement xParameter in xInput.Elements("parameter"))
                    list.Add(_engine.Execute<Action<Object>>("(o) => " + xParameter.Value + " = (" + xParameter.Attribute("type").Value + ")o", sharedSession));
            return list;
        }

        private IList<Action<T>> LoadEffects<T>(XElement element, string scriptIdentifier, Session sharedSession)
        {
            IList<Action<T>> effects = new List<Action<T>>();
            if (element != null)
                foreach (XElement xmlEffect in element.Elements("effect"))
                    effects.Add(_engine.Execute<Action<T>>("(" + scriptIdentifier + ") => " + xmlEffect.Value, sharedSession));
            return effects;
        }

        private Action<T> LoadPreExecute<T>(XElement element, string scriptIdentifier, Session session)
        {
            if (element == null) return null;
            return _engine.Execute<Action<T>>("(" + scriptIdentifier + ") => " + element.Value, session);
        }

        private IList<Func<T, bool>> LoadConditions<T>(XElement element, string scriptIdentifier, Session session)
        {
            IList<Func<T, Boolean>> conditions = new List<Func<T, bool>>();
            if (element != null)
                foreach (XElement xmlCondition in element.Elements("condition"))
                    conditions.Add(_engine.Execute<Func<T, Boolean>>("(" + scriptIdentifier + ") => " + xmlCondition.Value, session));
            return conditions;
        }

        private Func<T, string> LoadStringGenerator<T>(XElement xString, string scriptIdentifier, Session session, String exceptionMessage = null) where T : IEventable
        {
            if (exceptionMessage == null && xString == null)
                return null;
            String label = (String)xString.Mandatory(exceptionMessage);
            return _engine.Execute<Func<T, String>>("(" + scriptIdentifier + ") => { return \"" + label.Replace("{", "\"+").Replace("}", "+\"") + "\";}", session);
        }

        private IList<IModifier<T>> LoadModifiers<T>(XElement xModifiers, string scriptIdentifier, Session session) where T : IEventable
        {
            IList<IModifier<T>> modifiers = new List<IModifier<T>>();
            if (xModifiers == null) return modifiers;
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
                        _engine.Execute<Func<T, Double>>("(" + scriptIdentifier + ") => " + xmlModifier.Value, session), type));
                else
                    modifiers.Add(new StaticModifier<T>(Double.Parse(xmlModifier.Attribute("factor").Value),
                        _engine.Execute<Func<T, Boolean>>("(" + scriptIdentifier + ") => " + xmlModifier.Value, session), type));
            }
            return modifiers;
        }

        public override void Initialize(Action<string> feedback)
        { }

        public override void Update(double delta)
        { }
        public override void Dispose()
        { }

        public void RegisterAsService()
        {
            Game.Services.Register<IEventManager>(this);
        }

        public void Trigger(IEventable target, string eventId, string delay, object[] parameters)
        {
            DelayedEvents.Add(new DelayedEvent(target, Events[eventId], delay == null ? 0 : TimeSpanParser.Parse(delay), parameters));
        }

        public void DayUpdate()
        {
            //Process delayed events
            for (int i = DelayedEvents.Count-1; i >= 0; i--)
            {
                DelayedEvent delayedEvent = DelayedEvents[i];
                delayedEvent.Delay--;
                if (delayedEvent.Delay <= 0)
                {
                    delayedEvent.Event.Execute(delayedEvent.Target, delayedEvent.Parameters);
                    DelayedEvents.RemoveAt(i);
                }
            }
        }
    }
}