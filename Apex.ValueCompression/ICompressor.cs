using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apex.ValueCompression {

    public interface ICompressor<T> {

        void WriteCompressedValue(Stream stream, T value);

        T ReadCompressedValue(Stream stream);
    }
}
