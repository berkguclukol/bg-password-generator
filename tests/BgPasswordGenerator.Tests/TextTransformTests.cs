using BgPasswordGenerator.Core;

namespace BgPasswordGenerator.Tests;

public class TextTransformTests
{
    [Theory]
    [InlineData(TextOperation.Md5, "900150983cd24fb0d6963f7d28e17f72")]
    [InlineData(TextOperation.Sha1, "a9993e364706816aba3e25717850c26c9cd0d89d")]
    [InlineData(TextOperation.Sha256, "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData(TextOperation.Sha512, "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
    public void HashesMatchPublishedTestVectors(TextOperation operation, string expected)
    {
        Assert.Equal(expected, TextTransform.Apply(operation, "abc"));
        Assert.Equal(expected.ToUpperInvariant(), TextTransform.Apply(operation, "abc", uppercaseHex: true));
    }

    [Theory]
    [InlineData("a\r\nb")]
    [InlineData("a\rb")]
    public void LineEndingsCountAsLf(string text)
    {
        Assert.Equal(TextTransform.Apply(TextOperation.Sha256, "a\nb"), TextTransform.Apply(TextOperation.Sha256, text));
        Assert.Equal("YQpi", TextTransform.Apply(TextOperation.Base64Encode, text));
    }

    [Theory]
    [InlineData("abc", "YWJj")]
    [InlineData("şifre", "xZ9pZnJl")]
    [InlineData("", "")]
    public void Base64RoundTripsUtf8Text(string text, string base64)
    {
        Assert.Equal(base64, TextTransform.Apply(TextOperation.Base64Encode, text));
        Assert.Equal(text, TextTransform.Apply(TextOperation.Base64Decode, base64));
    }

    [Fact]
    public void Base64DecodeIgnoresWhitespace()
    {
        Assert.Equal("abc", TextTransform.Apply(TextOperation.Base64Decode, " YW\r\nJj\n"));
    }

    [Theory]
    [InlineData("not base64!")]
    [InlineData("YWJ")]
    [InlineData("/w==")] // Valid Base64 of the byte 0xFF, which isn't UTF-8 text
    public void Base64DecodeRejectsInvalidInput(string text)
    {
        Assert.Throws<FormatException>(() => TextTransform.Apply(TextOperation.Base64Decode, text));
    }

    [Theory]
    [InlineData(TextOperation.Md5, true)]
    [InlineData(TextOperation.Sha512, true)]
    [InlineData(TextOperation.Base64Encode, false)]
    [InlineData(TextOperation.Base64Decode, false)]
    public void IsHashSeparatesHashesFromEncodings(TextOperation operation, bool expected)
    {
        Assert.Equal(expected, TextTransform.IsHash(operation));
    }
}
