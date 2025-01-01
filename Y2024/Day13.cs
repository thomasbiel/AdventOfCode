using System;
using System.Collections.Generic;

namespace AdventOfCode.Y2024;

public class Day13 : Day
{
    private record Point(long X, long Y)
    {
        public static readonly Point Empty = new(0, 0);
    }
    
    private const string ButtonA = "Button A:";
    private const string ButtonB = "Button B:";
    private const string Prize = "Prize:";
    private const int ButtonCostA = 3;
    private const int ButtonCostB = 1;
    private const long Offset = 10_000_000_000_000L;
    
    private record Machine(Point ButtonA, Point ButtonB, Point Prize);
    
    [ExpectedResult(480L, 36838L)]
    public override object SolvePartOne()
    {
        var machines = this.GetMachines();
        return GetFewestToken(machines);
    }

    [ExpectedResult(null, 83029436920891L)]
    public override object SolvePartTwo()
    {
        var machines = this.GetMachines(p => new Point(p.X + Offset, p.Y + Offset));
        return GetFewestToken(machines);
    }
    
    private static long GetCheapestWayToWinMachine(Machine machine)
    {
        // Cramer's Rule: https://en.wikipedia.org/wiki/Cramer%27s_rule
        var determinant = machine.ButtonA.X * machine.ButtonB.Y - machine.ButtonA.Y * machine.ButtonB.X;
        var a = (machine.Prize.X * machine.ButtonB.Y - machine.Prize.Y * machine.ButtonB.X) / determinant;
        var b = (machine.ButtonA.X * machine.Prize.Y - machine.ButtonA.Y * machine.Prize.X) / determinant;
        if (machine.ButtonA.X * a + machine.ButtonB.X * b == machine.Prize.X &&
            machine.ButtonA.Y * a + machine.ButtonB.Y * b == machine.Prize.Y)
        {
            return a * ButtonCostA + b * ButtonCostB;
        }
        
        return 0L;
    }

    private static long GetFewestToken(List<Machine> machines)
    {
        var total = 0L;
        foreach (var machine in machines)
        {
            total += GetCheapestWayToWinMachine(machine);
        }
        
        return total;
    }

    private List<Machine> GetMachines(Func<Point, Point> fixPrizes = null)
    {
        var fix = fixPrizes ?? (p => p);
        var machines = new List<Machine>();
        var buttonA = Point.Empty;
        var buttonB = Point.Empty;
        var prize = Point.Empty;
        foreach (var line in this.GetInputLines())
        {
            static Point Read(string s)
            {
                var parts = s.Split(',');
                return new Point(int.Parse(parts[0].TrimStart()[2..]), int.Parse(parts[1].TrimStart()[2..]));
            }
            
            if (line.StartsWith(ButtonA)) buttonA = Read(line[ButtonA.Length..]);
            if (line.StartsWith(ButtonB)) buttonB = Read(line[ButtonB.Length..]);
            if (line.StartsWith(Prize)) prize = fix(Read(line[Prize.Length..]));
            
            if (string.IsNullOrWhiteSpace(line))
            {
                machines.Add(new Machine(buttonA, buttonB, prize));
            }
        }
        
        machines.Add(new Machine(buttonA, buttonB, prize));
        return machines;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               Button A: X+94, Y+34
               Button B: X+22, Y+67
               Prize: X=8400, Y=5400

               Button A: X+26, Y+66
               Button B: X+67, Y+21
               Prize: X=12748, Y=12176

               Button A: X+17, Y+86
               Button B: X+84, Y+37
               Prize: X=7870, Y=6450

               Button A: X+69, Y+23
               Button B: X+27, Y+71
               Prize: X=18641, Y=10279
               """;
    }
}