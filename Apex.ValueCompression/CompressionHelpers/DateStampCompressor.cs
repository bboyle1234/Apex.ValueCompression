using Apex.TimeStamps;
using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class DateStampCompressor {

        public static void WriteCompressedDateStamp(this IWriteBytes stream, DateStamp value) {
            stream.WriteCompressedUInt((uint)((value.Year * 12 + value.Month - 1) * 100 + value.Day));
        }

        public static DateStamp ReadCompressedDateStamp(this IReadBytes stream) {
            var value = (int)stream.ReadCompressedUInt();
            return new DateStamp(value / 100 / 12, (value / 100 % 12) + 1, value % 100);
        }

        public static void WriteCompressedNullableDateStamp(this IWriteBytes stream, DateStamp? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedDateStamp(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static DateStamp? ReadCompressedNullableDateStamp(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedDateStamp();
        }
    }
}
