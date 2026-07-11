using System;
using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var classDisplayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classDisplayName != null)
        {
            Console.WriteLine($"Класс: {classDisplayName.DisplayName}");
        }

        var classVersion = type.GetCustomAttribute<VersionAttribute>();
        if (classVersion != null)
        {
            Console.WriteLine($"Версия: {classVersion.Major}.{classVersion.Minor}");
        }

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var method in methods){
            var methodDisplayName = method.GetCustomAttribute<DisplayNameAttribute>();
            if (methodDisplayName != null)
            {
                Console.WriteLine($"Метод: {method.Name} ({methodDisplayName.DisplayName})");
            }
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var property in properties){
            var propertyDisplayName = property.GetCustomAttribute<DisplayNameAttribute>();
            if (propertyDisplayName != null)
            {
                Console.WriteLine($"Свойство: {property.Name} ({propertyDisplayName.DisplayName})");
            }
        }
    }
}
