using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class DecimalCompressor {

        public static void WriteCompressedDecimal(this Stream stream, decimal value) {
            value = value / 1.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000m;
            var bits = decimal.GetBits(value);
            stream.WriteCompressedInt(bits[0]);
            stream.WriteCompressedInt(bits[1]);
            stream.WriteCompressedInt(bits[2]);
            stream.WriteCompressedInt(bits[3]);
        }

        public static decimal ReadCompressedDecimal(this Stream stream) {
            var bits = new int[4];
            bits[0] = stream.ReadCompressedInt();
            bits[1] = stream.ReadCompressedInt();
            bits[2] = stream.ReadCompressedInt();
            bits[3] = stream.ReadCompressedInt();
            return new decimal(bits);
        }

        public static void WriteCompressedNullableDecimal(this Stream stream, decimal? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedDecimal(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static decimal? ReadCompressedNullableDecimal(this Stream stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedDecimal();
        }
    }
}
