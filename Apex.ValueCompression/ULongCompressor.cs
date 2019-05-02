using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class ULongCompressor {

        const ulong DATA_MASK = 0x7F; // 0111 1111
        const ulong MORE_MASK = 0x80; // 1000 0000

        public static void WriteCompressedULong(this Stream stream, ulong value) {
            while (value > DATA_MASK) {
                stream.WriteByte((byte)(value & DATA_MASK));
                value >>= 7;
            }
            stream.WriteByte((byte)(value | MORE_MASK));
        }

        public static ulong ReadCompressedULong(this Stream stream) {
            ulong result = 0;
            int shiftBits = 0;
            ulong inputByte = (ulong)stream.ReadByte();
            while ((inputByte < MORE_MASK)) {
                result |= (inputByte) << shiftBits;
                inputByte = (ulong)stream.ReadByte();
                shiftBits += 7;
            }
            return result | ((inputByte & DATA_MASK) << shiftBits);
        }

        public static void WriteCompressedNullableULong(this Stream stream, ulong? value) {
            if (value.HasValue) {
                stream.WriteCompressedInt(1);
                stream.WriteCompressedULong(value.Value);
            } else {
                stream.WriteCompressedInt(0);
            }
        }

        public static ulong? ReadCompressedNullableULong(this Stream stream) {
            if (stream.ReadCompressedInt() == 0) return null;
            return stream.ReadCompressedULong();
        }

    }
}
