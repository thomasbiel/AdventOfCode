using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

public class Day14 : Day
{
    private record Robot(Point Position, Point Velocity);

    private Point floorSize;
    private IReadOnlyCollection<Robot> robots;

    [ExpectedResult(12, 226236192)]
    public override object SolvePartOne()
    {
        this.floorSize = ExecutionContext.Mode == ExecutionMode.Debug
            ? new Point(11, 7)
            : new Point(101, 103);
        
        this.robots = this.GetRobots(1).ToArray();
        return this.CalculateSafetyFactor(seconds: 100);
    }

    [ExpectedResult(8168, 8168)]
    public override object SolvePartTwo()
    {
        this.floorSize = new Point(101, 103);
        this.robots = this.GetRobots(2).ToArray();
        
        var max = 10_000;
        var deviations = new double[max];
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        Parallel.For(
            0,
            max,
            options,
            i =>
            {
                var positions = this.MoveRobots(i);
                deviations[i] = CalculateStandardDeviation(positions);
            });
        
        if (ExecutionContext.Mode == ExecutionMode.Debug)
        {
            SaveDeviationDiagram(deviations, "deviations.html");
        }
        
        // get index of minimum deviation 
        var (_, index) = deviations.Select((d, i) => (d, i)).MinBy(e => e.d);
        this.PrintIteration(index);
        return index;
    }

    private void PrintIteration(int index) => this.DebugOut(() =>
    {
        var positions = this.MoveRobots(index);

        var sb = new StringBuilder();
        var rows = positions
            .GroupBy(p => p.Row)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Column).ToList());
            
        var empty = new List<int>();
        for (var i = 0; i < this.floorSize.Row; i++)
        {
            var points = rows.GetValueOrDefault(i, empty);
            var chars = Enumerable.Range(0, this.floorSize.Column).Select(v => points.Contains(v) ? 'X' : '.');
            var row = new string(chars.ToArray());
            sb.AppendLine(row);
        }

        return sb.ToString();
    });

    private static void SaveDeviationDiagram(ICollection<double> deviations, string fileName)
    {
        var min = deviations.Min(Math.Floor);
        var max = deviations.Max(Math.Ceiling);
        var separator = $",{Environment.NewLine}    ";
        var data = string.Join(separator, deviations.Select((d, i) => string.Format(CultureInfo.InvariantCulture, "{{x:{0}, y:{1:F2}}}", i, d)));
        var html = $$$"""
                      <!DOCTYPE html>
                      <html>
                      <body>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.4/Chart.js"></script>
                        <canvas id="chart" style="width:100%"></canvas>
                         
                        <script>
                        const xyValues = [
                          {{{data}}}
                        ];

                        new Chart("chart", {
                          type: "scatter",
                          data: {
                            datasets: [{
                              pointRadius: 3,
                              pointBackgroundColor: "black",
                              data: xyValues
                            }]
                          },
                          options: {
                            legend: {display: false},
                            title: {
                              display: true,
                              text: "Deviation per iteration"
                            },
                            scales: {
                              xAxes: [{ticks: {min: 0, max:{{{deviations.Count}}}}}],
                              yAxes: [{ticks: {min: {{{min}}}, max:{{{max}}}}}],
                            }
                          }
                        });
                        </script>
                      </body>
                      </html>
                      """;
       
        File.WriteAllText(fileName, html);
    }
    
    private List<Point> MoveRobots(int seconds)
    {
        var result = new List<Point>();
        foreach (var robot in this.robots)
        {
            var row = (robot.Position.Row + robot.Velocity.Row * seconds) % this.floorSize.Row;
            if (row < 0) row += this.floorSize.Row;

            var column = (robot.Position.Column + robot.Velocity.Column * seconds) % this.floorSize.Column;
            if (column < 0) column += this.floorSize.Column;

            result.Add(new Point(column, row));
        }

        return result;
    }
    
    private static double CalculateStandardDeviation(List<Point> positions)
    {
        var mean = CalculateMeanPosition(positions);
        var diff = positions.Sum(p => Math.Pow(p.GetDistance(mean), 2));
        return Math.Sqrt(diff / positions.Count);
    } 
    
    private static Point CalculateMeanPosition(List<Point> positions)
    {
        var meanColumn = positions.Sum(p => p.Column) / positions.Count;
        var meanRow = positions.Sum(p => p.Row) / positions.Count;
        return new Point(meanColumn, meanRow);
    }
    
    private int CalculateSafetyFactor(int seconds)
    {
        var quadrants = new int[2, 2];
        var middleColumn = this.floorSize.Column / 2;
        var middleRow = this.floorSize.Row / 2;
        
        var positions = this.MoveRobots(seconds);
        foreach (var (column, row) in positions)
        {
            if (row == middleRow || column == middleColumn)
            {
                continue;
            }
            
            quadrants[row < middleRow ? 0 : 1, column < middleColumn ? 0 : 1]++;
        }
        
        return quadrants[0, 0] * quadrants[0, 1] * quadrants[1, 0] * quadrants[1, 1];
    }

    private IEnumerable<Robot> GetRobots(int part)
    {
        foreach (var line in this.GetInputLines(part))
        {
            var parts = line.Split(" ");
            var p = Parse(parts[0]);
            var v = Parse(parts[1]);
            yield return new Robot(p, v);
        }

        static Point Parse(string s)
        {
            var parts = s[2..].Split(","); 
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
    
    protected override string GetTestInput(int? part = null)
    {
        return part == 2 // no debug input for part 2
            ? ExecutionContext.LoadInput(this, File.ReadAllText).TrimEnd()
            : """
              p=0,4 v=3,-3
              p=6,3 v=-1,-3
              p=10,3 v=-1,2
              p=2,0 v=2,-1
              p=0,0 v=1,3
              p=3,0 v=-2,-2
              p=7,6 v=-1,-3
              p=3,0 v=-1,-2
              p=9,3 v=2,3
              p=7,3 v=-1,2
              p=2,4 v=2,-3
              p=9,5 v=-3,-3
              """;
    }
}