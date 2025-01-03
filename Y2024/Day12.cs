using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

public class Day12 : Day
{
    private class Region : IEquatable<Region>
    {
        private record Fence(List<Point> Points, Direction Direction);
        
        private readonly HashSet<Point> points = new();
        
        public Region(char id)
        {
            this.Id = id;
        }
        
        public char Id { get; }

        public int CalculatePrice()
        {
            var fences = this.GetFences();
            var perimeter = fences.Sum(f => f.Points.Count);
            return this.Area * perimeter;
        }
        
        public int CalculateDiscountPrice()
        {
            var fences = this.GetFences();
            
            Comparison<Point> compareRows = (p1, p2) => p1.Row == p2.Row ? p1.Column - p2.Column : p1.Row - p2.Row;
            Comparison<Point> compareColumns = (p1, p2) => p1.Column == p2.Column ? p1.Row - p2.Row : p1.Column - p2.Column;

            var sides = 0;

            void CountVerticalSides(Point current, Point next)
            {
                var isContinuationOfSide = current.Row == next.Row - 1 && current.Column == next.Column;
                sides += isContinuationOfSide ? 0 : 1;
            }
            
            void CountHorizontalSides(Point current, Point next)
            {
                var isContinuationOfSide = current.Column == next.Column - 1 && current.Row == next.Row;
                sides += isContinuationOfSide ? 0 : 1;
            }
            
            foreach (var fence in fences)
            {
                fence.Points.Sort(fence.Direction is Direction.Up or Direction.Down ? compareRows : compareColumns);
                
                var count = fence.Points.Count;
                for (var i = 0; i < count; i++)
                {
                    var current = fence.Points[i]; 
                    var next = fence.Points[(i + 1) % count];
                    
                    Action<Point, Point> countSides = fence.Direction is Direction.Up or Direction.Down
                        ? CountHorizontalSides
                        : CountVerticalSides;
                    
                    countSides(current, next);
                }
            }

            return this.Area * sides;
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

        private IEnumerable<Point> GetAdjacentPoints(Point point)
        {
            return Point.Directions.Select(point.GetNext).Where(p => !this.Contains(p));
        }

        private int Area => this.points.Count;

        private Fence GetFence(List<Point> borderPoints, Direction d)
        {
            return new Fence(borderPoints.Where(p => !this.Contains(p.GetNext(d))).ToList(), d);
        }

        private List<Fence> GetFences()
        {
            var borderPoints = this.points.Where(p => this.GetAdjacentPoints(p).Any()).ToList();
            return Point.Directions.Select(d => this.GetFence(borderPoints, d)).ToList();
        }
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
            var surroundingPoints = Point.Directions
                .Select(point.GetNext)
                .Where(p => this.Contains(p) && this[p.Row, p.Column] == region.Id)
                .Where(visited.Add);
    
            foreach (var p in surroundingPoints)
            {
                region.Add(p);
                this.AddSurroundingPoints(visited, p, region);
            }
        }

        private bool Contains(Point point) => point.Row >= 0 && point.Column >= 0 && 
                                              point.Row <= this.MaxRow && point.Column <= this.MaxColumn;
    }

    private readonly Garden garden;

    public Day12()
    {
        this.garden = new Garden(this.GetInputLines().ToArray());
    }
    
    [ExpectedResult(1930, 1421958)]
    public override object SolvePartOne() => this.garden.Regions.Sum(r => r.CalculatePrice());

    [ExpectedResult(1206, 885394)]
    public override object SolvePartTwo() => this.garden.Regions.Sum(r => r.CalculateDiscountPrice());

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