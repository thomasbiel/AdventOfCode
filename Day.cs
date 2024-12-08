using System;
using System.IO;

namespace AdventOfCode;

public abstract class Day : IDay
{
    protected const int PartOne = 1;
    protected const int PartTwo = 2;
    
    public abstract object SolvePartOne();
    
    public abstract object SolvePartTwo();

    public static int GetNumber(Type type)
    {
        var name = type.Name;
        return int.Parse(name[nameof(Day).Length..]);
    }
    
    protected abstract string GetTestInput(int? part = null);

    protected void DebugOut(string message)
    {
        if (ExecutionContext.Mode == ExecutionMode.Debug)
        {
            Console.WriteLine(message);
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

    private T GetInput<T>(Func<string, T> factory)
    {
        var type = this.GetType();
        var year = YearAttribute.GetYear(type);
        var day = GetNumber(type);
        return ExecutionContext.LoadInput(year, day, factory);
    }
}