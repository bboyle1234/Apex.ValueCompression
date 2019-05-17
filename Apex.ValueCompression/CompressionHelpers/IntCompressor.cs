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
    public static class IntCompressor {

        public static void WriteCompressedInt(this IWriteBytes stream, int value) {
            stream.WriteCompressedUInt((uint)((value << 1) ^ (value >> 31)));
        }

        public static int ReadCompressedInt(this IReadBytes input) {
            var value = input.ReadCompressedUInt();
            return (int)((value >> 1) ^ (~(value & 1) + 1));
        }

        public static void WriteCompressedNullableInt(this IWriteBytes stream, int? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedInt(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static int? ReadCompressedNullableInt(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedInt();
        }
    }
}
