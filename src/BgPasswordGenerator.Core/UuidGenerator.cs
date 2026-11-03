namespace BgPasswordGenerator.Core;

public static class UuidGenerator
{
    /// <summary>
    /// Returns a random (version 4) UUID. <see cref="Guid.NewGuid"/> draws its bits from the OS CSPRNG.
    /// </summary>
    public static string Generate(bool uppercase, bool hyphens)
    {
        var value = Guid.NewGuid().ToString(hyphens ? "D" : "N");
        return uppercase ? value.ToUpperInvariant() : value;
    }
}
