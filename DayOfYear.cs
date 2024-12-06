using System;

namespace AdventOfCode;

public sealed record DayOfYear(Type Type, int Year, int Day)
{
    public static int GetNumber(Type type)
    {
        var name = type.Name;
        return int.Parse(name["Day".Length..]);
    }
}