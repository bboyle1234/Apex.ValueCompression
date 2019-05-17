using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class BoolCompressor {

        public static void WriteCompressedBool(this IWriteBytes stream, bool value) {
            stream.WriteCompressedInt(value ? 1 : 0);
        }

        public static bool ReadCompressedBool(this IReadBytes stream) {
            return stream.ReadCompressedInt() == 1;
        }

        public static void WriteCompressedNullableBool(this IWriteBytes stream, bool? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedBool(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static bool? ReadCompressedNullableBool(this IReadBytes stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedBool();
        }
    }
}
