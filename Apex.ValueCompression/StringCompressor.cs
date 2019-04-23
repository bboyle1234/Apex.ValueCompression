using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Math;

namespace Apex.ValueCompression {

    public static class StringCompressor {

        static readonly Encoding Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static void WriteCompressedString(this Stream stream, string value) {
            var bytes = Encoding.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0);
        }

        public static string ReadCompressedString(this Stream stream) {
            var bytes = new List<byte>();
            while (true) {
                var b = stream.ReadByte();
                if (b <= 0) return Encoding.GetString(bytes.ToArray());
                bytes.Add((byte)b);
            }
        }
    }
}
