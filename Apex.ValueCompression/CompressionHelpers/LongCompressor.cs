using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class LongCompressor {

        const long FIVE_BIT_DATA_MASK = 0x1F;     // 0001 1111
        const long MORE_MASK = 0x80;             // 1000 0000
        const long SIGN_MASK = 0x40;             // 0100 0000
        const long MIN_VALUE_MASK = 0x20;        // 0010 0000

        public static void WriteCompressedLong(this IWriteBytes stream, long value) {

            /// Because the twos-complement representation of long.MinValue cannot be negated.
            if (value == long.MinValue) {
                stream.WriteByte((byte)MIN_VALUE_MASK);
                return;
            }

            var isPositive = value >= 0;
            value = Abs(value);

            if (value <= FIVE_BIT_DATA_MASK) {
                stream.WriteByte((byte)(value | (isPositive ? 0 : SIGN_MASK)));
                return;
            }

            stream.WriteByte((byte)((value & FIVE_BIT_DATA_MASK) | (isPositive ? 0 : SIGN_MASK) | MORE_MASK));
            value >>= 5;
            stream.WriteCompressedULong((ulong)value);
        }

        public static long ReadCompressedLong(this IReadBytes input) {
            long firstByte = input.ReadByte();
            if ((firstByte & MIN_VALUE_MASK) > 0) return long.MinValue;

            var result = firstByte & FIVE_BIT_DATA_MASK;

            if (firstByte < MORE_MASK) {
                return ((firstByte & SIGN_MASK) == 0) ? result : -result;
            }

            result += (long)input.ReadCompressedULong() << 5;
            return ((firstByte & SIGN_MASK) == 0) ? result : -result;
        }

        public static void WriteCompressedNullableLong(this IWriteBytes stream, long? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedLong(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static long? ReadCompressedNullableLong(this IReadBytes stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedLong();
        }
    }
}
