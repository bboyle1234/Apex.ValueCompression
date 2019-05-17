using System.IO;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    public static class IReadBytesExtensions {

        /// <summary>
        /// Reads <paramref name="count"/> bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The array of bytes that was read.</returns>
        /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached before the read is completed.</exception>
        public static byte[] ReadBytes(this IReadBytes reader, int count) {
            var bytes = new byte[count];
            reader.ReadBytes(bytes, 0, count);
            return bytes;
        }

        /// <summary>
        /// Asyncronously reads <paramref name="count"/> bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>A <see cref="Task"/> that completes when the entire read operation has completed. The result of the task contains the array of bytes that was read.</returns>
        /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached before the read is completed.</exception>
        public static async Task<byte[]> ReadBytesAsync(this IReadBytes reader, int count) {
            var bytes = new byte[count];
            await reader.ReadBytesAsync(bytes, 0, count).ConfigureAwait(false);
            return bytes;
        }


        public static byte[] ReadRemaining(this IReadBytes reader) {
            using (var ms = new MemoryStream()) {
                reader.CopyTo(ms.AsIWriteBytes());
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> ReadRemainingAsync(this IReadBytes reader) {
            using (var ms = new MemoryStream()) {
                await reader.CopyToAsync(ms.AsIWriteBytes()).ConfigureAwait(false);
                return ms.ToArray();
            }
        }
    }
}
