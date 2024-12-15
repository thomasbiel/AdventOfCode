using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Y2024;

public class Day5 : Day
{
    private readonly List<(int, int)> pageOrder = [];
    private readonly string[] updates;
    
    public Day5()
    {
        var lines = this.GetInputLines().ToArray();
        var separator = Array.IndexOf(lines, string.Empty);
        var lineSpan = lines.AsSpan();
        
        this.updates = lineSpan[(separator + 1)..].ToArray();
        
        var ordering = lineSpan[..separator];
        foreach (var order in ordering)
        {
            var ids = order.Split('|');
            
            int Get(int i) => int.Parse(ids[i]);
            
            this.pageOrder.Add((Get(0), Get(1)));
        }
    }
    
    [ExpectedResult(143, 4578)]
    public override object SolvePartOne()
    {
        var sum = 0;
        foreach (var line in this.updates)
        {
            var pages = line.Split(',').Select(int.Parse).ToArray();
            if (this.UpdateIsValid(pages))
            {
                VerifyUpdatePageCount(pages);
                var index = (pages.Length + 1) / 2 - 1;
                sum += pages[index];
            }
        }
        
        return sum;
    }

    [ExpectedResult(123, 6179)]
    public override object SolvePartTwo()
    {
        var comparer = new PageComparer(this.pageOrder); 
        
        var sum = 0;
        foreach (var line in this.updates)
        {
            var pages = line.Split(',').Select(int.Parse).ToArray();
            if (!this.UpdateIsValid(pages))
            {
                VerifyUpdatePageCount(pages);
                var ordered = pages.OrderBy(p => p, comparer).ToArray();
                this.DebugOut($"{ListPages(pages)} => {ListPages(ordered)}");
                    
                var index = (ordered.Length + 1) / 2 - 1;
                sum += ordered[index];
            }
        }
        
        return sum;
    }

    private bool UpdateIsValid(int[] pages)
    {
        for (var i = 1; i < pages.Length; i++)
        {
            var page = pages[i];
            var order = this.pageOrder.Where(o => o.Item1 == page);
                
            for (var j = 0; j < i; j++)
            {
                var previous = pages[j];
                if (order.Any(o => o.Item2 == previous))
                {
                    this.DebugOut($"Update {ListPages(pages)} is invalid");
                    return false;
                }
            }
        }

        return true;
    }

    private static string ListPages(int[] pages) => string.Join(", ", pages);

    private static void VerifyUpdatePageCount(int[] pages)
    {
        if (pages.Length % 2 != 1) throw new InvalidDataException(string.Join(", ", pages));
    }

    private class PageComparer(List<(int, int)> pageOrder) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (pageOrder.Any(o => o.Item1 == x && o.Item2 == y))
            {
                return -1;
            }
            
            if (pageOrder.Any(o => o.Item1 == y && o.Item2 == x))
            {
                return 1;
            }
            
            return 0;
        }
    }
    
    protected override string GetTestInput(int? part = null)
    {
        return """
               47|53
               97|13
               97|61
               97|47
               75|29
               61|13
               75|53
               29|13
               97|29
               53|29
               61|53
               97|53
               61|29
               47|13
               75|47
               97|75
               47|61
               75|61
               47|29
               75|13
               53|13

               75,47,61,53,29
               97,61,53,29,13
               75,29,13
               75,97,47,61,53
               61,13,29
               97,13,75,29,47
               """;
    }
}