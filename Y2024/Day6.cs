using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day6 : Day<int>
{
    private sealed record Lab(int MaxRowIndex, int MaxColumnIndex, IReadOnlySet<Point> Obstructions)
    {
        public bool Contains(Point p)
        {
            return p.Column >= 0 && p.Column <= this.MaxColumnIndex && p.Row >= 0 && p.Row <= this.MaxRowIndex;
        }
    }
    
    private sealed record GuardTurn(Point Position, Direction Direction);
    
    private class Guard
    {
        private readonly HashSet<Point> positions = new();
        private readonly HashSet<GuardTurn> turns = new();
        private readonly Lab lab;
        
        private Point position;
        private Direction direction;
        
        public Guard(Lab lab, Point start)
        {
            this.lab = lab ?? throw new ArgumentNullException(nameof(lab));
            this.MoveTo(start ?? throw new ArgumentNullException(nameof(start)));
        }

        public bool? TryMove(Point obstruction = null)
        {
            var next = this.position.GetNext(this.direction);
            
            if (!this.lab.Contains(next))
            {
                return false;
            }
            
            if (!this.lab.Obstructions.Contains(next) && next != obstruction)
            {
                this.MoveTo(next);
                return true;
            }
            
            // turn right
            var turn = (int)this.direction + 1;
            this.direction = (Direction)(turn % 4);
            if (!this.turns.Add(new(this.position, this.direction)))
            {
                return null; // loop detected
            }
            
            return true;
        }
        
        public int DistinctPositionsVisited => this.positions.Count;

        public override string ToString() => $"Guard at {this.position} facing {this.direction}";

        private void MoveTo(Point p)
        {
            this.position = p;
            this.positions.Add(p);
        }
    }
    
    private readonly Point start;
    private readonly Lab lab;

    public Day6()
    {
        var lines = this.GetInputLines();
        
        var rows = lines.Length;
        var columns = lines[0].Length;
        var obstructions = new HashSet<Point>();
        
        for (var i = 0; i < lines.Length; i++)
        {
            var row = lines[i];
            for (var j = 0; j < row.Length; j++)
            {
                var position = row[j];
                if (position == '#')
                {
                    obstructions.Add(new Point(j, i));
                }
                else if (position == '^')
                {
                    this.start = new Point(j, i);
                }
            }
        }
        
        this.lab = new Lab(rows - 1, columns - 1, obstructions);
    }
    
    public override int SolvePartOne()
    {
        var guard = new Guard(this.lab, this.start);
        while (guard.TryMove() == true)
        {
            this.DebugOut(guard.ToString());
        }

        return guard.DistinctPositionsVisited;
    }

    public override int SolvePartTwo()
    {
        var count = 0;
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };
        Parallel.For(0, this.lab.MaxColumnIndex + 1, options, c =>
        {
            for (var r = 0; r <= this.lab.MaxRowIndex; r++)
            {
                var potentialObstruction = new Point(c, r);
                if (potentialObstruction == this.start)
                {
                    continue;
                }

                if (HasLoop(potentialObstruction))
                {
                    this.DebugOut($"Potential obstruction at {potentialObstruction} causes a loop.");
                    Interlocked.Increment(ref count);
                }
            }
        });

        return count;

        bool HasLoop(Point obstruction)
        {
            var guard = new Guard(this.lab, this.start);
            var canMove = guard.TryMove(obstruction);
            while (canMove == true)
            {
                canMove = guard.TryMove(obstruction);
            }

            return canMove == null;
        }
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               ....#.....
               .........#
               ..........
               ..#.......
               .......#..
               ..........
               .#..^.....
               ........#.
               #.........
               ......#...
               """;
    }
}