using System;

namespace AdventOfCode.Utilities;

public sealed record Point(int Column, int Row)
{
    public Point GetNext(Direction direction)
    {
        return direction switch
        {
            Direction.Up => this with { Row = this.Row - 1 },
            Direction.Right => this with { Column = this.Column + 1 },
            Direction.Down => this with { Row = this.Row + 1 },
            Direction.Left => this with { Column = this.Column - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Direction[] Directions => Enum.GetValues<Direction>();
    
    public override string ToString() => $"{this.Column},{this.Row}";
}