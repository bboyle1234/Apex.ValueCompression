using System;

namespace Apex.ValueCompression.Compressors {
    public static class ICompressorFactoryExtensionMethods {

        /// <summary>
        /// Gets a compressor for the given type.
        /// </summary>
        /// <typeparam name="T">The type to be compressed.</typeparam>
        /// <exception cref="Exception">Thrown if the required compressor cannot be found.</exception>
        public static ICompressor<T> GetRequiredCompressor<T>(this ICompressorFactory factory) {
            var result = factory.GetCompressor<T>();
            if (null == result) CompressorNotFoundException.Throw(typeof(T));
            return result;
        }

        /// <summary>
        /// Gets a compressor for the given type.
        /// </summary>
        /// <param name="type">The type to be compressed.</param>
        /// <exception cref="Exception">Thrown if the required compressor cannot be found.</exception>
        public static ICompressor GetRequiredCompressor(this ICompressorFactory factory, Type type) {
            var result = factory.GetCompressor(type);
            if (null == result) CompressorNotFoundException.Throw(type);
            return result;
        }

        /// <summary>
        /// Gets a decompressor for the given type.
        /// </summary>
        /// <typeparam name="T">The type to be decompressed.</typeparam>
        /// <exception cref="Exception">Thrown if the required decompressor cannot be found.</exception>
        public static IDecompressor<T> GetRequiredDecompressor<T>(this ICompressorFactory factory) {
            var result = factory.GetDecompressor<T>();
            if (null == result) DecompressorNotFoundException.Throw(typeof(T));
            return result;
        }

        /// <summary>
        /// Gets a decompressor for the given type.
        /// </summary>
        /// <param name="type">The type to be decompressed.</param>
        /// <exception cref="Exception">Thrown if the required decompressor cannot be found.</exception>
        public static IDecompressor GetRequiredDecompressor(this ICompressorFactory factory, Type type) {
            var result = factory.GetDecompressor(type);
            if (null == result) DecompressorNotFoundException.Throw(type);
            return result;
        }
    }
}
