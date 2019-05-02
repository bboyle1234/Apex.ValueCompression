using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression {

    public interface ICompressor {
        void Compress(Stream stream, object value);
    }

    public interface IDecompressor {
        object Decompress(Stream stream);
    }

    public interface ICompressor<in T> : ICompressor {
        void Compress(Stream stream, T value);
    }

    public interface IDecompressor<out T> : IDecompressor {
        new T Decompress(Stream stream);
    }

    public abstract class CompressorBase<T> : ICompressor<T>, IDecompressor<T> {

        public void Compress(Stream stream, object value)
            => Compress(stream, (T)value);

        object IDecompressor.Decompress(Stream stream)
            => this.Decompress(stream);

        public abstract void Compress(Stream stream, T value);

        public abstract T Decompress(Stream stream);
    }
}
