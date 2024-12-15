using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2023;

public class Day8 : Day
{
    private readonly Dictionary<string,Node> rules = new();
    private readonly string instructions;

    private readonly record struct Node(string Left, string Right);

    public Day8()
    {
        var i = 0;
        foreach (var line in this.GetInputLines())
        {
            if (i == 0) this.instructions = line;
            
            if (i > 1)
            {
                var parts = line.Split(' ');
                var id = parts[0];
                var left = parts[2][1..^1];
                var right = parts[3][..^1];
                this.rules.Add(id, new Node(left, right));
            }

            i++;
        }
    }
    
    [ExpectedResult(6L, 20659L)]
    public override object SolvePartOne() => this.GetSteps("AAA", id => id != "ZZZ");

    [ExpectedResult(6L, 15690466351717L)]
    public override object SolvePartTwo()
    {
        var paths = this.rules.Keys.Where(id => id.EndsWith('A')).ToList();
        var steps = paths.Select(p => this.GetSteps(p, c => !c.EndsWith('Z'))).ToList();  
        return GetLeastCommonMultiple(steps);
    }
    
    private static long GetLeastCommonMultiple(List<long> numbers)
    {
        return numbers.Aggregate((a, b) => a.GetLeastCommonMultiple(b));
    }

    private int Next(int i)
    {
        var next = i + 1;
        return next < this.instructions.Length ? next : 0;
    }

    private long GetSteps(string start, Func<string, bool> @continue)
    {
        var current = start;
        var steps = 0L;
        var i = 0;
        while (@continue(current))
        {
            var rule = this.rules[current];
            current = this.Proceed(i, rule);
            steps++;
            
            i = this.Next(i);
        }
        
        return steps;
    }

    private string Proceed(int i, Node rule) => this.instructions[i] == 'L' ? rule.Left : rule.Right;

    protected override string GetTestInput(int? part = null)
    {
        return """
               LLR

               AAA = (BBB, BBB)
               BBB = (AAA, ZZZ)
               ZZZ = (ZZZ, ZZZ)
               """;
    }
}