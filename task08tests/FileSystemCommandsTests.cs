using Xunit;
using System;
using System.IO;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "DirectorySizeCommandTestDir");
        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
        Directory.CreateDirectory(testDir);
        
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Assert.Equal(10, command.Size);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "FindFilesCommandTestDir");
        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
        Directory.CreateDirectory(testDir);

        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Assert.Single(command.FoundFiles);
        Assert.Equal("file1.txt", Path.GetFileName(command.FoundFiles[0]));

        Directory.Delete(testDir, true);
    }
}
