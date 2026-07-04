using System;
using CommandLib;

namespace FileSystemCommands;

[PluginLoad("LoggerPlugin")]
public class LoggerPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("LoggerPlugin успешно инициализирован и запущен.");
    }
}

[PluginLoad("HelloPlugin", "LoggerPlugin")]
public class HelloPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("HelloPlugin запущен! Привет из системы плагинов.");
    }
}
