using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day1 : Day<int>
{
    private readonly List<int> left = new();
    private readonly List<int> right = new();

    public Day1()
    {
        foreach (var line in GetInputLines())
        {
            var parts = line.Split("   ", StringSplitOptions.RemoveEmptyEntries);
            
            void Add(List<int> list, int i) => list.Add(int.Parse(parts[i]));
            
            Add(left, 0);
            Add(right, 1);
        }
        
        left.Sort();
        right.Sort();
    }
    
    public override int SolvePartOne()
    {
        var diff = 0;
        for (var i = 0; i < left.Count; i++)
        {
            diff += Math.Abs(right[i] - left[i]);
        }
        
        return diff;
    }

    public override int SolvePartTwo()
    {
        var score = 0;
        for (var i = 0; i < this.left.Count; i++)
        {
            var value = this.left[i];
            var count = this.right.Count(v => v == value);
            score += value * count;
        }
        
        return score;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               3   4
               4   3
               2   5
               1   3
               3   9
               3   3
               """;
    }
}