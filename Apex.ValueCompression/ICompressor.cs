using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression {

    public interface ICompressor<in T> {
        void Compress(Stream stream, T value);
    }

    public interface IDecompressor<out T> {
        T Decompress(Stream stream);
    }
}
