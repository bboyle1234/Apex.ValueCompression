using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Apex.ValueCompression.Compressors {

    public class Compressor {

        /// <summary>
        /// Creates a <see cref="CompressorBase{T}"/> that uses the given <paramref name="write"/> and <paramref name="read"/> methods.
        /// You can use this method as a quick and easy short-hand way to get a compressor class without writing all the boiler plate compressor code yourself.
        /// </summary>
        public static CompressorBase<T> Create<T>(Action<IWriteBytes, T> write, Func<IReadBytes, T> read)
            => new CompressorImpl<T>(write, read);

        private class CompressorImpl<T> : CompressorBase<T> {


            readonly Action<IWriteBytes, T> Write;
            readonly Func<IReadBytes, T> Read;

            public CompressorImpl(Action<IWriteBytes, T> write, Func<IReadBytes, T> read) {
                Write = write;
                Read = read;
            }

            public override void Compress(IWriteBytes stream, T value) => Write(stream, value);

            public override T Decompress(IReadBytes stream) => Read(stream);
        }
    }
}
