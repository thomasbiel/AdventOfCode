using System;

namespace AdventOfCode.Utilities;

public record Area(int MaxRowIndex, int MaxColumnIndex)
{
    public bool Contains(Point p)
    {
        return p.Column >= 0 && p.Column <= this.MaxColumnIndex && p.Row >= 0 && p.Row <= this.MaxRowIndex;
    }

    public void Enumerate(Action<Point> action, Action beforeNextRow = null)
    {
        for (var row = 0; row <= this.MaxRowIndex; row++)
        {
            for (var column = 0; column <= this.MaxColumnIndex; column++)
            {
                action(new Point(column, row));
            }
            
            beforeNextRow?.Invoke();
        }
    }
}