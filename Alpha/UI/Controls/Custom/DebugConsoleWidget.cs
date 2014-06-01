using System;
using System.Collections.Generic;
using System.Windows.Input;
using Alpha.Events;
using Alpha.Scripting;
using Alpha.UI.Coordinates;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using SharpDX;

namespace Alpha.UI.Controls.Custom
{
    class DebugConsoleWidget : Panel
    {
        private ScriptContext _scriptContext;
        private List<ConsoleLine> _lines = new List<ConsoleLine>(); 
        private ScriptContext ScriptContext
        {
            get { return _scriptContext ?? (_scriptContext = new ScriptContext(Game.Services.GetService<ICalendar>(), Game.Services.GetService<IEventManager>())); }
        }

        public DebugConsoleWidget(IGame game) : base(game, "debug_console", new UniRectangle(0,0,1.0f,0.5f), new Color(0,0,0,0.6f))
        {
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new MonitoringHeader(Game));
            TextInput input;
            Register(input = new TextInput(Game, "debug_console_input", new UniRectangle(0, new UniScalar(1.0f, -30), 1.0f, 30)));

            ScriptEngine engine = new ScriptEngine();
            Session session = Session.Create(ScriptContext);
            session.AddReference(typeof(ScriptContext).Assembly);
            Object obj;
            input.OnSubmit += (s) =>
            {
                _lines.Add(new ConsoleLine(s, ConsoleLine.ConsoleLineType.Command));
                try
                {
                    if ((obj = engine.Execute(s, session)) != null)
                        _lines.Add(new ConsoleLine(obj.ToString(), ConsoleLine.ConsoleLineType.Result));
                }
                catch (Roslyn.Compilers.CompilationErrorException e)
                {
                    _lines.Add(new ConsoleLine(e.Message, ConsoleLine.ConsoleLineType.Error));
                }
                Console.Clear();
                foreach (ConsoleLine line in _lines)
                {
                    Console.WriteLine((line.Type == ConsoleLine.ConsoleLineType.Command ? ">> ":"") + line.Content);
                }
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
            return false;
        }
    }
}
