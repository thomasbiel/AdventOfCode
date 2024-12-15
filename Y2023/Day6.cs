using System;
using System.Linq;

namespace AdventOfCode.Y2023;

public class Day6 : Day
{
    [ExpectedResult(288L, 588588L)]
    public override object SolvePartOne()
    {
        static int[] GetValues(string line) => line[10..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        
        var lines = this.GetInputLines().ToArray();
        var times = GetValues(lines[0]);
        var distances = GetValues(lines[1]);
        var races = times.Zip(distances, (time, distance) => (time, distance));
        
        var result = 1L;
        foreach (var (time, distance) in races)
        {
            result *= DetermineWaysToWin(time, distance);
        }
        
        return result;
    }

    [ExpectedResult(71503L, 34655848L)]
    public override object SolvePartTwo()
    {
        static long GetValue(string line) => long.Parse(line[10..].Replace(" ", string.Empty));
        
        var lines = this.GetInputLines().ToArray();
        var time = GetValue(lines[0]);
        var distance = GetValue(lines[1]);
        
        return DetermineWaysToWin(time, distance);
    }

    private static long DetermineWaysToWin(long maxTime, long maxDistance)
    {
        var wins = 0;
        for (var i = 1L; i < maxTime - 1; i++)
        {
            var speed = i;
            var time = maxTime - i;
            var distance = speed * time;
            if (distance > maxDistance) wins++;
        }

        return wins;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               Time:      7  15   30
               Distance:  9  40  200
               """;
    }
}