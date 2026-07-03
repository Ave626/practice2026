using System;
using System.Collections.Generic;
using System.IO;
using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private string directoryPath;
    private string searchPattern;

    public List<string> FoundFiles { get; private set; } = new List<string>();

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        this.directoryPath = directoryPath;
        this.searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath, searchPattern);
            
            FoundFiles.Clear();
            FoundFiles.AddRange(files);

            Console.WriteLine("Найдено файлов: " + FoundFiles.Count);
            foreach (string file in FoundFiles)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
        else
        {
            Console.WriteLine("Каталог не найден");
        }
    }
}
