using System;
using System.Collections.Generic;

namespace AdventOfCode.Utilities;

public static class GeometryExtensions
{
    // https://en.wikipedia.org/wiki/Pick%27s_theorem
    public static int CalculateNumberOfInnerPointsWithPicksTheorem(this IReadOnlyCollection<Point> points, double area)
    {
        var b = points.Count;
        var p = area - b / 2.0 + 1;
        return (int) p;
    }

    // https://en.wikipedia.org/wiki/Shoelace_formula
    public static double CalculateAreaOfPolygonWithShoelaceFormula(this IReadOnlyList<Point> points)
    {
        var n = points.Count;
        
        var area = 0.0;
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            area += points[i].Column * points[j].Row - 
                    points[j].Column * points[i].Row;
        }

        return Math.Abs(area) / 2.0;
    }
}