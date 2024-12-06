using System;
using System.Collections.Generic;

namespace AdventOfCode.Y2024;

[Year(2024)]
public class Day6 : Day
{
    private enum Action
    {
        Move,
        Exit
    }
    
    private sealed record Point(int Column, int Row);
    private sealed record Lab(int MaxRowIndex, int MaxColumnIndex, HashSet<Point> Obstructions);
    private sealed record Move(Point Next, Direction Direction, Action Action = Action.Move);
    
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    
    private class Guard
    {
        private static readonly Move LeftTheLab = new(null, Direction.Up, Action.Exit);
        
        private readonly HashSet<Point> positions = new();
        private readonly Lab lab;
        
        private Point position;
        private Direction direction;
        
        public Guard(Lab lab, Point start)
        {
            this.lab = lab ?? throw new ArgumentNullException(nameof(lab));
            this.MoveTo(start ?? throw new ArgumentNullException(nameof(start)));
        }

        public bool TryMove()
        {
            var move = this.direction switch
            {
                Direction.Up => MoveUpOrTurnRight(),
                Direction.Right => MoveRightOrTurnDown(),
                Direction.Down => MoveDownOrTurnLeft(),
                Direction.Left => MoveLeftOrTurnUp(),
                _ => LeftTheLab
            };

            if (move.Action == Action.Exit) return false;

            if (!this.lab.Obstructions.Contains(move.Next))
            {
                this.MoveTo(move.Next);
                return true;
            }
                
            this.direction = move.Direction;
            return true;
        }
        
        public int DistinctPositionsVisited => this.positions.Count;

        public override string ToString() => $"Guard at {this.position} facing {this.direction}";

        private Move MoveLeftOrTurnUp()
        {
            if (this.Column == 0) return LeftTheLab;
            var next = new Point(this.Column - 1, this.Row);
            return new Move(next, Direction.Up);
        }

        private Move MoveDownOrTurnLeft()
        {
            if (this.Row == this.lab.MaxRowIndex) return LeftTheLab;
            var next = new Point(this.Column, this.Row + 1);
            return new Move(next, Direction.Left);
        }

        private Move MoveRightOrTurnDown()
        {
            if (this.Column == this.lab.MaxColumnIndex) return LeftTheLab;
            var next = new Point(this.Column + 1, this.Row);
            return new Move(next, Direction.Down);
        }
        
        private Move MoveUpOrTurnRight()
        {
            if (this.Row == 0) return LeftTheLab;
            var next = new Point(this.Column, this.Row - 1);
            return new Move(next, Direction.Right);
        }

        private int Column => this.position.Column;
        
        private int Row => this.position.Row;

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
    
    public override object SolvePartOne()
    {
        var guard = new Guard(this.lab, this.start);
        while (guard.TryMove())
        {
            this.DebugOut(guard.ToString());
        }

        return guard.DistinctPositionsVisited;
    }

    public override object SolvePartTwo()
    {
        var safetyThreshold = (this.lab.MaxColumnIndex + this.lab.MaxRowIndex) * 4;

        var count = 0;
        for (var c = 0; c <= this.lab.MaxColumnIndex; c++)
        {
            for (var r = 0; r <= this.lab.MaxColumnIndex; r++)
            {
                var potentialObstruction = new Point(c, r);
                if (potentialObstruction == this.start)
                {
                    continue;
                }
                  
                if (this.lab.Obstructions.Add(potentialObstruction))
                {
                    if (HasLoop())
                    {
                        this.DebugOut($"Potential obstruction at {potentialObstruction} causes a loop.");
                        count++;
                    }
                    
                    this.lab.Obstructions.Remove(potentialObstruction);
                }
            }
        }

        return count;

        bool HasLoop()
        {
            var guard = new Guard(this.lab, this.start);
            var steps = 0;
            while (guard.TryMove())
            {
                steps++;
                if (steps > guard.DistinctPositionsVisited + safetyThreshold)
                {
                    return true;
                }
            }

            return false;
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