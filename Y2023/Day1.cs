using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023;

[Year(2023)]
public class Day1 : Day<int>
{
    private static readonly IReadOnlyDictionary<string, int> translation = new Dictionary<string, int>
    {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 }
    };
    
    public override int SolvePartOne() => this.Solve(1, @"[\d]");

    // word "digits" may overlap, e.g. "eighthree" -> 83
    public override int SolvePartTwo() => this.Solve(2, @"(?=([\d]|" + string.Join('|', translation.Keys) + "))");
    
    private int Solve(int part, string pattern)
    {
        var result = 0;
        foreach (var line in this.GetInputLines(part))
        {
            static int GetValue(Match m)
            {
                var s = !string.IsNullOrEmpty(m.Value) ? m.Value : m.Groups.Cast<Group>().First(g => g.GetType() == typeof(Group)).Value;
                return s.Length == 1 ? int.Parse(s) : translation[s];
            }
            
            var matches = Regex.Matches(line, pattern);
            var first = GetValue(matches[0]);
            var last = GetValue(matches[^1]);
            result += first * 10 + last;
        }
        
        return result;
    }

    protected override string GetTestInput(int? part = null)
    {
        if (part != 1)
        {
            return """
                   two1nine
                   eightwothree
                   abcone2threexyz
                   xtwone3four
                   4nineeightseven2
                   zoneight234
                   7pqrstsixteen
                   """;
        }
        
        return """
               1abc2
               pqr3stu8vwx
               a1b2c3d4e5f
               treb7uchet
               """;
    }
}