using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

public class Day10 : Day
{
    private class Map
    {
        private const int Trailhead = 0;
        private const int Peak = 9;
        private readonly Dictionary<Point, int> map = new();

        public Map(IEnumerable<string> lines)
        {
            var row = 0;
            foreach (var line in lines)
            {
                for (var column = 0; column < line.Length; column++)
                {
                    var c = line[column];
                    this.map[new Point(column, row)] = c - '0';
                }

                row++;
            }
        }
        
        private class Trail : List<Point>
        {
            public Trail() { }
            
            public Trail(Trail trail) : base(trail) { }

            public override string ToString() => string.Join("  ", this);
        }

        public IEnumerable<int> GetTrailheadScores()
        {
            foreach (var trailhead in this.Trailheads)
            {
                yield return this.GetTrailheadScore(trailhead);
            }
        }
        
        public IEnumerable<int> GetTrailheadRatings()
        {
            foreach (var start in this.Trailheads)
            {
                yield return this.GetTrailheadRating(start);
            }
        }
        
        private IEnumerable<Point> Trailheads => this.GetMapPoints(Trailhead);

        private int GetTrailheadRating(Point trailhead)
        {
            var trails = this.GetAllTrails(trailhead);
            return trails.Count;
        }

        private int GetTrailheadScore(Point trailhead)
        {
            var trails = this.GetAllTrails(trailhead);
            // count distinct peaks
            return trails.DistinctBy(t => t[^1]).Count();
        }

        // depth-first search (DFS) approach
        private List<Trail> GetAllTrails(Point trailhead)
        {
            var trails = new List<Trail>();
            var currentTrail = new Trail { trailhead };
            this.FindTrails(trailhead, currentTrail, trails);
            return trails;
        }

        private void FindTrails(Point current, Trail currentTrail, List<Trail> trails)
        {
            if (this.GetHeight(current) == Peak)
            {
                trails.Add(new Trail(currentTrail));
                return;
            }

            foreach (var direction in Enum.GetValues<Direction>())
            {
                var next = current.GetNext(direction);
                if (!this.IsOnMap(next) || !this.CanMove(next, this.GetHeight(current))) continue;

                currentTrail.Add(next);
                this.FindTrails(next, currentTrail, trails);
                currentTrail.RemoveAt(currentTrail.Count - 1);
            }
        }

        private int GetHeight(Point point) => this.map[point];

        private bool IsOnMap(Point next) => this.map.ContainsKey(next);
        
        private bool CanMove(Point target, int currentHeight) => this.GetHeight(target) == currentHeight + 1;

        private IEnumerable<Point> GetMapPoints(int height)
        {
            return this.map.Where(e => e.Value == height).Select(e => e.Key);
        }
    }

    private readonly Map map;

    public Day10()
    {
        this.map = new Map(this.GetInputLines());
    }
    
    [ExpectedResult(36, 517)]
    public override object SolvePartOne()
    {
        var scores = this.map.GetTrailheadScores();
        this.DebugOut(string.Join(" ", scores));
        return scores.Sum();
    }

    [ExpectedResult(81, 1116)]
    public override object SolvePartTwo()
    {
        var ratings = this.map.GetTrailheadRatings();
        this.DebugOut(string.Join(" ", ratings));
        return ratings.Sum();
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               89010123
               78121874
               87430965
               96549874
               45678903
               32019012
               01329801
               10456732
               """;
    }
}