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
                stream.WriteCompressedInt(1);
                stream.WriteCompressedTimeStamp(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static TimeStamp? ReadCompressedNullableTimeStamp(this IReadBytes stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedTimeStamp();
        }
    }
}
