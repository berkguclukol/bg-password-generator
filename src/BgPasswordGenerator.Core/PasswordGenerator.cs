using System.Security.Cryptography;

namespace BgPasswordGenerator.Core;

public static class PasswordGenerator
{
    public const int MinLength = 4;
    public const int MaxLength = 24;

    public const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Numbers = "0123456789";
    public const string Symbols = "!@#$%^&*(){}[]=<>/,.";

    /// <summary>
    /// Builds a password from a CSPRNG. Every selected character type appears at least once
    /// (when the length allows it) and the result is shuffled so types don't sit at fixed positions.
    /// Returns an empty string when no type is selected.
    /// </summary>
    public static string Generate(int length, bool lower, bool upper, bool numbers, bool symbols)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var selectedSets = new List<string>(4);
        if (lower) selectedSets.Add(Lowercase);
        if (upper) selectedSets.Add(Uppercase);
        if (numbers) selectedSets.Add(Numbers);
        if (symbols) selectedSets.Add(Symbols);
        if (selectedSets.Count == 0)
        {
            return string.Empty;
        }

        var pool = string.Concat(selectedSets);
        var chars = new List<char>(Math.Max(length, selectedSets.Count));
        // Guarantee at least one character from every selected type
        foreach (var set in selectedSets)
        {
            chars.Add(set[RandomNumberGenerator.GetInt32(set.Length)]);
        }
        while (chars.Count < length)
        {
            chars.Add(pool[RandomNumberGenerator.GetInt32(pool.Length)]);
        }

        var shuffled = chars.ToArray();
        RandomNumberGenerator.Shuffle(shuffled.AsSpan());
        return new string(shuffled, 0, length);
    }
}
