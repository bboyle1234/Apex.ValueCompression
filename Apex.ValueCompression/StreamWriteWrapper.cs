using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    internal sealed class StreamWriteWrapper : IWriteBytes {

        readonly Stream Stream;

        public StreamWriteWrapper(Stream stream) {
            Stream = stream;
        }

        /// <inheritdoc />
        public long Position
            => Stream.Position;

        /// <inheritdoc />
        public void WriteByte(byte value)
            => Stream.WriteByte(value);

        /// <inheritdoc />
        public void WriteBytes(byte[] bytes)
            => Stream.Write(bytes, 0, bytes.Length);

        /// <inheritdoc />
        public void WriteBytes(byte[] bytes, int offset, int count)
            => Stream.Write(bytes, offset, count);

        /// <inheritdoc />
        public Task WriteBytesAsync(byte[] bytes)
            => Stream.WriteAsync(bytes, 0, bytes.Length);

        /// <inheritdoc />
        public Task WriteBytesAsync(byte[] bytes, int offset, int count)
            => Stream.WriteAsync(bytes, offset, count);
    }
}
