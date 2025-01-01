using System;
using System.Collections.Generic;
using System.Linq;
using Point = AdventOfCode.Utilities.Point;

namespace AdventOfCode.Y2023;

public class Day11 : Day
{
    private const char Space = '.';

    [ExpectedResult(374L, 9795148L)]
    public override object SolvePartOne()
    {
        var galaxies = this.GetGalaxiesInExpandedUniverse(2);
        return GetTotalDistance(galaxies);
    }

    [ExpectedResult(8410L, 650672493820L)]
    public override object SolvePartTwo()
    {
        var expansionFactor = ExecutionContext.Mode == ExecutionMode.Default ? 1_000_000 : 100;
        var galaxies = this.GetGalaxiesInExpandedUniverse(expansionFactor);
        return GetTotalDistance(galaxies);
    }

    private static long GetTotalDistance(Dictionary<int, Point> galaxies)
    {
        var keys = galaxies.Keys.ToList();
        var total = 0L;
        for (var i = 0; i < keys.Count; i++)
        {
            for (var j = i + 1; j < keys.Count; j++)
            {
                var p1 = galaxies[keys[i]];
                var p2 = galaxies[keys[j]];
                var distance = Math.Abs(p1.Column - p2.Column) + Math.Abs(p1.Row - p2.Row);
                total += distance;
            }
        }
        
        return total;
    }

    private Dictionary<int, Point> GetGalaxiesInExpandedUniverse(int expansionFactor)
    {
        var (universe, emptyRows, emptyColumns) = this.GetUniverse();
        
        var id = 0;
        var galaxies = new Dictionary<int, Point>();
        for (var rowIndex = 0; rowIndex < universe.Length; rowIndex++)
        {
            var row = universe[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                if (row[columnIndex] == Space) continue;

                var exColumn = columnIndex + emptyColumns.Count(c => c < columnIndex) * (expansionFactor - 1);
                var exRow = rowIndex + emptyRows.Count(r => r < rowIndex) * (expansionFactor - 1);
                galaxies.Add(id++, new Point(exColumn, exRow));
            }
        }

        return galaxies;
    }

    private (string[] universe, List<int> emptyRows, List<int> emptyColumns) GetUniverse()
    {
        var universe = this.GetInputLines().ToArray();
        
        var emptyRows = new List<int>();
        for (var rowIndex = 0; rowIndex < universe.Length; rowIndex++)
        {
            var line = universe[rowIndex];
            if (line.All(c => c == Space))
            {
                emptyRows.Add(rowIndex);
            }
        }
        
        var emptyColumns = new List<int>();
        var maxColumnIndex = universe[0].Length;
        for (var columnIndex = 0; columnIndex < maxColumnIndex; columnIndex++)
        {
            if (universe.Select(l => l[columnIndex]).All(c => c == Space))
            {
                emptyColumns.Add(columnIndex);
            }
        }

        return (universe, emptyRows, emptyColumns);
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               ...#......
               .......#..
               #.........
               ..........
               ......#...
               .#........
               .........#
               ..........
               .......#..
               #...#.....
               """;
    }
}