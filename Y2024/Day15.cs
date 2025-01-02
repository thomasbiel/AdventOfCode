using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode.Y2024;

public class Day15 : Day
{
    private const char Wall = '#';
    private const char Robot = '@';
    private const char Space = '.';

    private record Warehouse(int Columns, int Rows, HashSet<Point> Walls)
    {
        public bool IsWall(Point point) => this.Walls.Contains(point);
    }

    private record Box(Point Location, char Type)
    {
        public const char Single = 'O';
        public const char LeftHalf = '[';
        private const char RightHalf = ']';

        public bool Contains(Point p)
        {
            return this.Type switch
            {
                LeftHalf => p == this.Location || p == this.Location.GetNext(Direction.Right),
                _ => p == this.Location
            };
        }
        
        public char this[Point p]
        {
            get
            {
                if (this.Type == Single) return Single; 
                return this.Location == p ? this.Type : RightHalf;
            }
        }

        public IEnumerable<Point> Locations
        {
            get
            {
                yield return this.Location;
                if (this.Type != Single)
                {
                    yield return this.Location.GetNext(Direction.Right);
                }
            }
        }
    }
    
    private readonly HashSet<Box> boxes = [];
    private Warehouse warehouse;
    private List<Direction> movements = [];
    private Point robot;

    [ExpectedResult(2028, 1456590)]
    public override object SolvePartOne()
    {
        this.ParseInput(1);
        this.PrintWarehouseLayout();
        
        this.ExecuteMovements();
        this.PrintWarehouseLayout();
        
        return this.SumGpsCoordinates();
    }

    [ExpectedResult(618, 1489116)]
    public override object SolvePartTwo()
    {
        this.Reset();
        this.ParseInput(2);
        this.EnlargeWarehouse();
        this.EnlargeBoxes();
        this.RepositionRobot();
        
        this.PrintWarehouseLayout();
        this.ExecuteMovements();
        this.PrintWarehouseLayout();
        
        return this.SumGpsCoordinates();
    }

    private void ExecuteMovements()
    {
        var movableBoxes = new HashSet<Box>();
        foreach (var direction in this.movements)
        {
            this.DebugOut($"Move {direction}");
            if (this.FindSpace(movableBoxes, direction))
            {
                this.MoveBoxes(movableBoxes, direction);
                this.robot = this.robot.GetNext(direction);
            }
            
            this.PrintWarehouseLayout();
        }
    }

    private bool FindSpace(HashSet<Box> movableBoxes, Direction direction)
    {
        var next = this.robot.GetNext(direction);
        
        movableBoxes.Clear();
        var pointsToCheck = new[] { next };
        
        var visited = new HashSet<Point>();
        while (pointsToCheck.All(p => !this.warehouse.IsWall(p)))
        {
            var boxesInTheWay = pointsToCheck.Select(this.TryGetBox).Where(b => b != null).ToArray();
            if (boxesInTheWay.Any())
            {
                foreach (var box in boxesInTheWay)
                {
                    movableBoxes.Add(box);                  
                }
            }
            
            if (pointsToCheck.All(p => boxesInTheWay.SelectMany(b => b.Locations).All(l => l != p)))
            {
                // found a space
                return true;
            }
            
            pointsToCheck = boxesInTheWay.SelectMany(b => b.Locations)
                .Select(p => p.GetNext(direction))
                .Append(next.GetNext(direction))
                .Where(p => visited.Add(p))
                .ToArray();
        }

        return false;
    }

    private void MoveBoxes(HashSet<Box> movableBoxes, Direction direction)
    {
        foreach (var box in movableBoxes)
        {
            this.boxes.Remove(box);
        }

        // do not combine with previous loop, since it will create duplicates that will be discarded
        foreach (var box in movableBoxes)
        {
            this.boxes.Add(box with { Location = box.Location.GetNext(direction) });
        }
    }

    private void ParseInput(int part)
    {
        var columns = 0;
        var row = 0;
        var walls = new HashSet<Point>();
        foreach (var line in this.GetInputLines(part))
        {
            if (columns == 0) columns = line.Length;
            
            if (string.IsNullOrWhiteSpace(line))
            {
                this.warehouse = new(columns, row, walls);
                continue;
            }
            
            if (this.warehouse is null)
            {
                var length = line.Length;
                for (var column = 0; column < length; column++)
                {
                    var c = line[column];
                    if (c == Space) continue;

                    var point = new Point(column, row);
                    switch (c)
                    {
                        case Wall:
                            walls.Add(point);
                            break;
                        case Box.LeftHalf:
                        //case Box.RightHalf: ignore, will be processed with left half
                        case Box.Single:
                            this.boxes.Add(new(point, c));
                            break;
                        case Robot:
                            this.robot = point;
                            break;
                    }
                }
                
                row++;
            }
            else
            {
                this.movements = this.movements.Concat(line.Select(ConvertToDirection)).ToList();
            }
        }
    }

    private static Direction ConvertToDirection(char c)
    {
        return c switch
        {
            '<' => Direction.Left,
            '>' => Direction.Right,
            '^' => Direction.Up,
            'v' => Direction.Down,
            _ => throw new InvalidOperationException($"Unknown direction {c}")
        };
    }

    private void RepositionRobot()
    {
        this.robot = this.robot with { Column = this.robot.Column * 2 };
    }

    private void EnlargeBoxes()
    {
        var existing = this.boxes.Select(b => b.Location).ToArray();
        this.boxes.Clear();
        
        foreach (var point in existing)
        {
            this.boxes.Add(new(point with { Column = point.Column * 2 }, Box.LeftHalf));
        }
    }

    private void Reset()
    {
        this.warehouse = null;
        this.movements.Clear();
        this.boxes.Clear();
    }

    private void EnlargeWarehouse()
    {
        var walls = new HashSet<Point>();
        foreach (var wall in this.warehouse.Walls)
        {
            var column = wall.Column * 2;
            walls.Add(wall with { Column = column });
            walls.Add(wall with { Column = column + 1 });
        }
        
        this.warehouse = new Warehouse(this.warehouse.Columns * 2, this.warehouse.Rows, walls);
    }

    private void PrintWarehouseLayout() => this.DebugOut(() =>
    {
        var sb = new StringBuilder();
        for (var r = 0; r < this.warehouse.Rows; r++)
        {
            var row = r;
            var chars = Enumerable.Range(0, this.warehouse.Columns).Select(c =>
            {
                var p = new Point(c, row);
                if (this.warehouse.Walls.Contains(p)) return Wall;
                var box = this.TryGetBox(p);
                if (box != null) return box[p];
                return p == this.robot ? Robot : Space;
            });
                
            sb.AppendLine(new string(chars.ToArray()));
        }

        return sb.ToString();
    });

    private Box TryGetBox(Point p) => this.boxes.FirstOrDefault(b => b.Contains(p));

    private int SumGpsCoordinates() => this.boxes.Select(b => b.Location).Sum(CalculateGpsCoordinate);
    
    private static int CalculateGpsCoordinate(Point b) => 100 * b.Row + b.Column;

    protected override string GetTestInput(int? part = null)
    {
        if (part == 1)
        {
            return """
                   ########
                   #..O.O.#
                   ##@.O..#
                   #...O..#
                   #.#.O..#
                   #...O..#
                   #......#
                   ########

                   <^^>>>vv<v>>v<<
                   """;
        }
        
        return """
               #######
               #...#.#
               #.....#
               #..OO@#
               #..O..#
               #.....#
               #######
               
               <vv<<^^<<^^
               """;
    }
}