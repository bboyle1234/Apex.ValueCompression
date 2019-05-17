using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression.Compressors {

    public interface ICompressor {
        void Compress(IWriteBytes stream, object value);
    }

    public interface ICompressor<in T> : ICompressor {
        void Compress(IWriteBytes stream, T value);
    }
}
