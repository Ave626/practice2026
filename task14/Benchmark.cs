using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ScottPlot;

namespace task14;

public class Benchmark{
    private static readonly Func<double, double> SIN = Math.Sin;
    private const double A = -100;
    private const double B = 100;

    private const double ExactValue = 0.0;
    private const double RequiredAccuracy = 1e-4;
    private const int MeasureCount = 10;

    public static void Run(){
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        double optimalStep = 0;
        double optimalStepTime = double.MaxValue;

        Console.WriteLine("Подбор шага:");
        Console.WriteLine($"{"Шаг",-12} {"Результат",-20} {"Погрешность",-15} {"Время (мс)",-12} {"Точность OK"}");

        foreach (double step in steps)
        {
            double result = DefiniteIntegral.SolveSingleThread(A, B, SIN, step);
            double error = Math.Abs(result - ExactValue);
            bool accurate = error <= RequiredAccuracy;
            double avgTime = MeasureSingleThreadTime(step);
            Console.WriteLine($"{step,-12:E1} {result,-20:F10} {error,-15:E4} {avgTime,-12:F2} {(accurate ? "Да" : "Нет")}");
            
            if (accurate && avgTime < optimalStepTime)
            {
                optimalStep = step;
                optimalStepTime = avgTime;
            }
        }

        Console.WriteLine($"\nОптимальный шаг по времени: {optimalStep:E1} (время: {optimalStepTime:F2} мс)");
        Console.WriteLine("Для теста потоков берем шаг 1e-6 (при больших шагах время слишком маленькое для многопотока).\n");

        double testStep = 1e-6;
        int maxThreads = Environment.ProcessorCount * 2;
        Console.WriteLine($"Тест количества потоков (шаг {testStep:E1}):");
        Console.WriteLine($"{"Потоки",-10} {"Время (мс)",-15}");

        var threadCounts = new List<int>();
        var times = new List<double>();
        double bestMultiTime = double.MaxValue;
        int optimalThreads = 2;

        for (int t = 2; t <= maxThreads; t++)
        {
            double avgTime = MeasureAverageTime(testStep, t);
            threadCounts.Add(t);
            times.Add(avgTime);

            Console.WriteLine($"{t,-10} {avgTime,-15:F2}");

            if (avgTime < bestMultiTime)
            {
                bestMultiTime = avgTime;
                optimalThreads = t;
            }
        }

        double singleThreadTime = MeasureSingleThreadTime(testStep);
        Console.WriteLine($"\nОднопоточная версия без потоков: {singleThreadTime:F2} мс");

        double speedup = (singleThreadTime - bestMultiTime) / singleThreadTime * 100;
        Console.WriteLine($"\nРезультаты:");
        Console.WriteLine($"Время в 1 поток (без Thread): {singleThreadTime:F2} мс");
        Console.WriteLine($"Лучшее многопоточное время:  {bestMultiTime:F2} мс (при {optimalThreads} потоках)");
        Console.WriteLine($"Ускорение:                    {speedup:F2}%");

        var plot = new Plot();
        plot.Add.Scatter(times.ToArray(), threadCounts.ConvertAll(x => (double)x).ToArray());
        plot.Title("Зависимость числа потоков от времени");
        plot.XLabel("Время выполнения (мс)");
        plot.YLabel("Количество потоков");
        plot.SavePng("benchmark_graph.png", 800, 600);

        using (var writer = new StreamWriter("benchmark_results.txt"))
        {
            writer.WriteLine($"Выбранный шаг интегрирования: {testStep:E1}");
            writer.WriteLine($"Оптимальное число потоков: {optimalThreads}");
            writer.WriteLine($"Время работы в 1 поток: {singleThreadTime:F2} мс");
            writer.WriteLine($"Время работы многопоточной версии: {bestMultiTime:F2} мс");
            writer.WriteLine($"Итоговое ускорение: {speedup:F2}%");
        }
    }

    private static double MeasureAverageTime(double step, int threads)
    {
        double totalMs = 0;
        for (int i = 0; i < MeasureCount; i++)
        {
            var sw = Stopwatch.StartNew();
            DefiniteIntegral.Solve(A, B, SIN, step, threads);
            sw.Stop();
            totalMs += sw.Elapsed.TotalMilliseconds;
        }
        return totalMs / MeasureCount;
    }

    private static double MeasureSingleThreadTime(double step)
    {
        double totalMs = 0;
        for (int i = 0; i < MeasureCount; i++)
        {
            var sw = Stopwatch.StartNew();
            DefiniteIntegral.SolveSingleThread(A, B, SIN, step);
            sw.Stop();
            totalMs += sw.Elapsed.TotalMilliseconds;
        }
        return totalMs / MeasureCount;
    }
}