using System;
using System.Linq;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Alpha.Core.Dynamic
{
    static class Engine
    {
        private static ScriptContext _scriptContext;
        private static readonly ScriptEngine _engine = new ScriptEngine();
        public static T Execute<T>(string query, Session session)
        {
            return session.Execute<T>(query);
        }

        public static Func<T,T2> GetFunc<T,T2>(string query, Session session)
        {
            ScriptNameAttribute attribute =
                (typeof (T).GetCustomAttributes(typeof (ScriptNameAttribute), false)).Cast<ScriptNameAttribute>()
                    .SingleOrDefault();
            String scriptIdentifier = attribute == null ? typeof (T).Name : attribute.ScriptName;
            return session.Execute<Func<T, T2>>("(" + scriptIdentifier + ") =>" + query);
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
