using System;
using System.Numerics;

namespace AdventOfCode.Utilities;

public static class NumericExtensions
{
    private static readonly int[] GuessDigits =
    [
        19, // 64
        19, // 63
        19, // 62
        19, // 61
        18, // 60
        18, // 59
        18, // 58
        17, // 57
        17, // 56
        17, // 55
        16, // 54
        16, // 53
        16, // 52
        16, // 51
        15, // 50
        15, // 49
        15, // 48
        14, // 47
        14, // 46
        14, // 45
        13, // 44
        13, // 43
        13, // 42
        13, // 41
        12, // 40
        12, // 39
        12, // 38
        11, // 37
        11, // 36
        11, // 35
        10, // 34
        10, // 33
        10, // 32
        10, // 31
        9, // 30
        9, // 29
        9, // 28
        8, // 27
        8, // 26
        8, // 25
        7, // 24
        7, // 23
        7, // 22
        7, // 21
        6, // 20
        6, // 19
        6, // 18
        5, // 17
        5, // 16
        5, // 15
        4, // 14
        4, // 13
        4, // 12
        4, // 11
        3, // 10
        3, // 9
        3, // 8
        2, // 7
        2, // 6
        2, // 5
        1, // 4
        1, // 3
        1, // 2
        1  // 1
    ];

    private static readonly ulong[] PowersOf10 =
    [
        1,
        10,
        100,
        1_000,
        10_000,
        100_000,
        1_000_000,
        10_000_000,
        100_000_000,
        1_000_000_000,
        10_000_000_000,
        100_000_000_000,
        1_000_000_000_000,
        10_000_000_000_000,
        100_000_000_000_000,
        1_000_000_000_000_000,
        10_000_000_000_000_000,
        100_000_000_000_000_000,
        1_000_000_000_000_000_000,
        10_000_000_000_000_000_000
    ];
    
    public static int GetDigitCount(this ulong number)
    {
        if (number == 0) return 1;
        
        var leadingZeros = BitOperations.LeadingZeroCount(number);
        var digits = GuessDigits[leadingZeros];
        if (number >= PowersOf10[digits])
        {
            digits++;
        }

        return digits;
    }
    
    public static long GetLeastCommonMultiple(this long a, long b)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(a);
        ArgumentOutOfRangeException.ThrowIfNegative(b);
        
        if (a == 0 || b == 0) return 0;
        return a * b / a.GetGreatestCommonDivisor(b);
    }

    public static long GetGreatestCommonDivisor(this long a, long b)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(a);
        ArgumentOutOfRangeException.ThrowIfNegative(b);
        
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        
        return a;
    }
}