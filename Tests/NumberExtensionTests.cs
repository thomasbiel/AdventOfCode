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
}