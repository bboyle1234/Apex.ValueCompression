using System.IO;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Inherit this class to create a quick simple implementation of <see cref="ICompressor{T}"/> and <see cref="IDecompressor{T}"/>
    /// </summary>
    public abstract class CompressorBase<T> : ICompressor<T>, IDecompressor<T> {

        void ICompressor.Compress(IWriteBytes stream, object value)
            => Compress(stream, (T)value);

        object IDecompressor.Decompress(IReadBytes stream)
            => Decompress(stream);

        public abstract void Compress(IWriteBytes stream, T value);

        public abstract T Decompress(IReadBytes stream);
    }
}
