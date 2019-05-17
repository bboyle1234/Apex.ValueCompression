using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class DateTimeCompressor {

        public static void WriteCompressedDateTime(this IWriteBytes stream, DateTime value) {
            stream.WriteCompressedLong(value.Ticks);
            stream.WriteCompressedEnum(value.Kind);
        }

        public static DateTime ReadCompressedDateTime(this IReadBytes stream) {
            return new DateTime(stream.ReadCompressedLong(), stream.ReadCompressedEnum<DateTimeKind>());
        }

        public static void WriteCompressedNullableDateTime(this IWriteBytes stream, DateTime? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedDateTime(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static DateTime? ReadCompressedNullableDateTime(this IReadBytes stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedDateTime();
        }
    }
}
