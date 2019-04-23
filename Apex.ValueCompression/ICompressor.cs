using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression {
    public interface ICompressor<T> {

        void Compress(Stream stream, T value);

        T Decompress(Stream stream);
    }
}
