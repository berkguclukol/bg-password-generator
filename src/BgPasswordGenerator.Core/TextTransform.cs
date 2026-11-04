using System.Security.Cryptography;
using System.Text;

namespace BgPasswordGenerator.Core;

public enum TextOperation
{
    Md5,
    Sha1,
    Sha256,
    Sha512,
    Base64Encode,
    Base64Decode,
}

public static class TextTransform
{
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public static bool IsHash(TextOperation operation) => operation is not (TextOperation.Base64Encode or TextOperation.Base64Decode);

    /// <summary>
    /// Applies the operation to UTF-8 text. Line endings count as LF, so the result matches
    /// tools like sha256sum regardless of how the text was typed or pasted.
    /// </summary>
    /// <exception cref="FormatException">The input of <see cref="TextOperation.Base64Decode"/> isn't Base64 of UTF-8 text.</exception>
    public static string Apply(TextOperation operation, string text, bool uppercaseHex = false)
    {
        if (operation == TextOperation.Base64Decode)
        {
            return DecodeBase64(text);
        }

        var bytes = Encoding.UTF8.GetBytes(NormalizeLineEndings(text));
        return operation switch
        {
            TextOperation.Md5 => ToHex(MD5.HashData(bytes), uppercaseHex),
            TextOperation.Sha1 => ToHex(SHA1.HashData(bytes), uppercaseHex),
            TextOperation.Sha256 => ToHex(SHA256.HashData(bytes), uppercaseHex),
            TextOperation.Sha512 => ToHex(SHA512.HashData(bytes), uppercaseHex),
            TextOperation.Base64Encode => Convert.ToBase64String(bytes),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    private static string DecodeBase64(string text)
    {
        var bytes = Convert.FromBase64String(text);
        try
        {
            return StrictUtf8.GetString(bytes);
        }
        catch (DecoderFallbackException ex)
        {
            throw new FormatException("The decoded bytes aren't UTF-8 text.", ex);
        }
    }

    // WinUI's TextBox reports line breaks as CR, and pasted text can contain CRLF
    private static string NormalizeLineEndings(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n');

    private static string ToHex(byte[] hash, bool uppercase) =>
        uppercase ? Convert.ToHexString(hash) : Convert.ToHexStringLower(hash);
}
