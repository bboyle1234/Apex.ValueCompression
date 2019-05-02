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

                    var compressorInterfaces = type.GetInterfaces()
                        .Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(ICompressor<>))
                        .ToList();

                    if (compressorInterfaces.Count > 0) {
                        var instance = Activator.CreateInstance(type);
                        foreach (var i in compressorInterfaces) {
                            var compressionType = i.GetGenericArguments().First();
                            Compressors[compressionType] = instance;
                        }
                    }


                    var decompressorInterfaces = type.GetInterfaces()
                        .Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IDecompressor<>))
                        .ToList();

                    if (decompressorInterfaces.Count > 0) {
                        var instance = Activator.CreateInstance(type);
                        foreach (var i in decompressorInterfaces) {
                            var compressionType = i.GetGenericArguments().First();
                            Decompressors[compressionType] = instance;
                        }
                    }
                }
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
}
