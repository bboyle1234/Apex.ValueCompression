using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Provides methods for getting compressor and decompressor instances.
    /// </summary>
    public interface ICompressorFactory {

        /// <summary>
        /// Gets a compressor for the given type. Returns null if the compressor cannot be found.
        /// </summary>
        /// <typeparam name="T">The type to be compressed.</typeparam>
        ICompressor<T> GetCompressor<T>();

        /// <summary>
        /// Gets a compressor for the given type.  Returns null if the compressor cannot be found.
        /// </summary>
        /// <param name="type">The type to be compressed.</param>
        ICompressor GetCompressor(Type type);

        /// <summary>
        /// Gets a decompressor for the given type. Returns null if the decompressor cannot be found.
        /// </summary>
        /// <typeparam name="T">The type to be decompressed.</typeparam>
        IDecompressor<T> GetDecompressor<T>();

        /// <summary>
        /// Gets a decompressor for the given type. Returns null if the decompressor cannot be found.
        /// </summary>
        /// <param name="type">The type to be decompressed.</param>
        IDecompressor GetDecompressor(Type type);
    }
}
