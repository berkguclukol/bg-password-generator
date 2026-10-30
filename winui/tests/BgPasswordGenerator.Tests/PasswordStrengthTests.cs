using BgPasswordGenerator.Core;

namespace BgPasswordGenerator.Tests;

public class PasswordStrengthTests
{
    [Theory]
    [InlineData("aB3!efgh", Strength.Strong)]
    [InlineData("aB3!efg", Strength.Moderate)]
    [InlineData("abc!ef", Strength.Moderate)]
    [InlineData("abc!e", Strength.Weak)]
    [InlineData("abcDEF123456", Strength.Weak)]
    [InlineData("12345!!!!", Strength.Weak)]
    [InlineData("", Strength.Weak)]
    public void MatchesElectronRules(string password, Strength expected)
    {
        Assert.Equal(expected, PasswordStrength.Evaluate(password));
    }
}
