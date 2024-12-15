using System.Collections.Generic;

namespace AdventOfCode.Y2024;

public class Day11 : Day
{
    private readonly Dictionary<string, long> cache = new();
    
    private long GetStonesAfter(string number, long blinks)
    {
        if (blinks <= 0) return this.Cache(blinks, number, 1);
        if (this.cache.TryGetValue($"{blinks}:{number}", out var result)) return result;
        
        if (number == "0")
        {
            return this.Cache(blinks, number, this.GetStonesAfter("1", blinks - 1));
        }

        if (number.Length % 2 == 0)
        {
            var firstHalf = number[..(number.Length / 2)];
            var secondHalf = number[(number.Length / 2)..].TrimStart('0').PadLeft(1, '0');
            var value = this.GetStonesAfter(firstHalf, blinks - 1L) + this.GetStonesAfter(secondHalf, blinks - 1L);
            return this.Cache(blinks, number, value);
        }

        return this.Cache(blinks, number, this.GetStonesAfter($"{long.Parse(number) * 2024}", blinks - 1L));
    }
    
    [ExpectedResult(55312L, 191690L)]
    public override object SolvePartOne() => this.CountStones(25);
                                          
    [ExpectedResult(65601038650482, 228651922369703)] // no test...
    public override object SolvePartTwo() => this.CountStones(75);

    private long Cache(long depth, string number, long value) => this.cache[$"{depth}:{number}"] = value;

    private long CountStones(int blinks)
    {
        var stones = this.CreateStones();
        
        var count = 0L;
        foreach (var stone in stones)
        {
            count += this.GetStonesAfter(stone, blinks);
        }

        return count;
    }

    private string[] CreateStones() => this.GetInput().TrimEnd().Split(' ');
    
    protected override string GetTestInput(int? part = null) => "125 17";
}