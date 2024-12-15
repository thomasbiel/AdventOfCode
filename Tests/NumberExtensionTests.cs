using System;
using System.Collections.Generic;
using AdventOfCode.Utilities;
using NUnit.Framework;

namespace AdventOfCode.Tests;

[TestFixture]
public class NumberExtensionTests
{
    [Test]
    public void Test([Range(1, 64)] int bits)
    {
        ulong n = 1;

        ulong value = 0;
        for (var i = 0; i < bits; i++)
        {
            value += n << i;
        }
        
        TestContext.WriteLine($"{value:B} = {value} -> {value.ToString().Length}");
    }

    public static IEnumerable<ulong> TestValues
    {
        get
        {
            var value = ulong.MaxValue;
            ulong last = 0;
            while (value > 1 && value != last)
            {
                var digits = value.GetDigitCount();
                var expected = value.ToString().Length;
                Assert.That(digits, Is.EqualTo(expected));
                last = value;
                yield return value -= value / 8;
            }
        }
    }
    
    [TestCaseSource(nameof(TestValues))]
    public void Verify_Digits(ulong value)
    {
        var digits = value.GetDigitCount();
        var expected = value.ToString().Length;
        Assert.That(digits, Is.EqualTo(expected));
    }
    
    [TestCase(0, 0, 0)]
    [TestCase(0, 1, 0)]
    [TestCase(1, 0, 0)]
    [TestCase(2, 3, 6)]
    [TestCase(3, 4, 12)]
    [TestCase(4, 6, 12)]
    [TestCase(6, 10, 30)]
    public void Verify_GetLeastCommonMultiple(long a, long b, long expected)
    {
        var lcm = a.GetLeastCommonMultiple(b);
        Assert.That(lcm, Is.EqualTo(expected));
    }

    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    public void Verify_GetLeastCommonMultiple_ArgumentOutOfRangeException(long a, long b)
    {
        Assert.That(() => a.GetLeastCommonMultiple(b), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    
    [TestCase(0, 0, 0)]
    [TestCase(1, 0, 1)]
    [TestCase(0, 1, 1)]
    [TestCase(1, 1, 1)]
    [TestCase(2, 4, 2)]
    [TestCase(6, 3, 3)]
    [TestCase(18, 24, 6)]
    public void Verify_GetGreatestCommonDivisor(long a, long b, long expected)
    {
        var gcd = a.GetGreatestCommonDivisor(b);
        Assert.That(gcd, Is.EqualTo(expected));
    }

    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    public void Verify_GetGreatestCommonDivisor_ArgumentOutOfRangeException(long a, long b)
    {
        Assert.That(() => a.GetGreatestCommonDivisor(b), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}