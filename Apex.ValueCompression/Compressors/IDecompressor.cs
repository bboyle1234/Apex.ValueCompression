using System.IO;

namespace Apex.ValueCompression.Compressors {

    public interface IDecompressor {
        object Decompress(IReadBytes stream);
    }

    public interface IDecompressor<out T> : IDecompressor {
        new T Decompress(IReadBytes stream);
    }

}
