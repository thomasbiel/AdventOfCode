using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023;

public class Day9 : Day
{
    [ExpectedResult(114L, 1725987467L)]
    public override object SolvePartOne()
    {
        var sum = 0L;
        foreach (var numbers in this.GetInputSequences())
        {
            var sequences = new List<List<int>> { numbers };

            var diff = GetLastDifference(numbers, sequences);
            for (var i = sequences.Count - 1; i >= 0; i--)
            {
                var sequence = sequences[i];
                var last = sequence[^1];
                var next = last + diff;
                diff = next;
                
                if (i == 0) sum += next;
            }
        }
        
        return sum;
    }

    [ExpectedResult(2L, 971L)]
    public override object SolvePartTwo()
    {
        var sum = 0L;
        foreach (var numbers in this.GetInputSequences())
        {
            var sequences = new List<List<int>> { numbers };

            var diff = GetLastDifference(numbers, sequences);
            for (var i = sequences.Count - 1; i >= 0; i--)
            {
                var sequence = sequences[i];
                var first = sequence[0];
                var previous = first - diff;
                diff = previous;
                
                if (i == 0) sum += previous;
            }
        }
        
        return sum;
    }

    private IEnumerable<List<int>> GetInputSequences()
    {
        foreach (var line in this.GetInputLines())
        {
            yield return line.Split(' ').Select(int.Parse).ToList();
        }
    }
    
    private static List<int> GetDifferences(List<int> numbers)
    {
        var diffs = new List<int>();
        for (var i = 0; i < numbers.Count - 1; i++)
        {
            diffs.Add(numbers[i + 1] - numbers[i]);
        }

        return diffs;
    }

    private static int GetLastDifference(List<int> numbers, List<List<int>> sequences)
    {
        int? diff = null;
        
        var list = numbers;
        while (list.Count > 1)
        {
            var diffs = GetDifferences(list);
            // stop if all diffs are equal, also no need to continue to 0s
            if (diffs.All(n => n == diffs[0]))
            {
                diff = diffs[0];
                break;
            }
                
            sequences.Add(diffs);
            list = diffs;
        }

        return diff ?? throw new InvalidOperationException($"Error in line {string.Join(' ', numbers)}");
    }
    
    protected override string GetTestInput(int? part = null)
    {
        return """
               0 3 6 9 12 15
               1 3 6 10 15 21
               10 13 16 21 30 45
               """;
    }
}