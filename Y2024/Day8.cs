using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day8 : Day<int>
{
    private record Point(int Column, int Row);

    private sealed record Antinode(int Column, int Row) : Point(Column, Row)
    {
        public bool IsValid(int maxColumn, int maxRow) => this.Column >= 0 && this.Column <= maxColumn &&
                                                          this.Row >= 0 && this.Row <= maxRow;
    }
    
    private sealed record Antenna(char Name, int Column, int Row) : Point(Column, Row)
    {
        public IEnumerable<Antinode> GetAntinodes(Antenna a)
        {
            if (this == a || a.Name != this.Name) yield break;
            
            var vector = (this.Column - a.Column, this.Row - a.Row);

            Point p = this;
            while (true)
            {
                // resonance: return antinode on antenna
                yield return new Antinode(p.Column, p.Row);
                p = p with { Column = p.Column + vector.Item1, Row = p.Row + vector.Item2 };
            }
        }
    }
    
    private readonly HashSet<Antenna> antennas;
    private readonly int maxRow, maxColumn;

    public Day8()
    {
        var lines = this.GetInputLines();
        
        this.maxColumn = lines[0].Length - 1;
        this.maxRow = lines.Length - 1;
        
        var regex = new Regex("[a-zA-Z0-9]");
        var set = new HashSet<Antenna>();
        for (var i = 0; i < lines.Length; i++)
        {
            foreach (Match m in regex.Matches(lines[i]))
            {
                set.Add(new Antenna(m.Value[0], m.Index, i));
            }
        }

        this.antennas = set;
    }

    public override int SolvePartOne() => GetAntinodeCount(includeResonances: false);

    public override int SolvePartTwo() => GetAntinodeCount(includeResonances: true);

    protected override string GetTestInput(int? part = null)
    {
        return """
               ............
               ........0...
               .....0......
               .......0....
               ....0.......
               ......A.....
               ............
               ............
               ........A...
               .........A..
               ............
               ............
               """;
    }
    
    private int GetAntinodeCount(bool includeResonances)
    {
        var set = new HashSet<Antinode>();
        foreach (var first in this.antennas)
        {
            foreach (var second in this.antennas)
            {
                var antinodes = first.GetAntinodes(second);
                if (!includeResonances) antinodes = antinodes.Take(2);
                
                foreach (var antinode in antinodes)
                {
                    if (!includeResonances && antinode.Row == first.Row && antinode.Column == first.Column)
                    {
                        // w/o resonances: ignore antinode on antenna 
                        continue;
                    }
                    
                    if (!antinode.IsValid(this.maxColumn, this.maxRow)) break;
                    
                    if (set.Add(antinode))
                    {
                        this.DebugOut($"{set.Count}. {antinode}");
                    }
                }
            }
        }
        
        return set.Count;
    }
}