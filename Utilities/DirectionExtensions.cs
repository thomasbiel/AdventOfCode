namespace AdventOfCode.Utilities;

public static class DirectionExtensions
{
    public static Direction TurnRight(this Direction direction)
    {
        var turn = (int)direction + 1;
        return (Direction)(turn % 4);
    }
    
    public static Direction TurnLeft(this Direction direction)
    {
        var turn = (int)direction + 3;
        return (Direction)(turn % 4);
    }
    
    public static Direction Reverse(this Direction direction) => (Direction)(((int)direction + 2) % 4);
}