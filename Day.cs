using System;
using System.IO;

namespace AdventOfCode;

public abstract class Day<T> : IDay
{
    protected const int PartOne = 1;
    protected const int PartTwo = 2;

    object IDay.SolvePartOne() => this.SolvePartOne();
    
    object IDay.SolvePartTwo() => this.SolvePartTwo();
    
    public abstract T SolvePartOne();
    
    public abstract T SolvePartTwo();
    
    protected abstract string GetTestInput(int? part = null);

    protected void DebugOut(string message) => this.DebugOut(() => message);
    
    protected void DebugOut(Func<string> getMessage)
    {
        if (ExecutionContext.Mode == ExecutionMode.Debug)
        {
            Console.WriteLine(getMessage());
        }
    }
    
    protected string[] GetInputLines(int? part = null)
    {
        return ExecutionContext.Mode == ExecutionMode.Default
            ? GetInput(File.ReadAllLines)
            : GetTestInput(part).Split("\n", StringSplitOptions.TrimEntries);
    }

    protected string GetInput(int? part = null)
    {
        return ExecutionContext.Mode == ExecutionMode.Default
            ? GetInput(File.ReadAllText)
            : GetTestInput(part);
    }

    private TResult GetInput<TResult>(Func<string, TResult> factory)
    {
        var type = this.GetType();
        var year = YearAttribute.GetYear(type);
        var day = DayOfYear.GetNumber(type);
        return ExecutionContext.LoadInput(year, day, factory);
    }
}