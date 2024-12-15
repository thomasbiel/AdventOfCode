using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2024;

public class Day4 : Day
{
    private static readonly int SearchLength = "XMAS".Length;
    private static readonly string Pattern = "(?=(XMAS|SAMX))";
    
    private readonly string[] lines;
    private readonly int lineLength;

    public Day4()
    {
        this.lines = this.GetInputLines().ToArray();
        this.lineLength = this.lines[0].Length;
    }

    [ExpectedResult(18, 2390)]
    public override object SolvePartOne()
    {
        var count = this.Count();
        count += this.CountColumns();
        count += this.CountDiagonals();
        count += this.CountAntiDiagonals();
        return count;
    }
    
    [ExpectedResult(9, 1809)]
    public override object SolvePartTwo()
    {
        var pattern = "MAS";
        var reversed = "SAM";

        var diff = pattern.Length - 1;
        
        string Cut(string value) => value[..Math.Min(value.Length, pattern.Length)];
        
        var count = 0;
        for (var column = 0; column < this.lineLength - diff; column++)
        {
            for (var line = 0; line < this.LineCount - diff; line++)
            {
                var diagonal = Cut(this.GetDiagonal(line, column));
                var antiDiagonal = Cut(this.GetAntiDiagonal(line, column + diff));
                
                if (diagonal == pattern && antiDiagonal == reversed ||
                    diagonal == reversed && antiDiagonal == pattern ||
                    diagonal == pattern && antiDiagonal == pattern ||
                    diagonal == reversed && antiDiagonal == reversed)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private int CountDiagonals()
    {
        var length = this.lineLength - SearchLength;
        
        var count = 0;
        for (var column = length; column >= 0; column--)
        {
            var diagonal = this.GetDiagonal(0, column);
            count += Count(diagonal);
        }

        for (var line = 1; line < this.LineCount - SearchLength + 1; line++)
        {
            var diagonal = this.GetDiagonal(line, 0);
            count += Count(diagonal);
        }
        
        return count;
    }
    
    private string GetDiagonal(int line, int column)
    {
        var sb = new StringBuilder(this.lineLength);
            
        var i = column;
        var r = line;
        while (r < this.LineCount && i < this.lineLength)
        {
            sb.Append(this.lines[r][i]);
            r++;
            i++;
        }

        return sb.ToString();
    }
    
    private string GetAntiDiagonal(int line, int column)
    {
        var sb = new StringBuilder(this.lineLength);
            
        var i = column;
        var r = line;
        while (r < this.LineCount && i >= 0)
        {
            sb.Append(this.lines[r][i]);
            r++;
            i--;
        }

        return sb.ToString();
    }

    private int LineCount => this.lines.Length;

    private int CountAntiDiagonals()
    {
        var count = 0;
        
        for (var column = SearchLength; column < this.lineLength; column++)
        {
            var diagonal = this.GetAntiDiagonal(0, column);
            count += Count(diagonal);
        }

        var lastColumn = this.lineLength - 1;
        for (var line = 1; line < this.LineCount; line++)
        {
            var diagonal = this.GetAntiDiagonal(line, lastColumn);
            count += Count(diagonal);
        }
        
        return count;
    }

    private int CountColumns()
    {
        var count = 0;
        for (var i = 0; i < this.lineLength; i++)
        {
            var column = new string(this.lines.Select(l => l[i]).ToArray());
            count += Count(column);
        }

        return count;
    }

    private int Count()
    {
        var count = 0;
        foreach (var line in this.lines)
        {
            count += Count(line);
        }

        return count;
    }

    private static int Count(string line) => Regex.Count(line, Pattern);

    protected override string GetTestInput(int? part = null)
    {
        return """
               MMMSXXMASM
               MSAMXMSMSA
               AMXSXMAAMM
               MSAMASMSMX
               XMASAMXAMM
               XXAMMXXAMA
               SMSMSASXSS
               SAXAMASAAA
               MAMMMXMMMM
               MXMXAXMASX
               """;
    }
}