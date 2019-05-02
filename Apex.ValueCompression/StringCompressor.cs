using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Math;

namespace Apex.ValueCompression {

    public static class StringCompressor {

        static readonly Encoding Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static void WriteCompressedString(this Stream stream, string value) {
            if (null == value) {
                stream.WriteCompressedInt(0);
            } else {
                stream.WriteCompressedInt(1);
                var bytes = Encoding.GetBytes(value);
                UIntCompressor.WriteCompressedUInt(stream, (uint)bytes.Length);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public static string ReadCompressedString(this Stream stream) {
            if (0 == stream.ReadCompressedInt()) return null;
            var length = (int)UIntCompressor.ReadCompressedUInt(stream);
            var bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return Encoding.GetString(bytes);
        }
    }
}
