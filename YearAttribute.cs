using System;
using System.Reflection;

namespace AdventOfCode;

[AttributeUsage(AttributeTargets.Class)]
public sealed class YearAttribute(int year) : Attribute
{
    public int Year { get; } = year;
    
    public static int GetYear(Type type)
    {
        var attribute = type.GetCustomAttribute<YearAttribute>();
        return attribute?.Year ?? throw new ArgumentException($"{type} has no year attribute");
    }
}