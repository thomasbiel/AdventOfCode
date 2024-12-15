using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2024;

public class Day1 : Day
{
    private readonly List<int> left = [];
    private readonly List<int> right = [];

    public Day1()
    {
        foreach (var line in this.GetInputLines())
        {
            var parts = line.Split("   ", StringSplitOptions.RemoveEmptyEntries);
            
            void Add(List<int> list, int i) => list.Add(int.Parse(parts[i]));
            
            Add(this.left, 0);
            Add(this.right, 1);
        }

        this.left.Sort();
        this.right.Sort();
    }
    
    [ExpectedResult(11, 1722302)]
    public override object SolvePartOne()
    {
        var diff = 0;
        for (var i = 0; i < this.left.Count; i++)
        {
            diff += Math.Abs(this.right[i] - this.left[i]);
        }
        
        return diff;
    }

    [ExpectedResult(31, 20373490)]
    public override object SolvePartTwo()
    {
        var score = 0;
        foreach (var value in this.left)
        {
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