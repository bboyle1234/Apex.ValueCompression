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
                stream.WriteCompressedBool(true);
                stream.WriteFullDouble(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static double? ReadNullableFullDouble(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadFullDouble();
        }

        public static void WriteDoubleOffset(this IWriteBytes stream, double seed, double tickSize, double value) {
            if (value == seed) {
                /// Since this is true 80% of the time in a trading platform, we use this optimization for significant performance gains.
                stream.WriteCompressedInt(0);
            } else {
                stream.WriteCompressedInt(ToTicks(value - seed, tickSize));
            }
        }

        public static double ReadDoubleOffset(this IReadBytes stream, double seed, double tickSize) {
            var numTicks = stream.ReadCompressedInt();
            /// Since this is true 80% of the time in a trading platform, we use this optimization for significant performance gains.
            if (numTicks == 0) return seed;
            return ToValue(tickSize, ToTicks(seed, tickSize) + numTicks);
        }

        static int ToTicks(double value, double tickSize)
            => (int)Round(value / tickSize, MidpointRounding.AwayFromZero);

        static double ToValue(double tickSize, int numTicks)
            => (double)(numTicks * (decimal)tickSize);
    }
}
