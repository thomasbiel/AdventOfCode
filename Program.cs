using System;
using System.Linq;

namespace AdventOfCode;

public static class Program
{
    private sealed record DayOfYear(Type Type, int Year, int Day);
    
    public static void Main()
    {
        var days = typeof(Program).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IDay)))
            .Select(t => new DayOfYear(t, YearAttribute.GetYear(t), Day.GetNumber(t)))
            .Where(d => ExecutionContext.YearIsSelected(d.Year) && ExecutionContext.DayIsSelected(d.Day))
            .OrderBy(d => d.Year * 100 + d.Day);

        foreach (var d in days)
        {
            var day = (IDay)Activator.CreateInstance(d.Type) ?? throw new InvalidProgramException();
            Console.Write($"Year {d.Year} day {d.Day}");
            Console.WriteLine();
            Console.WriteLine($"Answer Part One: {day.SolvePartOne()}");
            Console.WriteLine($"Answer Part Two: {day.SolvePartTwo()}");
            Console.WriteLine();
        }
    }
}