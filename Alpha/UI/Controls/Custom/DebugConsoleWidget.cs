using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Alpha.Events;
using Alpha.Scripting;
using Alpha.UI.Coordinates;
using Alpha.UI.Scrollable;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using SharpDX;

namespace Alpha.UI.Controls.Custom
{
    class DebugConsoleWidget : Panel
    {
        private ScriptContext _scriptContext;
        private readonly List<ConsoleLine> _lines = new List<ConsoleLine>(); 
        private readonly List<String> _submissions = new List<string>();
        private TextInput _input;
        private int _submissionPosition = 0;
        private ScriptContext ScriptContext
        {
            get { return _scriptContext ?? 
                (_scriptContext = 
                new ScriptContext(
                    Game.Services.Get<ICalendar>(), 
                    Game.Services.Get<IEventManager>(), 
                    Game.Services.Get<IRealmManager>().PlayerRealm, 
                    Game.Services.Get<IRealmManager>().Realms.Cast<IScriptableRealm>().ToList(),
                    Game.Services.Get<IFleetManager>())); }
        }

        public DebugConsoleWidget(IGame game) : base(game, "debug_console", new UniRectangle(0,0,1.0f,1.0f), new Color(0,0,0,0.8f))
        {
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new MonitoringHeader(Game, new UniRectangle(0, 0, 1.0f, 25)));
            ScrollableConsoleContainer container;
            Register(container = new ScrollableConsoleContainer(Game, new UniRectangle(0, 25, 1.0f, new UniScalar(1.0f, -55)), _lines));
            Register(_input = new TextInput(Game, "debug_console_input", new UniRectangle(0, new UniScalar(1.0f, -30), 1.0f, 30)));

            ScriptEngine engine = new ScriptEngine();
            Session session = Session.Create(ScriptContext);
            session.AddReference(typeof(ScriptContext).Assembly);
            session.AddReference(new AssemblyNameReference("System.Core"));
            engine.Execute("using System.Linq;", session);
            Object obj;
            _input.OnSubmit += (s) =>
            {
                if(s == "")
                    return;
                _submissions.Add(s);
                _submissionPosition = -1;
                if (s.Equals("clear"))
                {
                    _lines.Clear();
                    _submissions.Clear();
                    session = Session.Create(ScriptContext);
                    session.AddReference(typeof(ScriptContext).Assembly);
                    session.AddReference(new AssemblyNameReference("System.Core"));
                    engine.Execute("using System.Linq;", session);
                    container.Refresh();
                    return;
                }
                _lines.Add(new ConsoleLine(s, ConsoleLine.ConsoleLineType.Command));
                try
                {
                    if ((obj = engine.Execute(s, session)) != null)
                    {
                        IEnumerable list = obj as IEnumerable;
                        if (list != null && !(obj is String))
                        {
                            int count = 0;
                            ConsoleLine header;
                            _lines.Add(header = new ConsoleLine("", ConsoleLine.ConsoleLineType.Info));
                            foreach (var item in list)
                            {
                                if(count<5)
                                    _lines.Add(new ConsoleLine(item.ToString(), ConsoleLine.ConsoleLineType.Result));
                                count ++;
                            }
                            header.Content = "List of " + count + " items :";
                            if (count > 5)
                                _lines.Add(new ConsoleLine("and "+ (count-5) +" more items", ConsoleLine.ConsoleLineType.Info));
                        }
                        else
                            _lines.Add(new ConsoleLine(obj.ToString(), ConsoleLine.ConsoleLineType.Result));
                    }
                }
                catch (Roslyn.Compilers.CompilationErrorException e)
                {
                    _lines.Add(new ConsoleLine(e.Message, ConsoleLine.ConsoleLineType.Error));
                }
                container.MoveToLast();
                container.Refresh();
            };
        }
        
        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (!repeat && key == Key.Tab && UiManager.IsKeyPressed(Key.LeftShift))
            {
                Visible = !Visible;
                UiManager.RecalculateActiveComponents();
                return true;
            }
            else if (Visible && key == Key.Up && _submissions.Count>0)
            {
                _submissionPosition = Math.Min(_submissionPosition + 1, _submissions.Count - 1);
                _input.Text = _submissions[_submissions.Count - 1 - _submissionPosition];
                return true;
            }
            else if (Visible && key == Key.Down && _submissions.Count > 0 && _submissionPosition>0)
            {
                _submissionPosition = Math.Max(_submissionPosition - 1, 0);
                _input.Text = _submissions[_submissions.Count - 1 - _submissionPosition];
                return true;
            }
            return false;
        }
    }
}
