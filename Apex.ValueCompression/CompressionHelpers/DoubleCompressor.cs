using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class DoubleCompressor {

        public static void WriteFullDouble(this IWriteBytes stream, double value) {
            stream.WriteBytes(BitConverter.GetBytes(value), 0, 8);
        }

        public static double ReadFullDouble(this IReadBytes stream) {
            var bytes = new byte[8];
            stream.ReadBytes(bytes, 0, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static void WriteNullableFullDouble(this IWriteBytes stream, double? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteFullDouble(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static double? ReadNullableFullDouble(this IReadBytes stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadFullDouble();
        }

        public static void WriteDoubleOffset(this IWriteBytes stream, double seed, double tickSize, double value) {
            if (value == seed) {
                stream.WriteCompressedInt(0);
            } else {
                stream.WriteCompressedInt((int)Round((value - seed) / tickSize));
            }
        }

        public static double ReadDoubleOffset(this IReadBytes stream, double seed, double tickSize) {
            return (double)((decimal)Round((seed / tickSize) + stream.ReadCompressedInt()) * (decimal)tickSize);
        }
    }
}
