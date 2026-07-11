using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main(string[] args)
    {

        string dllPath;
        if (args.Length > 0)
        {
            dllPath = args[0];
        }
        else
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            dllPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../FileSystemCommands/bin/Debug/net8.0/FileSystemCommands.dll"));
            
            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(baseDir, "FileSystemCommands.dll");
            }
        }

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл '{dllPath}' не найден!");
            Console.WriteLine("Укажите путь к FileSystemCommands.dll первым аргументом.");
            return;
        }

        try
        {
            Console.WriteLine($"Загрузка библиотеки: {dllPath}");
            Assembly assembly = Assembly.LoadFrom(dllPath);
            string testDir = Path.Combine(Path.GetTempPath(), "CommandRunnerTestDir");
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Привет мир!");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "Тестовый файл с текстом");
            File.WriteAllText(Path.Combine(testDir, "log.txt"), "Какой-то лог");

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Console.WriteLine($"\nНайден тип команды: {type.Name}");

                    ICommand? command = null;
                    if (type.Name == "DirectorySizeCommand")
                    {
                        command = Activator.CreateInstance(type, new object[] { testDir }) as ICommand;
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        command = Activator.CreateInstance(type, new object[] { testDir, "*.txt" }) as ICommand;
                    }

                    if (command != null)
                    {
                        Console.WriteLine($"Выполнение {type.Name}...");
                        command.Execute();
                    }
                }
            }
            Directory.Delete(testDir, true);
            Console.WriteLine("\nВыполнение команд завершено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при выполнении: {ex.Message}");
        }
    }
}
