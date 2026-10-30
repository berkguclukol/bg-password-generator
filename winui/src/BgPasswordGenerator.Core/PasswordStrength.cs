namespace BgPasswordGenerator.Core;

public enum Strength
{
    Weak,
    Moderate,
    Strong,
}

public static class PasswordStrength
{
    // Same rules as the Electron version's printStrongNess()
    public static Strength Evaluate(string password)
    {
        var hasLower = password.Any(c => c is >= 'a' and <= 'z');
        var hasUpper = password.Any(c => c is >= 'A' and <= 'Z');
        var hasDigit = password.Any(c => c is >= '0' and <= '9');
        var hasSpecial = password.Any(c => !char.IsAsciiLetterOrDigit(c) && c != ' ');

        if (hasLower && hasUpper && hasDigit && hasSpecial && password.Length >= 8)
        {
            return Strength.Strong;
        }
        if ((hasLower || hasUpper) && hasSpecial && password.Length >= 6)
        {
            return Strength.Moderate;
        }
        return Strength.Weak;
    }
}
