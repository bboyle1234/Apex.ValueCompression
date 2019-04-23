using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class LongCompressor {

        const long SIX_BIT_DATA_MASK = 0x3F;     // 0011 1111
        const long SEVEN_BIT_DATA_MASK = 0x7F;   // 0111 1111
        const long MORE_MASK = 0x80;             // 1000 0000
        const long SIGN_MASK = 0x40;             // 0100 0000

        /// <summary>
        /// Cannot write long.MinValue
        /// </summary>
        public static void WriteCompressedLong(this Stream stream, long value) {
            var isPositive = value >= 0;
            value = Abs(value);

            if (value <= SIX_BIT_DATA_MASK) {
                stream.WriteByte((byte)(value | (isPositive ? 0 : SIGN_MASK)));
                return;
            }

            stream.WriteByte((byte)((value & SIX_BIT_DATA_MASK) | (isPositive ? 0 : SIGN_MASK) | MORE_MASK));
            value >>= 6;
            stream.WriteCompressedULong((ulong)value);
        }

        public static long ReadCompressedLong(this Stream input) {
            long firstByte = input.ReadByte();
            var result = firstByte & SIX_BIT_DATA_MASK;

            if (firstByte < MORE_MASK) {
                return ((firstByte & SIGN_MASK) == 0) ? result : -result;
            }

            result += (long)input.ReadCompressedULong() << 6;
            return ((firstByte & SIGN_MASK) == 0) ? result : -result;
        }
    }
}
