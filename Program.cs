using System;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode;

public static class Program
{
    public static void Main()
    {
        foreach (var d in DayOfYear.GetDays())
        {
            var (day, duration) = Measure(() => d.Create());
            Console.Write($"Year {d.Year} day {d.DayOfMonth} (warmup took {duration})");
            Console.WriteLine();
            
            var solvePartOne = Get(d.Type, nameof(day.SolvePartOne));
            var partOne = Measure(() => day.SolvePartOne());
            Console.WriteLine($"Answer Part One: {partOne.Result} in {partOne.Duration} {Verify(solvePartOne, partOne.Result)}");
            
            var solvePartTwo = Get(d.Type, nameof(day.SolvePartTwo));
            var partTwo = Measure(() => day.SolvePartTwo());
            Console.WriteLine($"Answer Part Two: {partTwo.Result} in {partTwo.Duration} {Verify(solvePartTwo, partTwo.Result)}");
            Console.WriteLine();
        }
    }

    private static (T Result, TimeSpan Duration) Measure<T>(Func<T> method)
    {
        var sw = Stopwatch.StartNew();
        var result = method();
        return (result, sw.Elapsed);
    }
    
    private static string Verify(MethodInfo method, object result)
    {
        var expected = method!.GetCustomAttribute<ExpectedResultAttribute>();
        if (expected != null)
        {
            var value = ExecutionContext.Mode == ExecutionMode.Default ? expected.Value : expected.Test;
            if (value == null) return "(tbd)";
                
            if (!value.Equals(result))
            {
                Console.WriteLine($"Expected {value} but got {result}");
                return "(error)";
            }
            
            return "(verified)";
        }
        
        return string.Empty;
    }
    
    private static MethodInfo Get(Type type, string methodName)
    {
        return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
    }
}