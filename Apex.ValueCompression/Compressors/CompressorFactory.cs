using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Use this class to store compressors, or build your own <see cref="ICompressorFactory"/>
    /// </summary>
    public class CompressorFactory : ICompressorFactory {

        readonly Dictionary<Type, object> Compressors = new Dictionary<Type, object>();
        readonly Dictionary<Type, object> Decompressors = new Dictionary<Type, object>();

        /// <summary>
        /// Adds the given compressor to the store.
        /// </summary>
        public void Add<T>(ICompressor<T> compressor) {
            Compressors[typeof(T)] = compressor;
        }

        /// <summary>
        /// Adds the given decompressor to the store.
        /// </summary>
        public void Add<T>(IDecompressor<T> decompressor) {
            Decompressors[typeof(T)] = decompressor;
        }

        /// <summary>
        /// Adds all compressors and decompressors found in the given assembly.
        /// The assembly is searched for types inheriting <see cref="ICompressor{T}"/> and <see cref="IDecompressor{T}"/>
        /// </summary>
        public void AddFromAssembly(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                var compressorInfo = type.GetCompressorTypeData().ToArray();
                var decompressorInfo = type.GetDecompressorTypeData().ToArray();
                if (compressorInfo.Length > 0 || decompressorInfo.Length > 0) {
                    var instance = Activator.CreateInstance(type);
                    foreach(var x in compressorInfo) 
                        Compressors[x.CompressedObjectType] = instance;
                    foreach (var x in decompressorInfo)
                        Decompressors[x.CompressedObjectType] = instance;
                }
            }
        }

        /// <inheritdoc />
        public virtual ICompressor<T> GetCompressor<T>() {
            return GetCompressor(typeof(T)) as ICompressor<T>;
        }

        /// <inheritdoc />
        public virtual ICompressor GetCompressor(Type type) {
            Compressors.TryGetValue(type, out var compressor);
            return compressor as ICompressor;
        }

        /// <inheritdoc />
        public virtual IDecompressor<T> GetDecompressor<T>() {
            return GetDecompressor(typeof(T)) as IDecompressor<T>;
        }

        /// <inheritdoc />
        public virtual IDecompressor GetDecompressor(Type type) {
            Decompressors.TryGetValue(type, out var decompressor);
            return decompressor as IDecompressor;
        }
    }
}
