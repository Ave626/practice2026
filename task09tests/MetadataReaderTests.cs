using Xunit;
using System;
using System.IO;

namespace task09tests;

public class MetadataReaderTests
{
    [Fact]
    public void Main_WithNoArguments_ShouldPrintErrorMessage()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);

        task09.Program.Main(new string[0]);

        var output = writer.ToString();
        Assert.Contains("Ошибка", output);
    }

    [Fact]
    public void Main_WithValidDll_ShouldPrintCorrectMetadata()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string dllPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../task08/FileSystemCommands/bin/Debug/net8.0/FileSystemCommands.dll"));

        if (!File.Exists(dllPath))
        {
            dllPath = Path.Combine(baseDir, "FileSystemCommands.dll");
        }

        if (File.Exists(dllPath))
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            task09.Program.Main(new string[] { dllPath });

            var output = writer.ToString();

            Assert.Contains("Класс: FileSystemCommands.DirectorySizeCommand", output);
            Assert.Contains("[DisplayName] = \"Подсчет размера директории\"", output);
            Assert.Contains("[Version] = 1.0", output);

            Assert.Contains("Класс: FileSystemCommands.FindFilesCommand", output);
            Assert.Contains("[DisplayName] = \"Поиск файлов по маске\"", output);
            Assert.Contains("[Version] = 1.1", output);

            Assert.Contains("Execute", output);
            Assert.Contains("String directoryPath", output);
        }
    }
}
