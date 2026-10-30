using BgPasswordGenerator.Core;

namespace BgPasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    private static readonly string[] Sets =
    [
        PasswordGenerator.Lowercase,
        PasswordGenerator.Uppercase,
        PasswordGenerator.Numbers,
        PasswordGenerator.Symbols,
    ];

    public static TheoryData<bool, bool, bool, bool> AllTypeCombinations()
    {
        var data = new TheoryData<bool, bool, bool, bool>();
        for (var mask = 1; mask < 16; mask++)
        {
            data.Add((mask & 1) != 0, (mask & 2) != 0, (mask & 4) != 0, (mask & 8) != 0);
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(AllTypeCombinations))]
    public void ContainsEverySelectedTypeAndNoOthers(bool lower, bool upper, bool numbers, bool symbols)
    {
        bool[] selected = [lower, upper, numbers, symbols];
        for (var length = PasswordGenerator.MinLength; length <= PasswordGenerator.MaxLength; length++)
        {
            for (var run = 0; run < 50; run++)
            {
                var password = PasswordGenerator.Generate(length, lower, upper, numbers, symbols);

                Assert.Equal(length, password.Length);
                for (var i = 0; i < Sets.Length; i++)
                {
                    Assert.Equal(selected[i], password.Any(Sets[i].Contains));
                }
            }
        }
    }

    [Fact]
    public void ReturnsEmptyWhenNoTypeSelected()
    {
        Assert.Equal(string.Empty, PasswordGenerator.Generate(18, false, false, false, false));
    }

    [Fact]
    public void FirstCharacterTypeIsNotFixed()
    {
        // Guards against a fixed type order (e.g. always starting with a lowercase letter)
        var firstTypes = new HashSet<int>();
        for (var run = 0; run < 500; run++)
        {
            var first = PasswordGenerator.Generate(18, true, true, true, true)[0];
            firstTypes.Add(Array.FindIndex(Sets, s => s.Contains(first)));
        }
        Assert.Equal(4, firstTypes.Count);
    }

    [Fact]
    public void CharactersAreRoughlyUniformAcrossThePool()
    {
        var pool = string.Concat(Sets);
        var counts = new Dictionary<char, int>();
        const int samples = 4000;
        for (var run = 0; run < samples; run++)
        {
            foreach (var c in PasswordGenerator.Generate(24, true, true, true, true))
            {
                counts[c] = counts.GetValueOrDefault(c) + 1;
            }
        }

        Assert.Equal(pool.Length, counts.Count);
        var expected = samples * 24.0 / pool.Length;
        // The guaranteed-one-per-type step skews small sets slightly; allow a generous band
        Assert.All(counts.Values, n => Assert.InRange(n, expected * 0.7, expected * 1.6));
    }
}
