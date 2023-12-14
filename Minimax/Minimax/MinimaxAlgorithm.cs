using System.Diagnostics;

namespace Minimax;

static class MinimaxAlgorithm
{
    public static (double, long) FindMinimax(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        
        Stopwatch stopwatch = new();
        stopwatch.Start();

        double[] rowMax = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            double maxInRow = double.MinValue;
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] > maxInRow)
                {
                    maxInRow = matrix[i, j];
                }
            }
            rowMax[i] = maxInRow;
        }

        double minimax = rowMax[0];
        for (int i = 1; i < rows; i++)
        {
            if (rowMax[i] < minimax)
            {
                minimax = rowMax[i];
            }
        }

        stopwatch.Stop();
        return (minimax, stopwatch.ElapsedTicks);
    }
    
    public static (double, long, int) FindMinimaxParallel(double[,] matrix, int threadsAmount)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        if (threadsAmount <= 0)
        {
            threadsAmount = Environment.ProcessorCount * 12 / 7;
        }

        if (threadsAmount > rows)
        {
            threadsAmount = rows;
        }

        var rowsPerOneThread = rows / threadsAmount;
        
        var allThreads = new List<int>();
        for (int i = 0; i < threadsAmount; i++)
        {
            allThreads.Add(rowsPerOneThread);
        }
        allThreads[^1] += rows % threadsAmount;

        var startedThreadsAmount = 0;
        var resetEvent = new ManualResetEvent(false);
        
        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        double[] rowMax = new double[rows];
        for (int threadI = 0; threadI < threadsAmount; threadI++)
        {
            var currentI = threadI;
            var thread = new Thread(() =>
            {
                for (int i = currentI * rowsPerOneThread; i < currentI * rowsPerOneThread + allThreads[currentI]; i++)
                {
                    double maxInRow = double.MinValue;
                    for (int j = 0; j < cols; j++)
                    {
                        if (matrix[i, j] > maxInRow)
                        {
                            maxInRow = matrix[i, j];
                        }
                    }
                    rowMax[i] = maxInRow;
                }

                
                if (Interlocked.Increment(ref startedThreadsAmount) == threadsAmount)
                {
                    resetEvent.Set();
                }
            });
            
            thread.Start();
        }

        resetEvent.WaitOne();
        resetEvent.Reset();
        startedThreadsAmount = 0;
        var lockObject = new object();
        
        double minimax = rowMax[0];
        for (int i = 0; i < threadsAmount; i++)
        {
            var currentI = i;
            var thread = new Thread(() =>
            {
                for (int j = currentI * rowsPerOneThread; j < currentI * rowsPerOneThread + allThreads[currentI]; j++)
                {
                    if (rowMax[j] < minimax)
                    {
                        lock (lockObject)
                        {
                            minimax = rowMax[j];
                        }
                    }
                }
                
                if (Interlocked.Increment(ref startedThreadsAmount) == threadsAmount)
                {
                    resetEvent.Set();
                }
            });
            
            thread.Start();
        }
        
        resetEvent.WaitOne();
        stopwatch.Stop();
        
        return (minimax, stopwatch.ElapsedTicks, threadsAmount);
    }
}