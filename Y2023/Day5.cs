using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Y2023;

public class Day5 : Day
{
    private const string MapMarker = " map:";

    private record Map(long SourceRangeStart, long TargetRangeStart, long Range)
    {
        public bool IsMapped(long value) => value >= this.SourceRangeStart && value < this.SourceRangeStart + this.Range;
        
        public long Translate(long value) => this.TargetRangeStart + value - this.SourceRangeStart;
    }

    private readonly Dictionary<string, List<Map>> categoryMaps = new();
    private readonly long[] seedLists;
    private readonly string[] categories =
    [
        "seed-to-soil",
        "soil-to-fertilizer",
        "fertilizer-to-water",
        "water-to-light",
        "light-to-temperature",
        "temperature-to-humidity",
        "humidity-to-location"
    ];

    public Day5()
    {
        const int sourceIndex = 1;
        const int destinationIndex = 0;
        const int rangeIndex = 2;
        
        List<Map> maps = null;
        
        var i = 0;
        foreach (var line in this.GetInputLines())
        {
            if (i == 0) this.seedLists = line.Split(": ")[1].Split(" ").Select(long.Parse).ToArray();

            if (i > 1)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.Contains(MapMarker))
                {
                    maps = [];
                    this.categoryMaps.Add(line[..^MapMarker.Length], maps);
                }
                else
                {
                    var parts = line.Split(" ");
                    // order: destination - source - range
                    var source = Get(parts, sourceIndex);
                    var target = Get(parts, destinationIndex);
                    var range = Get(parts, rangeIndex);
                    var map = new Map(source, target, range);
                    maps!.Add(map);
                }
            }

            i++;
        }
    }

    [ExpectedResult(35L, 340994526L)]
    public override object SolvePartOne() => this.GetLowestSeedLocation(this.seedLists);

    [ExpectedResult(46L, 52210644L)]
    public override object SolvePartTwo() => this.GetLowestSeedLocation(this.GetSeeds());

    private IEnumerable<long> GetSeeds()
    {
        var ranges = this.seedLists.Length / 2;
        for (var i = 0; i < ranges; i++)
        {
            var offset = i * 2;
            var start = this.seedLists[offset];
            var range = this.seedLists[offset + 1];
            for (var seed = start; seed < start + range; seed++)
            {
                yield return seed;
            }            
        }
    }
    
    private long GetLowestSeedLocation(IEnumerable<long> seeds)
    {
        var min = long.MaxValue;
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        Parallel.ForEach(seeds, options, seed =>
        {
            var cache = new Dictionary<long, long>();
            if (!cache.TryGetValue(seed, out var location))
            {
                location = this.GetSeedLocation(seed);
                cache.Add(seed, location);
            }
            
            min = Math.Min(min, location);
        });

        this.DebugOut($"location {min}");
        return min;
    }

    private long GetSeedLocation(long seed)
    {
        this.DebugOut("seed " + seed);
        var value = seed;
        foreach (var category in this.categories)
        {
            var maps = this.categoryMaps[category];
            var map = maps.FirstOrDefault(m => m.IsMapped(value));
            var unmapped = value;
            var mapped = map?.Translate(unmapped) ?? value;
            this.DebugOut(() => $"{category}: {unmapped} => {mapped}");
            value = mapped;
        }
        
        return value;
    }

    private static long Get(string[] parts, int i) => long.Parse(parts[i]);
    
    protected override string GetTestInput(int? part = null)
    {
        return """
               seeds: 79 14 55 13

               seed-to-soil map:
               50 98 2
               52 50 48

               soil-to-fertilizer map:
               0 15 37
               37 52 2
               39 0 15

               fertilizer-to-water map:
               49 53 8
               0 11 42
               42 0 7
               57 7 4

               water-to-light map:
               88 18 7
               18 25 70

               light-to-temperature map:
               45 77 23
               81 45 19
               68 64 13

               temperature-to-humidity map:
               0 69 1
               1 0 69

               humidity-to-location map:
               60 56 37
               56 93 4
               """;
    }
}