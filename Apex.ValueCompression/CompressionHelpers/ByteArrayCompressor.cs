using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    public static class ByteArrayCompressor {

        public static void WriteCompressedByteArray(this IWriteBytes stream, byte[] bytes) {
            if (bytes is null) {
                stream.WriteCompressedBool(false);
            } else {
                stream.WriteCompressedBool(true);
                stream.WriteCompressedUInt((uint)bytes.Length);
                stream.WriteBytes(bytes);
            }
        }

        public static byte[] ReadCompressedByteArray(this IReadBytes stream) {
            if (!stream.ReadCompressedBool()) return null;
            var length = (int)stream.ReadCompressedUInt();
            return stream.ReadBytes(length);
        }
    }
}
