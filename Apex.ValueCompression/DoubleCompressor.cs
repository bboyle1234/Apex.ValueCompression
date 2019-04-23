using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {
    public static class DoubleCompressor {

        public static void WriteFullDouble(this Stream stream, double value) {
            stream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public static double ReadFullDouble(this Stream stream) {
            var bytes = new byte[8];
            stream.Read(bytes, 0, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static void WriteDoubleOffset(this Stream stream, double seed, double tickSize, double value) {
            if (value == seed) {
                stream.WriteCompressedInt(0);
            } else {
                stream.WriteCompressedInt((int)Round((value - seed) / tickSize));
            }
        }

        public static double ReadDoubleOffset(this Stream stream, double seed, double tickSize) {
            return (double)((decimal)Round((seed / tickSize) + stream.ReadCompressedInt()) * (decimal)tickSize);
        }
    }
}
