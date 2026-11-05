using System.Buffers.Binary;
using System.Numerics;

namespace BgPasswordGenerator.Core;

/// <summary>
/// MD5 from RFC 1321 for WebAssembly, where .NET's <see cref="System.Security.Cryptography.MD5"/> isn't supported.
/// </summary>
internal static class ManagedMd5
{
    private const int BlockSize = 64;

    private static readonly int[] Shifts =
    [
        7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
        5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
        4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
        6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21,
    ];

    private static readonly uint[] Constants =
        Enumerable.Range(1, BlockSize).Select(i => (uint)(long)(Math.Abs(Math.Sin(i)) * 4294967296.0)).ToArray();

    public static byte[] Hash(ReadOnlySpan<byte> data)
    {
        // Padding: 0x80, zeros up to 8 bytes short of a block, then the length in bits
        var paddedLength = ((data.Length + 8) / BlockSize + 1) * BlockSize;
        var message = new byte[paddedLength];
        data.CopyTo(message);
        message[data.Length] = 0x80;
        BinaryPrimitives.WriteUInt64LittleEndian(message.AsSpan(paddedLength - 8), (ulong)data.Length * 8);

        uint a0 = 0x67452301, b0 = 0xefcdab89, c0 = 0x98badcfe, d0 = 0x10325476;
        Span<uint> words = stackalloc uint[16];
        for (var offset = 0; offset < paddedLength; offset += BlockSize)
        {
            for (var i = 0; i < words.Length; i++)
            {
                words[i] = BinaryPrimitives.ReadUInt32LittleEndian(message.AsSpan(offset + i * 4));
            }

            uint a = a0, b = b0, c = c0, d = d0;
            for (var i = 0; i < BlockSize; i++)
            {
                var (mix, word) = (i / 16) switch
                {
                    0 => ((b & c) | (~b & d), i),
                    1 => ((d & b) | (~d & c), (5 * i + 1) % 16),
                    2 => (b ^ c ^ d, (3 * i + 5) % 16),
                    _ => (c ^ (b | ~d), 7 * i % 16),
                };
                var rotated = BitOperations.RotateLeft(a + mix + Constants[i] + words[word], Shifts[i]);
                (a, d, c, b) = (d, c, b, b + rotated);
            }

            a0 += a;
            b0 += b;
            c0 += c;
            d0 += d;
        }

        var hash = new byte[16];
        BinaryPrimitives.WriteUInt32LittleEndian(hash, a0);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(4), b0);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(8), c0);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(12), d0);
        return hash;
    }
}
