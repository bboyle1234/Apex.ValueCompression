using System;
using System.Collections.Concurrent;

namespace Apex.ValueCompression.Compressors {

    /// <summary>
    /// Inject all your <see cref="ICompressor{T}"/> and <see cref="IDecompressor{T}"/> instances to your service provider,
    /// then inject this class as the singleton <see cref="ICompressorFactory"/> instance to automatically make use of all the compressors
    /// that you've injected.
    /// </summary>
    public class ServiceProviderCompressorFactory : ICompressorFactory {

        readonly IServiceProvider ServiceProvider;
        readonly ConcurrentDictionary<Type, object> Compressors = new ConcurrentDictionary<Type, object>();
        readonly ConcurrentDictionary<Type, object> Decompressors = new ConcurrentDictionary<Type, object>();

        public ServiceProviderCompressorFactory(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public ICompressor<T> GetCompressor<T>() {
            return Compressors.GetOrAdd(typeof(T), t => ServiceProvider.GetService(typeof(ICompressor<T>))) as ICompressor<T>;
        }

        /// <inheritdoc />
        public ICompressor GetCompressor(Type type) {
            return Compressors.GetOrAdd(type, t => ServiceProvider.GetService(typeof(ICompressor<>).MakeGenericType(type))) as ICompressor;
        }

        /// <inheritdoc />
        public IDecompressor<T> GetDecompressor<T>() {
            return Decompressors.GetOrAdd(typeof(T), t => ServiceProvider.GetService(typeof(IDecompressor<T>))) as IDecompressor<T>;
        }

        /// <inheritdoc />
        public IDecompressor GetDecompressor(Type type) {
            return Decompressors.GetOrAdd(type, t => ServiceProvider.GetService(typeof(IDecompressor<>).MakeGenericType(type))) as IDecompressor;
        }
    }
}
