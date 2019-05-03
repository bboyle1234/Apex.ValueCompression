using System.IO;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Inherit this class to create a quick simple implementation of <see cref="ICompressor{T}"/> and <see cref="IDecompressor{T}"/>
    /// </summary>
    public abstract class CompressorBase<T> : ICompressor<T>, IDecompressor<T> {

        public void Compress(Stream stream, object value)
            => Compress(stream, (T)value);

        object IDecompressor.Decompress(Stream stream)
            => this.Decompress(stream);

        public abstract void Compress(Stream stream, T value);

        public abstract T Decompress(Stream stream);
    }
}
