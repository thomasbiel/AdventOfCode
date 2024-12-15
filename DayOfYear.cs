using System;
using System.Linq;

namespace AdventOfCode;

public class DayOfYear
{
    public DayOfYear(Type type)
    {
        this.Type = type;
        
        var ns = type.Namespace!.Split('.');
        var year = ns[^1].TrimStart('Y');
        this.Year = int.Parse(year);

        var name = type.Name;
        this.DayOfMonth = int.Parse(name["Day".Length..]);
    }

    public Type Type { get; }
    
    public int DayOfMonth { get; }

    public int Year { get; }

    public Day Create() => (Day)Activator.CreateInstance(this.Type) ?? throw new InvalidProgramException();

    public static IOrderedEnumerable<DayOfYear> GetDays()
    {
        return typeof(Program).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.BaseType == typeof(Day))
            .Select(t => new DayOfYear(t))
            .Where(d => ExecutionContext.YearIsSelected(d.Year) && ExecutionContext.DayIsSelected(d.DayOfMonth))
            .OrderBy(d => d.Year * 100 + d.DayOfMonth);
    }
}