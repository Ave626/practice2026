using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using ScottPlot;
using task17;

namespace task18tests;

public class SchedulerTests
{
    private class TestLongCommand : ILongCommand
    {
        private readonly string _name;
        private readonly int _totalSteps;
        private readonly List<string> _executionLog;
        private int _currentStep = 0;

        public TestLongCommand(string name, int totalSteps, List<string> executionLog)
        {
            _name = name;
            _totalSteps = totalSteps;
            _executionLog = executionLog;
        }

        public bool IsCompleted => _currentStep >= _totalSteps;

        public void Execute()
        {
            if (IsCompleted) return;
            _currentStep++;
            lock (_executionLog)
            {
                _executionLog.Add($"{_name}-{_currentStep}");
            }
        }
    }

    private class PlotTestLongCommand : ILongCommand
    {
        private readonly int _totalSteps;
        private readonly int _sleepMs;
        private readonly Stopwatch _stopwatch;
        private readonly List<(double Time, double Progress)> _points;
        private int _step = 0;

        public PlotTestLongCommand(int totalSteps, int sleepMs, Stopwatch stopwatch, List<(double Time, double Progress)> points)
        {
            _totalSteps = totalSteps;
            _sleepMs = sleepMs;
            _stopwatch = stopwatch;
            _points = points;
        }

        public bool IsCompleted => _step >= _totalSteps;

        public void Execute()
        {
            if (IsCompleted) return;
            _step++;
            Thread.Sleep(_sleepMs);
            _points.Add((_stopwatch.Elapsed.TotalMilliseconds, (double)_step / _totalSteps * 100));
        }
    }

    [Fact]
    public void TestRoundRobinScheduling()
    {
        var executionLog = new List<string>();
        var scheduler = new QueueScheduler();
        var server = new ServerThread(scheduler);

        var cmdA = new TestLongCommand("A", 3, executionLog);
        var cmdB = new TestLongCommand("B", 2, executionLog);

        server.Queue.Add(cmdA);
        server.Queue.Add(cmdB);

        var softStop = new SoftStopCommand(server);
        server.Queue.Add(softStop);

        server.Start();
        
        bool joined = server.Thread.Join(2000);
        Assert.True(joined);

        Assert.Equal(new[] { "A-1", "B-1", "A-2", "B-2", "A-3" }, executionLog);
    }

    [Fact]
    public void TestHardStopInterruptsScheduler()
    {
        var executionLog = new List<string>();
        var scheduler = new QueueScheduler();
        var server = new ServerThread(scheduler);

        var cmdA = new TestLongCommand("A", 5, executionLog);
        var cmdB = new TestLongCommand("B", 5, executionLog);

        server.Queue.Add(cmdA);
        server.Queue.Add(cmdB);

        var hardStop = new HardStopCommand(server);
        server.Queue.Add(hardStop);

        server.Start();
        
        bool joined = server.Thread.Join(2000);
        Assert.True(joined);

        Assert.Contains("A-1", executionLog);
        Assert.Contains("B-1", executionLog);
        Assert.DoesNotContain("A-5", executionLog);
        Assert.DoesNotContain("B-5", executionLog);
    }

    [Fact]
    public void GraphGenerate()
    {
        var scheduler = new QueueScheduler();
        var server = new ServerThread(scheduler);

        var task1Points = new List<(double Time, double Progress)>();
        var task2Points = new List<(double Time, double Progress)>();
        var task3Points = new List<(double Time, double Progress)>();

        var stopwatch = Stopwatch.StartNew();

        var task1 = new PlotTestLongCommand(2, 15, stopwatch, task1Points);
        var task2 = new PlotTestLongCommand(3, 20, stopwatch, task2Points);
        var task3 = new PlotTestLongCommand(5, 10, stopwatch, task3Points);

        server.Queue.Add(task1);
        server.Queue.Add(task2);
        server.Queue.Add(task3);
        server.Queue.Add(new SoftStopCommand(server));

        server.Start();
        bool joined = server.Thread.Join(2000);
        Assert.True(joined);

        var plot = new Plot();
        plot.Title("Диаграмма прогресса выполнения задач (Round Robin)");
        plot.XLabel("Время (мс)");
        plot.YLabel("Прогресс (%)");

        double[] xs1 = new double[task1Points.Count + 1];
        double[] ys1 = new double[task1Points.Count + 1];
        xs1[0] = 0;
        ys1[0] = 0;
        for (int i = 1; i < task1Points.Count + 1; i++)
        {
            xs1[i] = task1Points[i - 1].Time;
            ys1[i] = task1Points[i - 1].Progress;
        }
        var s1 = plot.Add.Scatter(xs1, ys1);
        s1.LegendText = "Задача 1";
        s1.LineWidth = 2;
        s1.MarkerSize = 8;

        double[] xs2 = new double[task2Points.Count + 1];
        double[] ys2 = new double[task2Points.Count + 1];
        xs2[0] = 0;
        ys2[0] = 0;
        for (int i = 1; i < task2Points.Count + 1; i++)
        {
            xs2[i] = task2Points[i - 1].Time;
            ys2[i] = task2Points[i - 1].Progress;
        }
        var s2 = plot.Add.Scatter(xs2, ys2);
        s2.LegendText = "Задача 2";
        s2.LineWidth = 2;
        s2.MarkerSize = 8;

        double[] xs3 = new double[task3Points.Count + 1];
        double[] ys3 = new double[task3Points.Count + 1];
        xs3[0] = 0;
        ys3[0] = 0;
        for (int i = 1; i < task3Points.Count + 1; i++)
        {
            xs3[i] = task3Points[i - 1].Time;
            ys3[i] = task3Points[i - 1].Progress;
        }
        var s3 = plot.Add.Scatter(xs3, ys3);
        s3.LegendText = "Задача 3";
        s3.LineWidth = 2;
        s3.MarkerSize = 8;

        plot.ShowLegend();

        string filePathPNG = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "progress_chart.png");
        string filePathTXT = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "report.txt");
        plot.SavePng(filePathPNG, 800, 600);

        using (var writer = new StreamWriter(filePathTXT))
        {
            writer.WriteLine($"Задача 1 выполнилась за {task1Points.Last().Time} мс");
            writer.WriteLine($"Задача 2 выполнилась за {task2Points.Last().Time} мс");
            writer.WriteLine($"Задача 3 выполнилась за {task3Points.Last().Time} мс");
        }
    }
}
