using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLib;

namespace task10;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: Укажите путь к папке с плагинами (DLL).");
            return;
        }

        string pluginDir = args[0];
        if (!Directory.Exists(pluginDir))
        {
            Console.WriteLine($"Ошибка: Папка '{pluginDir}' не найдена.");
            return;
        }

        try
        {
            string[] dllFiles = Directory.GetFiles(pluginDir, "*.dll");
            var plugins = new Dictionary<string, Type>();
            var dependencyGraph = new Dictionary<string, List<string>>();

            foreach (string dllPath in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            var pluginAttr = type.GetCustomAttribute<PluginLoadAttribute>();
                            if (pluginAttr != null)
                            {
                                string name = pluginAttr.PluginName;
                                if (plugins.ContainsKey(name))
                                {
                                    continue;
                                }
                                plugins.Add(name, type);
                                dependencyGraph.Add(name, new List<string>(pluginAttr.Dependencies));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Предупреждение: Ошибка загрузки {Path.GetFileName(dllPath)}: {ex.Message}");
                }
            }

            List<string> executionOrder = GetExecutionOrder(dependencyGraph);

            Console.WriteLine("\n=== Запуск плагинов ===");
            foreach (string name in executionOrder)
            {
                if (plugins.TryGetValue(name, out Type type))
                {
                    Console.WriteLine($"Выполнение плагина: {name}");
                    ICommand command = (ICommand)Activator.CreateInstance(type);
                    command.Execute();
                }
                else
                {
                    Console.WriteLine($"Ошибка: Плагин '{name}' не найден в загруженных сборках (хотя заявлен как зависимость).");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public static List<string> GetExecutionOrder(Dictionary<string, List<string>> graph)
    {
        var visited = new Dictionary<string, int>();
        var order = new List<string>();

        foreach (var node in graph.Keys)
        {
            visited[node] = 0;
        }

        foreach (var node in graph.Keys)
        {
            if (visited[node] == 0)
            {
                Visit(node, graph, visited, order);
            }
        }

        return order;
    }

    private static void Visit(string node, Dictionary<string, List<string>> graph, Dictionary<string, int> visited, List<string> order)
    {
        visited[node] = 1;

        if (graph.ContainsKey(node))
        {
            foreach (string dependency in graph[node])
            {
                if (!visited.ContainsKey(dependency))
                {
                    visited[dependency] = 0;
                }

                if (visited[dependency] == 1)
                {
                    throw new InvalidOperationException($"Обнаружена циклическая зависимость с участием плагина '{node}' и '{dependency}'!");
                }

                if (visited[dependency] == 0)
                {
                    Visit(dependency, graph, visited, order);
                }
            }
        }

        visited[node] = 2;
        order.Add(node);
    }
}
