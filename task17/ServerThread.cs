using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    
    private Action _strategy;
    
    private readonly Thread _thread;
    
    private bool _stopRequested = false;

    public ServerThread()
    {
        _strategy = DefaultStrategy;
        _thread = new Thread(RunLoop);
    }

    public Thread Thread => _thread;
    public BlockingCollection<ICommand> Queue => _queue;

    public void Start() => _thread.Start();

    public void UpdateStrategy(Action newStrategy)
    {
        _strategy = newStrategy;
    }

    public void StopHard()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("Команда HardStop должна выполняться только в ServerThread.");

        _stopRequested = true;
    }

    public void StopSoft()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("Команда SoftStop должна выполняться только в ServerThread.");

        _queue.CompleteAdding();

        UpdateStrategy(() =>
        {
            if (_queue.TryTake(out var command))
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle?.Invoke(ex, command);
                }
            }
            else
            {
                _stopRequested = true;
            }
        });
    }

    private void RunLoop()
    {
        while (!_stopRequested)
        {
            _strategy();
        }
    }

    private void DefaultStrategy()
    {
        ICommand? command = null;
        try
        {
            command = _queue.Take();
            command.Execute();
        }
        catch (InvalidOperationException)
        {
            _stopRequested = true;
        }
        catch (Exception ex)
        {
            if (command != null)
            {
                ExceptionHandler.Handle?.Invoke(ex, command);
            }
        }
    }
}
