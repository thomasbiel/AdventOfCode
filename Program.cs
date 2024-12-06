using System;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode;

public static class Program
{
    public static void Main()
    {
        var days = typeof(Program).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IDay)))
            .Select(t => new DayOfYear(t, YearAttribute.GetYear(t), DayOfYear.GetNumber(t)))
            .Where(d => ExecutionContext.YearIsSelected(d.Year) && ExecutionContext.DayIsSelected(d.Day))
            .OrderBy(d => d.Year * 100 + d.Day);

        foreach (var d in days)
        {
            var (day, duration) = Measure(() => (IDay)Activator.CreateInstance(d.Type) ?? throw new InvalidProgramException());
            Console.Write($"Year {d.Year} day {d.Day} (warmup took {duration})");
            Console.WriteLine();
            var partOne = Measure(day.SolvePartOne);
            Console.WriteLine($"Answer Part One: {partOne.Result} in {partOne.Duration}");
            var partTwo = Measure(day.SolvePartTwo);
            Console.WriteLine($"Answer Part Two: {partTwo.Result} in {partTwo.Duration}");
            Console.WriteLine();
        }
    }

    private static (T Result, TimeSpan Duration) Measure<T>(Func<T> solver)
    {
        var sw = Stopwatch.StartNew();
        var result = solver();
        return (result, sw.Elapsed);
    }
}