using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {
    public static class EnumCompressor {

        public static void WriteCompressedEnum<T>(this Stream stream, T value) where T : Enum {
            stream.WriteCompressedInt((int)(object)value);
        }

        public static T ReadCompressedEnum<T>(this Stream stream) where T : Enum {
            return (T)(object)stream.ReadCompressedInt();
        }

        public static void WriteCompressedNullableEnum<T>(this Stream stream, T? value) where T : struct, Enum {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedEnum(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static T? ReadCompressedNullableEnum<T>(this Stream stream) where T : struct, Enum {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedEnum<T>();
        }
    }
}
