﻿using System;
using System.IO;
using static System.Math;

namespace Apex.ValueCompression {

    public static class UIntCompressor {

        const uint DATA_MASK = 0x7F; // 0111 1111
        const uint MORE_MASK = 0x80; // 1000 0000

        public static void WriteCompressedUInt(this IWriteBytes stream, uint value) {
            while (value > DATA_MASK) {
                stream.WriteByte((byte)((value & DATA_MASK)));
                value >>= 7;
            }
            stream.WriteByte((byte)(value | MORE_MASK));
        }

        public static uint ReadCompressedUInt(this IReadBytes stream) {
            uint result = 0;
            int shiftBits = 0;

            int inputByte = stream.ReadByte();

            while ((inputByte < MORE_MASK)) {
                result |= (uint)((inputByte) << shiftBits);
                inputByte = stream.ReadByte();
                shiftBits += 7;
            }

            return result | (uint)((inputByte & DATA_MASK) << shiftBits);
        }

        public static void WriteCompressedNullableUInt(this IWriteBytes stream, uint? value) {
            if (value.HasValue) {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedUInt(value.Value);
            } else {
                stream.WriteCompressedBool(false);
            }
        }

        public static uint? ReadCompressedNullableUInt(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            return stream.ReadCompressedUInt();
        }

    }
}
