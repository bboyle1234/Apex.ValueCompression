using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {
    public static class EnumCompressor {

        public static void WriteCompressedEnum<T>(this IWriteBytes stream, T value) where T : Enum {
            stream.WriteCompressedInt((int)(object)value);
        }

        public static T ReadCompressedEnum<T>(this IReadBytes stream) where T : Enum {
            return (T)(object)stream.ReadCompressedInt();
        }

        public static void WriteCompressedNullableEnum<T>(this IWriteBytes stream, T? value) where T : struct, Enum {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedEnum(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static T? ReadCompressedNullableEnum<T>(this IReadBytes stream) where T : struct, Enum {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedEnum<T>();
        }
    }
}
