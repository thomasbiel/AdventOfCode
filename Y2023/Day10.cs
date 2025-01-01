using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2023;

public class Day10 : Day
{
    private sealed record Pipe(char Name, params Direction[] connectors)
    {
        private bool West => this.Any(Direction.Left);

        private bool East => this.Any(Direction.Right);

        private bool North => this.Any(Direction.Up);

        private bool South => this.Any(Direction.Down);

        public bool ConnectsTo(Pipe pipe, Direction direction)
        {
            return direction switch
            {
                Direction.Left => this.West && pipe.East,
                Direction.Right => this.East && pipe.West,
                Direction.Up => this.North && pipe.South,
                Direction.Down => this.South && pipe.North,
                _ => false
            };
        }
        
        private bool Any(Direction direction) => this.connectors.Any(c => c == direction);
    }

    private sealed record PipeMap(Dictionary<Point, Pipe> Pipes, int MaxRowIndex, int MaxColumnIndex)
        : Area(MaxRowIndex, MaxColumnIndex)
    {
        public Pipe TryGet(Point p) => this.Pipes.GetValueOrDefault(p);
        
        public void SetStartPipe(Point start)
        {
            var prevCol = Math.Max(0, start.Column - 1); 
            var nextCol = Math.Min(start.Column + 1, this.MaxColumnIndex);
            var prevRow = Math.Max(0, start.Row - 1);
            var nextRow = Math.Min(start.Row + 1, this.MaxRowIndex);

            var pipeWest = prevCol < start.Column ? this.TryGet(start with { Column = prevCol }) : null;
            var pipeEast = nextCol > start.Column ? this.TryGet(start with { Column = nextCol }) : null;
            var pipeNorth = prevRow < start.Row ? this.TryGet(start with { Row = prevRow }) : null;
            var pipeSouth = nextRow > start.Row ? this.TryGet(start with { Row = nextRow }) : null;

            var surroundingPipes = new Dictionary<Direction, Pipe>
            {
                { Direction.Left, pipeWest },
                { Direction.Up, pipeNorth },
                { Direction.Right, pipeEast },
                { Direction.Down, pipeSouth }
            }.Where(p => p.Value != null).ToArray();

            foreach (var pipe in PipeLookup.Values)
            {
                if (surroundingPipes.Count(e => pipe.ConnectsTo(e.Value, e.Key)) == 2)
                {
                    this.Pipes[start] = pipe;
                    return;
                }
            }
        
            throw new InvalidOperationException("No fitting pipe found for start.");
        }
    }

    private static readonly IReadOnlyList<Pipe> PipeTypes =
    [
        new('|', Direction.Up, Direction.Down),
        new('-', Direction.Left, Direction.Right),
        new('L', Direction.Up, Direction.Right),
        new('J', Direction.Left, Direction.Up),
        new('7', Direction.Left, Direction.Down),
        new('F', Direction.Down, Direction.Right)
    ];
    
    private static readonly Dictionary<char, Pipe> PipeLookup = PipeTypes.ToDictionary(p => p.Name);
    
    [ExpectedResult(8, 6886)]
    public override object SolvePartOne()
    {
        var (pipeMap, start) = this.CreatePipeMap(1);

        this.PrintPipeMap(pipeMap);

        var visited = WalkThePipeLoop(pipeMap, start);
        return visited.Count / 2;
    }

    [ExpectedResult(8, 371)]
    public override object SolvePartTwo()
    {
        var (pipeMap, start) = this.CreatePipeMap(2);
        var points = WalkThePipeLoop(pipeMap, start).ToList();
        var area = points.CalculateAreaOfPolygonWithShoelaceFormula();
        return points.CalculateNumberOfInnerPointsWithPicksTheorem(area);
    }

    private (PipeMap pipeMap, Point start) CreatePipeMap(int part)
    {
        var row = 0;

        Point start = null;
        var maxColumnIndex = 0;
        var pipes = new Dictionary<Point, Pipe>(); 
        foreach (var line in this.GetInputLines(part))
        {
            if (row == 0) maxColumnIndex = line.Length - 1;

            var column = 0;
            foreach (var p in line)
            {
                if (PipeLookup.TryGetValue(p, out var pipe))
                {
                    pipes.Add(new(column, row), pipe);
                }

                if (p == 'S')
                {
                    start = new(column, row);
                }

                column++;
            }

            row++;
        }
        
        var maxRowIndex = row - 1;
        if (start == null) throw new InvalidOperationException("No start found.");

        var pipeMap = new PipeMap(pipes, maxRowIndex, maxColumnIndex);
        pipeMap.SetStartPipe(start);
        return (pipeMap, start);
    }

    private static HashSet<Point> WalkThePipeLoop(PipeMap pipeMap, Point start)
    {
        var directions = Enum.GetValues<Direction>();
        var current = start;
        var visited = new HashSet<Point>();
        while (visited.Add(current))
        {
            var pipe = pipeMap.TryGet(current);
            var next = directions.FirstOrDefault(d =>
            {
                var next = current.GetNext(d);
                var nextPipe = pipeMap.TryGet(next); 
                return nextPipe != null && pipe.ConnectsTo(nextPipe, d) && !visited.Contains(next);
            });
            
            current = current.GetNext(next);
        }

        return visited;
    }

    private void PrintPipeMap(PipeMap pipeMap)
    {
        this.DebugOut(
            () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Pipe map:");
                pipeMap.Enumerate(
                    p => sb.Append(pipeMap.TryGet(p)?.Name ?? '.'),
                    () => sb.AppendLine());
                
                return sb.ToString();
            });
    }

    protected override string GetTestInput(int? part = null)
    {
        if (part == 2)
        {
            return """
                   .F----7F7F7F7F-7....
                   .|F--7||||||||FJ....
                   .||.FJ||||||||L7....
                   FJL7L7LJLJ||LJ.L-7..
                   L--J.L7...LJS7F-7L7.
                   ....F-J..F7FJ|L7L7L7
                   ....L7.F7||L7|.L7L7|
                   .....|FJLJ|FJ|F7|.LJ
                   ....FJL-7.||.||||...
                   ....L---J.LJ.LJLJ...
                   """;
        }
        
        return """
               ..F7.
               .FJ|.
               SJ.L7
               |F--J
               LJ...
               """;
    }
}