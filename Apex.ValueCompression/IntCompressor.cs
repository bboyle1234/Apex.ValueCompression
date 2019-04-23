using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class IntCompressor {

        const int SIX_BIT_DATA_MASK = 0x3F;     // 0011 1111
        const int MORE_MASK = 0x80;             // 1000 0000
        const int SIGN_MASK = 0x40;             // 0100 0000

        /// <summary>
        /// Cannot write int.MinValue
        /// </summary>
        public static void WriteCompressedInt(this Stream stream, int value) {
            var isPositive = value >= 0;
            value = Abs(value);

            if (value <= SIX_BIT_DATA_MASK) {
                stream.WriteByte((byte)(value | (isPositive ? 0 : SIGN_MASK)));
                return;
            }

            stream.WriteByte((byte)((value & SIX_BIT_DATA_MASK) | (isPositive ? 0 : SIGN_MASK) | MORE_MASK));
            value >>= 6;
            stream.WriteCompressedUInt((uint)value);
        }

        public static int ReadCompressedInt(this Stream input) {

            int firstByte = input.ReadByte();
            var result = firstByte & SIX_BIT_DATA_MASK;

            if (firstByte < MORE_MASK) {
                return ((firstByte & SIGN_MASK) == 0) ? result : -result;
            }

            result += (int)input.ReadCompressedUInt() << 6;
            return ((firstByte & SIGN_MASK) == 0) ? result : -result;
        }
    }
}
