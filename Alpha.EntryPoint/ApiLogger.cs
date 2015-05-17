using System;
using System.Linq;
using System.Reflection;
using Alpha.Core;

namespace Alpha.EntryPoint
{
    static class Api
    {
        public static void LogPublicElements()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"API.csv"))
            {
                foreach (Type type in Assembly.GetAssembly(typeof(World)).GetTypes().Where(t => t.IsPublic))
                {
                    if (!type.GetMembers(BindingFlags.Public
                                        | BindingFlags.Instance
                                        | BindingFlags.DeclaredOnly
                                        | BindingFlags.InvokeMethod).Any())
                        continue;
                    file.WriteLine(type.Name);
                    foreach (ConstructorInfo member in type.GetConstructors())
                        file.WriteLine(";Constructor"+"(" + (member.GetParameters().Any() ? string.Join(", ", member.GetParameters().Select(p => p.ParameterType.Name)) : "")+")");
                    foreach (MethodInfo member in type.GetMethods())
                    {
                        if (!(new[] { "ToString", "GetHashCode", "Equals", "GetType" }).Contains(member.Name))
                        {
                            if (member.DeclaringType != null && member.DeclaringType.IsInterface)
                            {
                                if(!member.DeclaringType.IsPublic)
                                    continue;
                            }
                            var toBeDeleted = new[] { "System.", "Collections.Generic.", "`1", "`2", "Alpha.Core.Realms.", "Alpha.Core.Provinces.", "Alpha.Core.Fleets.", "Alpha.Core.Commands.", "Alpha.Core.Calendars.", "Alpha.Toolkit.Math." };
                            string name = member.ToString();
                            name = toBeDeleted.Aggregate(name, (current, value) => current.Replace(value, string.Empty));
                            name = name.Replace(" ", ";");
                            file.WriteLine(name);
                        }
                    }
                    file.WriteLine();
                }
            }
        }
    }
}
