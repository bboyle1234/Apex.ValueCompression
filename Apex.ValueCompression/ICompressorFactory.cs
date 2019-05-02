using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apex.ValueCompression {

    public interface ICompressorFactory {
        ICompressor<T> GetCompressor<T>();
        ICompressor GetCompressor(Type type);
        IDecompressor<T> GetDecompressor<T>();
        IDecompressor GetDecompressor(Type type);
    }

    public static class ICompressorFactoryExtensionMethods {

        public static ICompressor<T> GetRequiredCompressor<T>(this ICompressorFactory factory) {
            var result = factory.GetCompressor<T>();
            if (null == result) throw new Exception($"Could not find compressor for type '{typeof(T)}'.");
            return result;
        }

        public static ICompressor GetRequiredCompressor(this ICompressorFactory factory, Type type) {
            var result = factory.GetCompressor(type);
            if (null == result) throw new Exception($"Could not find compressor for type '{type}'.");
            return result;
        }

        public static IDecompressor<T> GetRequiredDecompressor<T>(this ICompressorFactory factory) {
            var result = factory.GetDecompressor<T>();
            if (null == result) throw new Exception($"Could not find decompressor for type '{typeof(T)}'.");
            return result;
        }

        public static IDecompressor GetRequiredDecompressor(this ICompressorFactory factory, Type type) {
            var result = factory.GetDecompressor(type);
            if (null == result) throw new Exception($"Could not find decompressor for type '{type}'.");
            return result;
        }
    }

    public class CompressorFactory : ICompressorFactory {

        readonly Dictionary<Type, object> Compressors = new Dictionary<Type, object>();
        readonly Dictionary<Type, object> Decompressors = new Dictionary<Type, object>();


        public void Add<T>(ICompressor<T> compressor) {
            Compressors[typeof(T)] = compressor;
        }

        public void Add<T>(IDecompressor<T> decompressor) {
            Decompressors[typeof(T)] = decompressor;
        }

        public void AddFromAssembly(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                if (type.IsClass && !type.IsAbstract) {
                    try { Add(type); } catch { }
                }
            }
        }

        public void Add(Type type, object instance = null) {

            if (null != instance && instance.GetType() != type)
                throw new Exception("type mismatch.");

            var compressorInterfaces = type.GetInterfaces()
                .Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(ICompressor<>))
                .ToList();

            var decompressorInterfaces = type.GetInterfaces()
                .Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IDecompressor<>))
                .ToList();

            if (compressorInterfaces.Count == 0 && decompressorInterfaces.Count == 0)
                throw new Exception($"Unable to find interfaces for type '{type}'.");

            if (null == instance)
                instance = Activator.CreateInstance(type);

            foreach (var i in compressorInterfaces) {
                var compressionType = i.GetGenericArguments().First();
                Compressors[compressionType] = instance;
            }

            foreach (var i in decompressorInterfaces) {
                var compressionType = i.GetGenericArguments().First();
                Decompressors[compressionType] = instance;
            }
        }

        public ICompressor<T> GetCompressor<T>() {
            Compressors.TryGetValue(typeof(T), out var compressor);
            return compressor as ICompressor<T>;
        }

        public ICompressor GetCompressor(Type type) {
            Compressors.TryGetValue(type, out var compressor);
            return compressor as ICompressor;
        }

        public IDecompressor<T> GetDecompressor<T>() {
            Decompressors.TryGetValue(typeof(T), out var decompressor);
            return decompressor as IDecompressor<T>;
        }

        public IDecompressor GetDecompressor(Type type) {
            Decompressors.TryGetValue(type, out var decompressor);
            return decompressor as IDecompressor;
        }
    }

    public class ServiceProviderCompressorFactory : ICompressorFactory {

        readonly IServiceProvider ServiceProvider;
        readonly CompressorFactory Factory;

        public ServiceProviderCompressorFactory(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
            Factory = new CompressorFactory();
        }

        public ICompressor<T> GetCompressor<T>() {
            var result = Factory.GetCompressor<T>();
            if (null == result) {
                var serviceType = typeof(ICompressor<T>);
                result = ServiceProvider.GetService(serviceType) as ICompressor<T>;
                if (null == result) throw new Exception($"Unable to find compressor for type '{typeof(T)}'.");
                Factory.Add(result.GetType(), result);
            }
            return result;
        }

        public ICompressor GetCompressor(Type type) {
            var result = Factory.GetCompressor(type);
            if (null == result) {
                var serviceType = typeof(ICompressor<>).MakeGenericType(type);
                result = ServiceProvider.GetService(serviceType) as ICompressor;
                if (null == result) throw new Exception($"Unable to find compressor for type '{type}'.");
                Factory.Add(result.GetType(), result);
            }
            return result;
        }

        public IDecompressor<T> GetDecompressor<T>() {
            var result = Factory.GetDecompressor<T>();
            if (null == result) {
                var serviceType = typeof(IDecompressor<T>);
                result = ServiceProvider.GetService(serviceType) as IDecompressor<T>;
                if (null == result) throw new Exception($"Unable to find decompressor for type '{typeof(T)}'.");
                Factory.Add(result.GetType(), result);
            }
            return result;
        }

        public IDecompressor GetDecompressor(Type type) {
            var result = Factory.GetDecompressor(type);
            if (null == result) {
                var serviceType = typeof(IDecompressor<>).MakeGenericType(type);
                result = ServiceProvider.GetService(serviceType) as IDecompressor;
                if (null == result) throw new Exception($"Unable to find decompressor for type '{type}'.");
                Factory.Add(result.GetType(), result);
            }
            return result;
        }
    }
}
