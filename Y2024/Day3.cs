using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2024;

public class Day3 : Day
{
    [ExpectedResult(161, 178538786)]
    public override object SolvePartOne()
    {
        var memory = this.GetInput();
        return CalculateTotal(memory);
    }

    [ExpectedResult(48, 102467299)]
    public override object SolvePartTwo()
    {
        var memory = this.GetInput();

        // remove everything from "don't()" to "do()" from memory
        var cleansed = Regex.Replace(memory, @"don't\(\).*?do\(\)", string.Empty, RegexOptions.Singleline);
        var i = cleansed.IndexOf("don't()", StringComparison.Ordinal);
        if (i > -1)
        {
            cleansed = cleansed[..i];
        }
        
        return CalculateTotal(cleansed);
    }

    protected override string GetTestInput(int? part = null)
    {
        return part == 1
            ? "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"
            : "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
    }

    private static int CalculateTotal(string memory)
    {
        var matches = Regex.Matches(memory, @"mul\(([0-9]+),([0-9]+)\)");

        var total = 0;
        foreach (Match match in matches)
        {
            var x = int.Parse(match.Groups[1].Value);
            var y = int.Parse(match.Groups[2].Value);
            total += x * y;
        }

        return total;
    }
}