using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Alpha.Scripting;
using Alpha.Toolkit;
using Alpha.UI.Controls;
using Alpha.UI.Styles;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Alpha
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
                    String id = (String) xmlEvent.Attribute("id");
                    if (id == null)
                        throw new InvalidEventDataException(String.Format("An event without id is defined in file {0}", file));
                    Event<T> newEvent = new Event<T>(id);
                    newEvent.IsTriggeredOnly = xmlEvent.Attribute("triggered-only") != null &&
                                              xmlEvent.Attribute("triggered-only").Value.Equals("true");
                    //Load label
                    String label = (String) xmlEvent.Element("Label");
                    if (label == null)
                        throw new InvalidEventDataException(String.Format("No label defined for event {0} in file {1}", id , file));
                    newEvent.LabelFunc = _engine.Execute<Func<T, String>>("(Realm) => { return \""+label.Replace("{","\"+").Replace("}","+\"")+"\";}", NewSession);
                    //Load conditions
                    var xElement = xmlEvent.Element("conditions");
                    if (xElement != null)
                        foreach (XElement xmlCondition in xElement.Elements("condition"))
                            newEvent.Conditions.Add(_engine.Execute<Func<T, Boolean>>("(Realm) => " + xmlCondition.Value, NewSession));
                    //Load base mtth
                    String mtth = (String)xmlEvent.Element("baseMtth");
                    if (mtth == null)
                        throw new InvalidEventDataException(String.Format("No base mean time to happen (baseMtth) defined for event {0} in file {1}", id, file));
                    newEvent.BaseMeanTimeToHappen = TimeSpanParser.Parse(mtth);
                    //Load multipliers
                    xElement = xmlEvent.Element("multipliers");
                    if (xElement != null)
                    {
                        foreach (XElement xmlModifier in xElement.Elements())
                        {
                            ModifierType type;
                            if(xmlModifier.Name=="multiplier")
                                type = ModifierType.Multiplier;
                            else if(xmlModifier.Name=="reducer")
                                type = ModifierType.Reducer;
                            else
                                continue;

                            if (xmlModifier.Attribute("factor") == null)
                                newEvent.Modifiers.Add(new DynamicModifier<T>(
                                    _engine.Execute<Func<T, Double>>("(Realm) => " + xmlModifier.Value, NewSession), type));
                            else
                                newEvent.Modifiers.Add(new StaticModifier<T>(Double.Parse(xmlModifier.Attribute("factor").Value),
                                    _engine.Execute<Func<T, Boolean>>("(Realm) => " + xmlModifier.Value, NewSession), type));
                        }
                    }
                    
                    list.Add(newEvent);
                }
            }
            return list;
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
