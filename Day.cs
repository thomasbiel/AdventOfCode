using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode;

public abstract class Day
{
    protected const int PartOne = 1;
    protected const int PartTwo = 2;

    public abstract object SolvePartOne();
    
    public abstract object SolvePartTwo();
    
    protected abstract string GetTestInput(int? part = null);

    protected void DebugOut(string message) => this.DebugOut(() => message);
    
    protected void DebugOut(Func<string> getMessage)
    {
        if (ExecutionContext.Mode == ExecutionMode.Debug)
        {
            Console.WriteLine(getMessage());
        }
    }
    
    protected IEnumerable<string> GetInputLines(int? part = null)
    {
        return ExecutionContext.Mode == ExecutionMode.Default
            ? this.GetInput(ReadAllLines)
            : this.GetTestInput(part).Split("\n", StringSplitOptions.TrimEntries);
    }

    protected string GetInput(int? part = null)
    {
        return ExecutionContext.Mode == ExecutionMode.Default
            ? this.GetInput(File.ReadAllText)
            : this.GetTestInput(part);
    }

    private TResult GetInput<TResult>(Func<string, TResult> factory) => ExecutionContext.LoadInput(this, factory);

    private static IEnumerable<string> ReadAllLines(string path)
    {
        var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }
}