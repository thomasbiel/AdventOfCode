using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day2 : Day<int>
{
    public override int SolvePartOne() => Solve(PartOne);

    private int Solve(int part)
    {
        var lines = this.GetInputLines();
        
        var safeCount = 0;
        foreach (var line in lines)
        {
            var levels = line.Split(" ").Select(int.Parse).ToArray();

            if (IsReportSafe(levels))
            {
                safeCount++;
            }
            else if (part == 2)
            {
                for (var i = 0; i < levels.Length; i++)
                {
                    var dampened = levels.ToList();
                    dampened.RemoveAt(i);

                    if (IsReportSafe(dampened))
                    {
                        safeCount++;
                        break;
                    }
                }
            }
        }

        return safeCount;
    }

    private static bool IsReportSafe(IReadOnlyList<int> levels)
    {
        var first = levels[0];
        var last = first;
        var increasing = levels[1] > first;
        var isSafe = true;
        for (var i = 1; i < levels.Count; i++)
        {
            var current = levels[i];
            if (increasing)
            {
                if (current <= last)
                {
                    isSafe = false;
                    break;
                }
            }
            else
            {
                if (current >= last)
                {
                    isSafe = false;
                    break;
                }
            }
                
            if (Math.Abs(current - last) > 3)
            {
                isSafe = false;
                break;
            }
                
            last = levels[i];
        }

        return isSafe;
    }

    public override int SolvePartTwo() => Solve(PartTwo);

    protected override string GetTestInput(int? part = null)
    {
        return """
               7 6 4 2 1
               1 2 7 8 9
               9 7 6 2 1
               1 3 2 4 5
               8 6 4 4 1
               1 3 6 7 9
               """;
    }
}