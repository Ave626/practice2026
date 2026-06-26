using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(m => m.Name);
    }
}
