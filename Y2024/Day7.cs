using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day7 : Day
{
    private sealed record Operation(string Name, Func<ulong, ulong, ulong> Evaluate);
    
    private static readonly Operation Add = new("+", (x, y) => x + y);
    private static readonly Operation Multiply = new("*", (x, y) => x * y);
    private static readonly Operation Concatenate = new("||", (x, y) => x * (uint)Math.Pow(10, GetDigitCount(y)) + y);

    private sealed record Equation(ulong Result, ulong[] Values)
    {
        public bool Evaluate(IEnumerable<Operation> operations)
        {
            var i = 0;
            var value = this.Values[i];
            foreach (var op in operations)
            {
                var next = this.Values[++i];
                value = op.Evaluate(value, next);
            }
            
            return value == Result;
        }
    }

    private readonly IReadOnlyList<Equation> equations;
    
    public Day7()
    {
        static ulong Int(string value) => ulong.Parse(value);

        var options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        var list = new List<Equation>();
        var lines = GetInputLines();
        foreach (var line in lines)
        {
            var i = line.IndexOf(':');
            var id = Int(line[..i]);
            var values = line[(i + 1)..].Split(' ', options).Select(Int).ToArray();
            list.Add(new(id, values));
        }

        equations = list;
    }

    public override object SolvePartOne() => GetTotal([Add, Multiply]);

    public override object SolvePartTwo() => GetTotal([Add, Multiply, Concatenate]);

    protected override string GetTestInput(int? part = null)
    {
        return """
               190: 10 19
               3267: 81 40 27
               83: 17 5
               156: 15 6
               7290: 6 8 6 15
               161011: 16 10 13
               192: 17 8 14
               21037: 9 7 18 13
               292: 11 6 16 20
               """;
    }

    private ulong GetTotal(IReadOnlyList<Operation> operations)
    {
        ulong total = 0;
        foreach (var equation in this.equations)
        {
            var combinations = GetCombinations(operations, equation.Values.Length - 1);
            foreach (var combination in combinations)
            {
                if (equation.Evaluate(combination))
                {
                    var sb = new StringBuilder();
                    foreach (var (op, value) in combination.Zip(equation.Values, (op, value) => (op.Name, value)))
                    {
                        sb.Append(value);
                        sb.Append(' ');
                        sb.Append(op);
                        sb.Append(' ');
                    }
                    
                    sb.Append(equation.Values[^1]);
                    sb.Append(" = ");
                    sb.Append(equation.Result);
                    
                    this.DebugOut(sb.ToString());
                    total += equation.Result;
                    break;
                }
            }
        }

        return total;
    }

    private static int GetDigitCount(ulong number)
    {
        if (number == 0) return 1;
        return (int)Math.Floor(Math.Log10(number) + 1);
    }

    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(IReadOnlyList<T> list, int length)
    {
        if (length == 0)
        {
            yield return Array.Empty<T>();
            yield break;
        }

        foreach (var item in list)
        {
            var tailCombinations = GetCombinations(list, length - 1);
            foreach (var tail in tailCombinations)
            {
                yield return new[] { item }.Concat(tail);
            }
        }
    }
}