using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {
    public interface IReadBytes {

        /// <summary>
        /// Gets the position of the reader in its underlying data in the derived class.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the length of the underlying data in the derived class.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the derived class does not support it.</exception>
        long Length { get; }

        /// <summary>
        /// Reads a byte. 
        /// </summary>
        /// <returns>The byte that was read.</returns>
        /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached before the read is completed.</exception>
        byte ReadByte();

        /// <summary>
        /// Reads <paramref name="count"/> bytes into the given <paramref name="destination"/> starting at <paramref name="destinationOffset"/>.
        /// </summary>
        /// <param name="destination">The buffer that the bytes will be written to as they are read.</param>
        /// <param name="destinationOffset">The position in the <paramref name="destination"/> buffer at which to begin writing bytes.</param>
        /// <param name="count">The number of bytes to read into the <paramref name="destination"/> buffer.</param>
        /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached before the read is completed.</exception>
        void ReadBytes(byte[] destination, int destinationOffset, int count);

        /// <summary>
        /// Reads <paramref name="count"/> bytes into the given <paramref name="destination"/> starting at <paramref name="destinationOffset"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes when all the bytes have been read into the given <paramref name="destination"/></returns>
        /// <param name="destination">The buffer that the bytes will be written to as they are read.</param>
        /// <param name="destinationOffset">The position in the <paramref name="destination"/> buffer at which to begin writing bytes.</param>
        /// <param name="count">The number of bytes to read into the <paramref name="destination"/> buffer.</param>
        /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached before the read is completed.</exception>
        Task ReadBytesAsync(byte[] destination, int destinationOffset, int count);

        /// <summary>
        /// Copies all remaining data into the <paramref name="destination"/> writer.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="bufferSize">
        /// The size of the buffer to use during the copy operation. Default value is 81920 (80Kb).
        /// Not all implementations will actually use this value. Some will just do whatever's optimal for
        /// themselves.
        /// </param>
        void CopyTo(IWriteBytes destination, int bufferSize = 81920);

        /// <summary>
        /// Copies all remaining data into the <paramref name="destination"/> writer.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="bufferSize">
        /// The size of the buffer to use during the copy operation. Default value is 81920 (80Kb).
        /// Not all implementations will actually use this value. Some will just do whatever's optimal for
        /// themselves.
        /// </param>
        /// <returns>A <see cref="Task"/> that completes when the copy operation is completed.</returns>
        Task CopyToAsync(IWriteBytes destination, int bufferSize = 81920);
    }
}
