using Apex.TimeStamps;
using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class MonthStampCompressor {

        public static void WriteCompressedMonthStamp(this IWriteBytes stream, MonthStamp value) {
            stream.WriteCompressedUInt((uint)(value.Year * 12 + value.Month));
        }

        public static MonthStamp ReadCompressedMonthStamp(this IReadBytes stream) {
            var value = (int)stream.ReadCompressedUInt();
            return new MonthStamp(value / 12, value % 12);
        }

        public static void WriteCompressedNullableMonthStamp(this IWriteBytes stream, MonthStamp? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedMonthStamp(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static MonthStamp? ReadCompressedNullableMonthStamp(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedMonthStamp();
        }
    }
}
