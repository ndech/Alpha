using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Alpha.Core.Dynamic
{
    static class Engine
    {
        private static ScriptContext _scriptContext;
        static readonly ScriptEngine _engine = new ScriptEngine();
        public static T Execute<T>(string query, Session session)
        {
            return _engine.Execute<T>(query, session);
        }

        private static ScriptContext ScriptContext
        {
            get
            {
                return _scriptContext ?? (_scriptContext = new ScriptContext());
            }
        }

        public static Session NewSession
        {
            get
            {
                Session session = Session.Create(ScriptContext);
                session.AddReference(typeof(ScriptContext).Assembly);
                return session;
            }
        }
    }
}
