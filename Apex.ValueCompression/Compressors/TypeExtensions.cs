﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression.Compressors {

    public static class TypeExtensions {

        public static IEnumerable<CompressorTypeData> GetCompressorTypeData(this Assembly assembly)
            => assembly.GetTypes().SelectMany(t => t.GetCompressorTypeData());

        public static IEnumerable<CompressorTypeData> GetDecompressorTypeData(this Assembly assembly)
            => assembly.GetTypes().SelectMany(t => t.GetDecompressorTypeData());

        public static IEnumerable<CompressorTypeData> GetCompressorTypeData(this Type type) {
            if (!type.IsClass) yield break;
            if (type.IsAbstract) yield break;
            foreach (var compressorInterface in type.GetInterfaces().Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(ICompressor<>))) {
                yield return new CompressorTypeData {
                    ServiceType = compressorInterface,
                    ImplementationType = type,
                    CompressedObjectType = compressorInterface.GetGenericArguments()[0],
                };
            }
        }

        public static IEnumerable<CompressorTypeData> GetDecompressorTypeData(this Type type) {
            if (!type.IsClass) yield break;
            if (type.IsAbstract) yield break;
            foreach (var decompressorInterface in type.GetInterfaces().Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IDecompressor<>))) {
                yield return new CompressorTypeData {
                    ServiceType = decompressorInterface,
                    ImplementationType = type,
                    CompressedObjectType = decompressorInterface.GetGenericArguments()[0],
                };
            }
        }

    }
}
