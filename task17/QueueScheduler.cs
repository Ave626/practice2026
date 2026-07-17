using System;
using System.Collections.Generic;

namespace task17;

public class QueueScheduler : IScheduler
{
    private readonly Queue<ICommand> _commands = new();

    public bool HasCommand()
    {
        return _commands.Count > 0;
    }

    public ICommand Select()
    {
        if (_commands.Count == 0)
            throw new InvalidOperationException("Планировщик пуст.");
        
        return _commands.Dequeue();
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null) throw new ArgumentNullException(nameof(cmd));
        _commands.Enqueue(cmd);
    }
}
