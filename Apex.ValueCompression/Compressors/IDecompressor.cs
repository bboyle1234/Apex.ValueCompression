using System.IO;

namespace Apex.ValueCompression.Compressors {

    public interface IDecompressor {
        object Decompress(Stream stream);
    }

    public interface IDecompressor<out T> : IDecompressor {
        new T Decompress(Stream stream);
    }

}
