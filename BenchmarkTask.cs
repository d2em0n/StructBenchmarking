using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Avalonia.Controls.Primitives;
using NUnit.Framework;

namespace StructBenchmarking;
public class Benchmark : IBenchmark
{
    public double MeasureDurationInMs(ITask task, int repetitionCount)
    {
        GC.Collect();                   // Эти две строчки нужны, чтобы уменьшить вероятность того,
        GC.WaitForPendingFinalizers();  // что Garbadge Collector вызовется в середине измерений
                                        // и как-то повлияет на них.

        task.Run();
        var sw = new Stopwatch();
        sw.Restart();
        for (int i = 0; i < repetitionCount; i++)
             task.Run(); 
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds / repetitionCount;
	}
}

public class StringBuilderTest : ITask
{
    public void Run()
    {
        var strBuilder = new StringBuilder();
        for (var i = 0; i < 10000; i++)
        {
            strBuilder.Append("a");
        }
        strBuilder.ToString();
    }
}

public class StringTest : ITask
{
    public void Run()
    {
        var str = new string('a', 10000);
    }
}


[TestFixture]
public class RealBenchmarkUsageSample
{ 
    [Test]
    public void StringConstructorFasterThanStringBuilder()
    {
        var benchmark = new Benchmark();       

        var stringFromBuilder = new StringBuilderTest();
        var stringSpecial = new StringTest();

        var timeBuilder = benchmark.MeasureDurationInMs(stringFromBuilder, 100000);
        var timeString = benchmark.MeasureDurationInMs(stringSpecial, 100000);
        Assert.Less(timeString, timeBuilder);
    }
}