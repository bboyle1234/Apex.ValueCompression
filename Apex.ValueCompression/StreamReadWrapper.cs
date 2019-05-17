using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    internal sealed class StreamReadWrapper : IReadBytes {

        readonly Stream Stream;

        public StreamReadWrapper(Stream stream) {
            Stream = stream;
        }

        /// <inheritdoc />
        public long Position
            => Stream.Position;

        /// <inheritdoc />
        public long Length
            => Stream.Length;

        /// <inheritdoc />
        public byte ReadByte() {
            var result = Stream.ReadByte();
            if (result < 0) EndOfStreamException.ThrowRead();
            return (byte)result;
        }

        /// <inheritdoc />
        public void ReadBytes(byte[] buffer, int offset, int count) {
            var totalBytesRead = 0;
            while (totalBytesRead < count) {
                var bytesRead = Stream.Read(buffer, totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0) EndOfStreamException.ThrowRead();
                totalBytesRead += bytesRead;
            }
        }

        /// <inheritdoc />
        public async Task ReadBytesAsync(byte[] buffer, int offset, int count) {
            var totalBytesRead = 0;
            while (totalBytesRead < count) {
                var bytesRead = await Stream.ReadAsync(buffer, totalBytesRead, count - totalBytesRead).ConfigureAwait(false);
                if (bytesRead == 0) EndOfStreamException.ThrowRead();
                totalBytesRead += bytesRead;
            }
        }

        /// <inheritdoc />
        public void CopyTo(IWriteBytes destination, int bufferSize = 81920) {
            var buffer = new byte[bufferSize];
            var bytesRead = Stream.Read(buffer, 0, bufferSize);
            while (bytesRead > 0) {
                destination.WriteBytes(buffer, 0, bytesRead);
                bytesRead = Stream.Read(buffer, 0, bufferSize);
            }
        }

        /// <inheritdoc />
        public async Task CopyToAsync(IWriteBytes destination, int bufferSize = 81920) {
            var buffer = new byte[bufferSize];
            var bytesRead = await Stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);
            while (bytesRead > 0) {
                await destination.WriteBytesAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                bytesRead = await Stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);
            }
        }
    }
}
