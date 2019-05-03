using System;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Used for obtaining information about a compressor or decompressor type.
    /// </summary>
    public class CompressorTypeData {

        /// <summary>
        /// The type of the class that implements the compressor or decompressor. You can create instances of this class to perform the actual work.
        /// </summary>
        public Type ImplementationType;

        /// <summary>
        /// Would be either <see cref="ICompressor{T}"/> or <see cref="IDecompressor{T}"/>.
        /// </summary>
        public Type ServiceType;

        /// <summary>
        /// The type of the object to be compressed or decompressed.
        /// </summary>
        public Type CompressedObjectType;
    }
}
