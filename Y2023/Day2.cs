using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023;

[Year(2023)]
public class Day2 : Day<int>
{
    private sealed record CubeSet(int Red, int Green, int Blue)
    {
        public static readonly CubeSet Empty = new(0, 0, 0);
        
        public int GetPower() => this.Red * this.Green * this.Blue;
    }

    private sealed record Game(int Id, CubeSet[] Sets)
    {
        public bool IsPossible(int maxRed, int maxGreen, int maxBlue)
        {
            return this.Sets.All(s => s.Red <= maxRed && s.Green <= maxGreen && s.Blue <= maxBlue);
        }

        public CubeSet GetMinimum()
        {
            return this.Sets.Aggregate(
                CubeSet.Empty,
                (acc, set) => new CubeSet(
                    Math.Max(acc.Red, set.Red),
                    Math.Max(acc.Green, set.Green),
                    Math.Max(acc.Blue, set.Blue)));
        }
    }

    private readonly IReadOnlyList<Game> games;
    
    public Day2()
    {
        var idRegex = new Regex("Game (\\d*):");
        
        var list = new List<Game>();
        var lines = this.GetInputLines();
        foreach (var line in lines)
        {
            var content = line.Split(": ")[1];
            var parts = content.Split("; ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var cubeSets = GetCubeSets(parts);
            
            var id = int.Parse(idRegex.Match(line).Groups[1].Value);
            var game = new Game(id, cubeSets.ToArray());
            
            list.Add(game);
        }

        this.games = list;
    }
    
    public override int SolvePartOne()
    {
        var result = 0;
        foreach (var game in this.games)
        {
            if (game.IsPossible(maxRed: 12, maxGreen: 13, maxBlue: 14))
            {
                result += game.Id;
            }
            else
            {
                this.DebugOut($"{game} is impossible");
            }
        }

        return result;
    }

    public override int SolvePartTwo()
    {
        var result = 0;
        foreach (var game in this.games)
        {
            var minimum = game.GetMinimum();
            this.DebugOut($"{game} has minimum {minimum}");
            result += minimum.GetPower();
        }

        return result;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
               Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
               Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
               Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
               Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
               """;
    }
    
    private static IEnumerable<CubeSet> GetCubeSets(string[] colorSets)
    {
        foreach (var colorSet in colorSets)
        {
            var cubeSet = CubeSet.Empty;
            var colors = colorSet.Split(", ");
            foreach (var entry in colors)
            {
                var s = entry.Split(' ');
                var count = int.Parse(s[0]);
                var color = s[1];
                cubeSet = color switch
                {
                    "red" => cubeSet with { Red = count },
                    "green" => cubeSet with { Green = count },
                    "blue" => cubeSet with { Blue = count },
                    _ => cubeSet
                };
            }
            
            yield return cubeSet;
        }
    }
}