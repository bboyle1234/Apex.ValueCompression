using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class IntCompressor {

        const int FIVE_BIT_DATA_MASK = 0x1F;    // 0001 1111
        const int MORE_MASK = 0x80;             // 1000 0000
        const int SIGN_MASK = 0x40;             // 0100 0000
        const int MIN_VALUE_MASK = 0x20;        // 0010 0000

        public static void WriteCompressedInt(this Stream stream, int value) {

            /// Because the twos-complement representation of int.MinValue cannot be negated.
            if (value == int.MinValue) {
                stream.WriteByte(MIN_VALUE_MASK);
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
            stream.WriteCompressedUInt((uint)value);
        }

        public static int ReadCompressedInt(this Stream input) {

            int firstByte = input.ReadByte();
            if ((firstByte & MIN_VALUE_MASK) > 0) return int.MinValue;

            var result = firstByte & FIVE_BIT_DATA_MASK;

            if (firstByte < MORE_MASK) {
                return ((firstByte & SIGN_MASK) == 0) ? result : -result;
            }

            result += (int)input.ReadCompressedUInt() << 5;
            return ((firstByte & SIGN_MASK) == 0) ? result : -result;
        }

        public static void WriteCompressedNullableInt(this Stream stream, int? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedInt(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static int? ReadCompressedNullableInt(this Stream stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedInt();
        }
    }
}
