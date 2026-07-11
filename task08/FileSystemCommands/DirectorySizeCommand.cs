using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private string directoryPath;
    
    public long Size { get; private set; }

    public DirectorySizeCommand(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Указанный каталог не найден: {directoryPath}");
        }

        Size = CalculateDirectorySize(directoryPath);
        Console.WriteLine($"Размер каталога '{directoryPath}': {Size} байт");
    }

    private long CalculateDirectorySize(string path){
        long size = 0;

        foreach (var filePath in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(filePath);
            size += fileInfo.Length;
        }

        foreach (var dirPath in Directory.GetDirectories(path))
        {
            size += CalculateDirectorySize(dirPath);
        }

        return size;
    }
}
