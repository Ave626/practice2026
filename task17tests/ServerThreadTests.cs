using System;
using System.Threading;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    private class MockCommand : ICommand
    {
        public bool Executed { get; private set; }
        private readonly Action? _action;

        public MockCommand(Action? action = null)
        {
            _action = action;
        }

        public void Execute()
        {
            Executed = true;
            _action?.Invoke();
        }
    }

    [Fact]
    public void TestHardStop()
    {
        var serverThread = new ServerThread();
        var cmd1 = new MockCommand();
        var hardStop = new HardStopCommand(serverThread);
        var cmd2 = new MockCommand();

        serverThread.Queue.Add(cmd1);
        serverThread.Queue.Add(hardStop);
        serverThread.Queue.Add(cmd2);

        serverThread.Start();
        serverThread.Thread.Join(2000);

        Assert.True(cmd1.Executed);
        Assert.False(cmd2.Executed);
        Assert.False(serverThread.Thread.IsAlive);
    }

    [Fact]
    public void TestSoftStop()
    {
        var serverThread = new ServerThread();
        var cmd1 = new MockCommand();
        var softStop = new SoftStopCommand(serverThread);
        var cmd2 = new MockCommand();

        serverThread.Queue.Add(cmd1);
        serverThread.Queue.Add(softStop);
        serverThread.Queue.Add(cmd2);

        serverThread.Start();
        serverThread.Thread.Join(2000);

        Assert.True(cmd1.Executed);
        Assert.True(cmd2.Executed);
        Assert.False(serverThread.Thread.IsAlive);
    }

    [Fact]
    public void TestStopFromWrongThread()
    {
        var serverThread = new ServerThread();
        var hardStop = new HardStopCommand(serverThread);
        var softStop = new SoftStopCommand(serverThread);

        var hardStopException = Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Equal("Команда HardStop должна выполняться только в ServerThread.", hardStopException.Message);

        var softStopException = Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        Assert.Equal("Команда SoftStop должна выполняться только в ServerThread.", softStopException.Message);
    }

    [Fact]
    public void TestExceptionHandling()
    {
        var serverThread = new ServerThread();
        var exceptionToThrow = new Exception("Test exception");
        var cmdWithException = new MockCommand(() => throw exceptionToThrow);
        var hardStop = new HardStopCommand(serverThread);

        Exception? caughtException = null;
        ICommand? caughtCommand = null;

        ExceptionHandler.Handle = (ex, cmd) =>
        {
            caughtException = ex;
            caughtCommand = cmd;
        };

        serverThread.Queue.Add(cmdWithException);
        serverThread.Queue.Add(hardStop);

        serverThread.Start();
        serverThread.Thread.Join(2000);

        Assert.Same(exceptionToThrow, caughtException);
        Assert.Same(cmdWithException, caughtCommand);
    }
}
