using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

public class Day12 : Day
{
    private class Region : IEquatable<Region>
    {
        private readonly HashSet<Point> points = new();
        
        public Region(char id)
        {
            this.Id = id;
        }
        
        public char Id { get; }

        public int Price
        {
            get
            {
                var minColumn = this.points.Min(p => p.Column);
                var maxColumn = this.points.Max(p => p.Column);
                var minRow = this.points.Min(p => p.Row);
                var maxRow = this.points.Max(p => p.Row);

                var perimeter = 0;
                for (var row = minRow; row <= maxRow; row++)
                {
                    for (var column = minColumn; column <= maxColumn; column++)
                    {
                        var point = new Point(column, row);
                        if (!this.Contains(point)) continue;

                        var surroundingPoints = Enum.GetValues<Direction>()
                            .Select(point.GetNext)
                            .Where(p => !this.Contains(p));

                        perimeter += surroundingPoints.Count();
                    }
                }

                var area = this.points.Count;
                return area * perimeter;
            }
        }

        public bool Contains(Point point) => this.points.Contains(point);

        public void Add(Point point) => this.points.Add(point);

        public override string ToString() => $"{this.Id}: {this.points.Count}";

        public bool Equals(Region other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Id == other.Id && this.points.SetEquals(other.points);
        }

        public override bool Equals(object obj) => obj is Region r && this.Equals(r);

        public override int GetHashCode() => this.Id.GetHashCode();
    }
    
    private class Garden
    {
        private readonly string[] lines;

        public Garden(string[] lines)
        {
            this.lines = lines;
            this.MaxColumn = lines[0].Length - 1;
            this.MaxRow = lines.Length - 1;
            
            var regions = new List<Region>();

            var visited = new HashSet<Point>();
            for (var row = 0; row <= this.MaxRow; row++)
            {
                for (var column = 0; column <= this.MaxColumn; column++)
                {
                    var id = this[row, column];
                    var point = new Point(column, row);
                    var region = regions.FirstOrDefault(r => r.Contains(point));
                    if (region != null) continue;

                    region = new Region(id);
                    regions.Add(region);
                    region.Add(point);

                    this.AddSurroundingPoints(visited, point, region);
                }
            }

            this.Regions = regions;
        }

        public int MaxRow { get; }

        public int MaxColumn { get; }

        public char this[int row, int column] => this.lines[row][column];
        
        public IReadOnlyList<Region> Regions { get; }
        
        private void AddSurroundingPoints(HashSet<Point> visited, Point point, Region region)
        {
            var surroundingPoints = Enum.GetValues<Direction>()
                .Select(point.GetNext)
                .Where(p => this.Contains(p) && this[p.Row, p.Column] == region.Id)
                .Where(visited.Add);
    
            foreach (var p in surroundingPoints)
            {
                region.Add(p);
                this.AddSurroundingPoints(visited, p, region);
            }
        }
        
        public bool Contains(Point point) => point.Row >= 0 && point.Column >= 0 && 
                                             point.Row <= this.MaxRow && point.Column <= this.MaxColumn;
    }
    
    [ExpectedResult(1930, 1421958)]
    public override object SolvePartOne()
    {
        var garden = new Garden(this.GetInputLines().ToArray());
        return garden.Regions.Sum(r => r.Price);
    }

    [ExpectedResult(1206)]
    public override object SolvePartTwo()
    {
        return 0;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               RRRRIICCFF
               RRRRIICCCF
               VVRRRCCFFF
               VVRCCCJFFF
               VVVVCJJCFE
               VVIVCCJJEE
               VVIIICJJEE
               MIIIIIJJEE
               MIIISIJEEE
               MMMISSJEEE
               """;
    }
}