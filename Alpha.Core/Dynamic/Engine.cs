using System;
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
            return session.Execute<T>(query);
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
                Session session = _engine.CreateSession(ScriptContext);
                session.AddReference(typeof(ScriptContext).Assembly);
                Execute<String>("using Alpha.Core.Tags;", session);
                return session;
            }
        }
    }
}
