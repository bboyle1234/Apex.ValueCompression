using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression.Compressors {

    public interface ICompressor {
        void Compress(Stream stream, object value);
    }

    public interface ICompressor<in T> : ICompressor {
        void Compress(Stream stream, T value);
    }
}
