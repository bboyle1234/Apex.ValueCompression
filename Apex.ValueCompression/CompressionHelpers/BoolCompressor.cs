using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    /// <summary>
    /// Writes boolean to the stream as a single byte.
    /// 0 = False, 1 = True, 2 = nullable boolean null.
    /// </summary>
    public static class BoolCompressor {

        public static void WriteCompressedBool(this IWriteBytes stream, bool value) {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public static bool ReadCompressedBool(this IReadBytes stream) {
            return stream.ReadByte() == 1;
        }

        public static void WriteCompressedNullableBool(this IWriteBytes stream, bool? value) {
            if (null == value) {
                stream.WriteByte(2);
            } else {
                stream.WriteByte(value.Value ? (byte)1 : (byte)0);
            }
        }

        public static bool? ReadCompressedNullableBool(this IReadBytes stream) {
            var byteValue = stream.ReadByte();
            if (byteValue == 2) return null;
            return byteValue == 1;
        }
    }
}
