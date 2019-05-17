using Apex.TimeStamps;
using System;
using System.Globalization;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class TimeStampCompressor {

        public static void WriteCompressedTimeStamp(this IWriteBytes stream, TimeStamp value) {
            stream.WriteCompressedLong(value.TicksUtc);
        }

        public static TimeStamp ReadCompressedTimeStamp(this IReadBytes stream) {
            return new TimeStamp(stream.ReadCompressedLong());
        }

        public static void WriteCompressedNullableTimeStamp(this IWriteBytes stream, TimeStamp? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedTimeStamp(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static TimeStamp? ReadCompressedNullableTimeStamp(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedTimeStamp();
        }
    }
}
