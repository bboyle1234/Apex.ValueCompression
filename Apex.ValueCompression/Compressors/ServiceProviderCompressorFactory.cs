using System;
using System.Collections.Concurrent;

namespace Apex.ValueCompression.Compressors {

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
            var serviceType = typeof(ICompressor<>).MakeGenericType(type);
            return Compressors.GetOrAdd(type, t => ServiceProvider.GetService(serviceType)) as ICompressor;
        }

        /// <inheritdoc />
        public IDecompressor<T> GetDecompressor<T>() {
            return Decompressors.GetOrAdd(typeof(T), t => ServiceProvider.GetService(typeof(IDecompressor<T>))) as IDecompressor<T>;
        }

        /// <inheritdoc />
        public IDecompressor GetDecompressor(Type type) {
            var serviceType = typeof(IDecompressor<>).MakeGenericType(type);
            return Decompressors.GetOrAdd(type, t => ServiceProvider.GetService(serviceType)) as IDecompressor;
        }
    }
}
