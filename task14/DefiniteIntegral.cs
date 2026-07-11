using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a,double b,Func<double,double> function,double step,int threadsnumber){
        double totalSum = 0.0;

        using(Barrier barrier = new Barrier(threadsnumber + 1)){
            double totalWidth = b - a;
            double widthPerThread = totalWidth / threadsnumber;

            Thread[] threads = new Thread[threadsnumber];

            for(int i = 0;i < threadsnumber;i++){
                int threadIndex = i;
                threads[i] = new Thread(() =>
                {
                   double threadStart = a + threadIndex * widthPerThread;
                   double threadEnd = a + (threadIndex + 1) * widthPerThread;

                   int stepsCount = (int)Math.Round((threadEnd - threadStart) / step);
                   if(stepsCount < 1) stepsCount = 1;

                   double localStep = (threadEnd - threadStart) / stepsCount;
                   double localSum  = 0.0;

                   for (int j = 0; j < stepsCount; j++)
                    {
                        double x1 = threadStart + j * localStep;
                        double x2 = threadStart + (j + 1) * localStep;
                        localSum += (function(x1) + function(x2)) / 2.0 * localStep;
                    }

                    Accumulate(ref totalSum,localSum);

                    barrier.SignalAndWait();
                });

                threads[i].Start();
            }

            barrier.SignalAndWait();

        }
        return totalSum;
    }

    private static void Accumulate(ref double location, double value)
    {
        double currentValue;
        double newValue;
        do
        {
            currentValue = location;
            newValue = currentValue + value;
        }
        while (Interlocked.CompareExchange(ref location, newValue, currentValue) != currentValue);
    }
}