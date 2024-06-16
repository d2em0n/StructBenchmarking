using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace StructBenchmarking;

enum TaskType
{
    Allocation,
    Call
}

interface IFabric
{
    public ITask CreateTask(int field, TaskType taskType);
}

class ClassFabric : IFabric
{
    public ITask CreateTask(int field, TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.Allocation:
                return new ClassArrayCreationTask(field);

            case TaskType.Call:
                return new MethodCallWithClassArgumentTask(field);

            default:
                return null;
        }
    }
}

class StructFabric : IFabric
{
    public ITask CreateTask(int field, TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.Allocation:
                return new StructArrayCreationTask(field);

            case TaskType.Call:
                return new MethodCallWithStructArgumentTask(field);

            default:
                return null;
        }
    }
}

public class Experiments
{
    private static List<ExperimentResult> PrepareData(IFabric fabric, IBenchmark benchmark,
        int repetitionsCount, TaskType taskType)
    {
        var dots = new List<ExperimentResult>();

        foreach (var field in Constants.FieldCounts)
        {
            dots.Add(new ExperimentResult(field,
                benchmark.MeasureDurationInMs(fabric.CreateTask(field, taskType), repetitionsCount)));
        }
        return dots;
    }

    public static ChartData BuildChartDataForArrayCreation(
        IBenchmark benchmark, int repetitionsCount)
    {
        return new ChartData
        {
            Title = "Create array",
            ClassPoints = PrepareData(new ClassFabric(), benchmark, repetitionsCount, TaskType.Allocation),
            StructPoints = PrepareData(new StructFabric(), benchmark, repetitionsCount, TaskType.Allocation),
        };
    }

    public static ChartData BuildChartDataForMethodCall(
        IBenchmark benchmark, int repetitionsCount)
    { 
        return new ChartData
        {
            Title = "Call method with argument",
            ClassPoints = PrepareData(new ClassFabric(), benchmark, repetitionsCount, TaskType.Call),
            StructPoints = PrepareData(new StructFabric(), benchmark, repetitionsCount, TaskType.Call),
        };
    }
}