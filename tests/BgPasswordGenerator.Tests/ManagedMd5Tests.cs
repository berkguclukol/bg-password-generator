using System.Security.Cryptography;
using System.Text;
using BgPasswordGenerator.Core;

namespace BgPasswordGenerator.Tests;

public class ManagedMd5Tests
{
    [Theory]
    [InlineData("", "d41d8cd98f00b204e9800998ecf8427e")]
    [InlineData("a", "0cc175b9c0f1b6a831c399e269772661")]
    [InlineData("abc", "900150983cd24fb0d6963f7d28e17f72")]
    [InlineData("message digest", "f96b697d7cb7938d525a2f31aaf161d0")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "c3fcd3d76192e4007dfb496cca67e13b")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57edf4a22be3c955ac49da2e2107b67a")]
    public void MatchesRfc1321TestSuite(string text, string expected)
    {
        Assert.Equal(expected, Convert.ToHexStringLower(ManagedMd5.Hash(Encoding.ASCII.GetBytes(text))));
    }

    [Fact]
    public void MatchesDotNetMd5AcrossBlockBoundaries()
    {
        for (var length = 0; length <= 300; length++)
        {
            var data = RandomNumberGenerator.GetBytes(length);
            Assert.Equal(MD5.HashData(data), ManagedMd5.Hash(data));
        }
    }
}
