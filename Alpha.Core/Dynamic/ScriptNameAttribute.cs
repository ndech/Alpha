using System;

namespace Alpha.Core.Dynamic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    class ScriptNameAttribute : Attribute
    {
        public string ScriptName { get; private set; }

        internal ScriptNameAttribute(string scriptName)
        {
            ScriptName = scriptName;
        }
    }
}
