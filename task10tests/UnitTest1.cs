using Xunit;
using System;
using System.Collections.Generic;
using task10;

namespace task10tests;

public class UnitTest1
{
    [Fact]
    public void TestTopologicalSort_CorrectOrder()
    {
        var graph = new Dictionary<string, List<string>>
        {
            { "PluginA", new List<string> { "PluginB" } },
            { "PluginB", new List<string>() }
        };

        var order = Program.GetExecutionOrder(graph);

        Assert.Equal(2, order.Count);
        Assert.Equal("PluginB", order[0]);
        Assert.Equal("PluginA", order[1]);
    }

    [Fact]
    public void TestTopologicalSort_Cycle_Throws()
    {
        var graph = new Dictionary<string, List<string>>
        {
            { "PluginA", new List<string> { "PluginB" } },
            { "PluginB", new List<string> { "PluginA" } }
        };

        Assert.Throws<InvalidOperationException>(() => Program.GetExecutionOrder(graph));
    }
}
