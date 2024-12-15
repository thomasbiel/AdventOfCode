using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023;

public class Day3 : Day
{
    private readonly string[] engineLayout;
    private readonly IReadOnlyList<Symbol> symbols;

    private sealed record Symbol(char Name, int Column, int Row);

    public Day3()
    {
        this.engineLayout = this.GetInputLines().ToArray();
        var list = new List<Symbol>();
        for (var i = 0; i < this.engineLayout.Length; i++)
        {
            var line = this.engineLayout[i];
            var matches = Regex.Matches(line, @"[^.\d]");
            foreach (Match m in matches)
            {
                list.Add(new Symbol(m.Value[0], m.Index, i));
            }
        }

        this.symbols = list;
    }
    
    [ExpectedResult(4361, 557705)]
    public override object SolvePartOne()
    {
        var result = 0;
        for (var row = 0; row < this.engineLayout.Length; row++)
        {
            var line = this.engineLayout[row];
            foreach (Match m in Regex.Matches(line, "\\d+"))
            {
                var r = row;
                var potentialSymbols = this.symbols.Where(s => Math.Abs(s.Row - r) < 2);
                if (potentialSymbols.Any(s => this.SymbolFitsToMatch(m, s)))
                {
                    result += int.Parse(m.Value);
                }
                else
                {
                    this.DebugOut($"{m.Value} is not adjacent to a symbol");
                }
            }
        }

        return result;
    }

    [ExpectedResult(467835, 84266818)]
    public override object SolvePartTwo()
    {
        var result = 0;
        
        var potentialGears = this.symbols.Where(s => s.Name == '*');
        foreach (var symbol in potentialGears)
        {
            var lines = this.engineLayout.Where((_, i) => i >= symbol.Row - 1 && i <= symbol.Row + 1); 
            var numbers = lines.SelectMany(l => Regex.Matches(l, "\\d+"))
                .Where(m => this.SymbolFitsToMatch(m, symbol))
                .Select(m => int.Parse(m.Value))
                .ToArray();

            if (numbers.Length == 2)
            {
                result += numbers[0] * numbers[1];
            }
        }
        
        return result;
    }
    
    protected override string GetTestInput(int? part = null)
    {
        return """
               467..114..
               ...*......
               ..35..633.
               ......#...
               617*......
               .....+.58.
               ..592.....
               ......755.
               ...$.*....
               .664.598..
               """;
    }

    private bool SymbolFitsToMatch(Match match, Symbol symbol)
    {
        var start = match.Index;
        var end = start + match.Length - 1;
        return start < symbol.Column && end >= symbol.Column - 1 ||
               start == symbol.Column ||
               start == symbol.Column + 1 ||
               end == symbol.Column ||
               end == symbol.Column - 1;
    }
}