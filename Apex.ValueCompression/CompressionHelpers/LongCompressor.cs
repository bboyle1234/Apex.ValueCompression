using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    /// <summary>
    /// Uses Zig-Zag encoding to achieve small compression for negative as well as positive numbers.
    /// See: 
    /// https://developers.google.com/protocol-buffers/docs/encoding#signed-integers
    /// https://stackoverflow.com/questions/2210923/zig-zag-decoding
    /// </summary>
    public static class LongCompressor {

        public static void WriteCompressedLong(this IWriteBytes stream, long value) {
            stream.WriteCompressedULong((ulong)((value << 1) ^ (value >> 63)));
        }

        public static long ReadCompressedLong(this IReadBytes input) {
            var value = input.ReadCompressedULong();
            return (long)((value >> 1) ^ (~(value & 1) + 1));
        }

        public static void WriteCompressedNullableLong(this IWriteBytes stream, long? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedLong(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static long? ReadCompressedNullableLong(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedLong();
        }
    }
}
