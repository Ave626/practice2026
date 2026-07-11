using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace task09;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: Укажите путь к DLL файлу в качестве аргумента.");
            return;
        }

        string dllPath = args[0];
        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл '{dllPath}' не найден.");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"=== Информация о сборке: {assembly.GetName().Name} ===");
            Console.WriteLine($"Путь: {dllPath}\n");

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (type.IsClass && type.IsPublic)
                {
                    Console.WriteLine($"Класс: {type.FullName}");

                    Console.WriteLine("  Атрибуты класса:");
                    object[] classAttributes = type.GetCustomAttributes(false);
                    if (classAttributes.Length == 0)
                    {
                        Console.WriteLine("    (нет атрибутов)");
                    }
                    foreach (object attr in classAttributes)
                    {
                        if (attr is DisplayNameAttribute displayNameAttr)
                        {
                            Console.WriteLine($"    - [DisplayName] = \"{displayNameAttr.DisplayName}\"");
                        }
                        else if (attr is VersionAttribute versionAttr)
                        {
                            Console.WriteLine($"    - [Version] = {versionAttr.Major}.{versionAttr.Minor}");
                        }
                        else
                        {
                            Console.WriteLine($"    - {attr.GetType().Name}");
                        }
                    }

                    Console.WriteLine("  Конструкторы:");
                    ConstructorInfo[] constructors = type.GetConstructors();
                    foreach (ConstructorInfo ctor in constructors)
                    {
                        Console.Write($"    - {type.Name}(");
                        ParameterInfo[] parameters = ctor.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.WriteLine(")");
                    }

                    Console.WriteLine("  Методы:");
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    if (methods.Length == 0)
                    {
                        Console.WriteLine("    (нет методов)");
                    }
                    foreach (MethodInfo method in methods)
                    {
                        Console.Write($"    - {method.ReturnType.Name} {method.Name}(");
                        ParameterInfo[] parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.Write(")");
                        object[] methodAttrs = method.GetCustomAttributes(false);
                        if (methodAttrs.Length > 0)
                        {
                            Console.Write(" [");
                            foreach (object attr in methodAttrs)
                            {
                                if (attr is DisplayNameAttribute dna)
                                {
                                    Console.Write($"DisplayName(\"{dna.DisplayName}\")");
                                }
                                else
                                {
                                    Console.Write(attr.GetType().Name);
                                }
                            }
                            Console.Write("]");
                        }
                        Console.WriteLine();
                    }

                    Console.WriteLine(new string('-', 50));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при анализе сборки: {ex.Message}");
        }
    }
}
