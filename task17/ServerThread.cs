using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IScheduler? _scheduler;
    private Action _strategy;
    private readonly Thread _thread;
    private bool _stopRequested = false;

    public ServerThread() : this(null)
    {
    }

    public ServerThread(IScheduler? scheduler)
    {
        _scheduler = scheduler;
        _strategy = DefaultStrategy;
        _thread = new Thread(RunLoop);
    }

    public Thread Thread => _thread;
    public BlockingCollection<ICommand> Queue => _queue;
    public IScheduler? Scheduler => _scheduler;

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
            ICommand? command = null;
            
            if (_queue.TryTake(out var cmd))
            {
                command = cmd;
            }
            else if (_scheduler != null && _scheduler.HasCommand())
            {
                command = _scheduler.Select();
            }
            else
            {
                _stopRequested = true;
                return;
            }

            try
            {
                command.Execute();

                if (_scheduler != null && command is ILongCommand longCommand && !longCommand.IsCompleted)
                {
                    _scheduler.Add(command);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle?.Invoke(ex, command);
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
            if (_scheduler == null)
            {
                command = _queue.Take();
                command.Execute();
            }
            else
            {
                int timeout = _scheduler.HasCommand() ? 0 : Timeout.Infinite;

                if (_queue.TryTake(out var cmd, timeout))
                {
                    command = cmd;
                }
                else if (_scheduler.HasCommand())
                {
                    command = _scheduler.Select();
                }

                if (command != null)
                {
                    command.Execute();

                    if (_scheduler != null && command is ILongCommand longCommand && !longCommand.IsCompleted)
                    {
                        _scheduler.Add(command);
                    }
                }
            }
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
