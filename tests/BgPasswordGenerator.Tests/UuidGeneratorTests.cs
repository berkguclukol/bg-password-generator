using System.Text.RegularExpressions;
using BgPasswordGenerator.Core;

namespace BgPasswordGenerator.Tests;

public class UuidGeneratorTests
{
    [Theory]
    [InlineData(false, true, "^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$")]
    [InlineData(true, true, "^[0-9A-F]{8}-[0-9A-F]{4}-4[0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$")]
    [InlineData(false, false, "^[0-9a-f]{12}4[0-9a-f]{3}[89ab][0-9a-f]{15}$")]
    [InlineData(true, false, "^[0-9A-F]{12}4[0-9A-F]{3}[89AB][0-9A-F]{15}$")]
    public void IsVersion4InTheRequestedFormat(bool uppercase, bool hyphens, string pattern)
    {
        for (var run = 0; run < 1000; run++)
        {
            Assert.Matches(new Regex(pattern), UuidGenerator.Generate(uppercase, hyphens));
        }
    }

    [Fact]
    public void ValuesAreUnique()
    {
        var values = Enumerable.Range(0, 10000).Select(_ => UuidGenerator.Generate(false, true)).ToHashSet();
        Assert.Equal(10000, values.Count);
    }
}
