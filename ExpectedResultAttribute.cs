using System;

namespace AdventOfCode;

[AttributeUsage(AttributeTargets.Method)]
public class ExpectedResultAttribute(object test, object value = null) : Attribute
{
    public object Test { get; } = test;
    
    public object Value { get; } = value;
}