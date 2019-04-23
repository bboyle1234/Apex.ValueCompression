using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apex.ValueCompression.Tests {

    [TestClass]
    public class ReflectionTests {

        [TestMethod]
        public void Test1() {
            var types = GetCompressorTypes(Assembly.GetExecutingAssembly());
        }

        Dictionary<Type, Type> GetCompressorTypes(Assembly assembly) {
            var result = new Dictionary<Type, Type>();
            foreach(var type in assembly.GetTypes()) {
                if (type.IsClass && !type.IsAbstract) {
                    var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ICompressor<>));
                    if (null != interfaceType) {
                        result[interfaceType] = type;
                    }
                }
            }
            return result;
        }
    }

    class A : ICompressor<int> {
        public void Compress(Stream stream, int value) => throw new NotImplementedException();
        public int Decompress(Stream stream) => throw new NotImplementedException();
    }
}
