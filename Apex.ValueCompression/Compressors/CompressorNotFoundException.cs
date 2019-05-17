using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression.Compressors {

    [Serializable]
    public sealed class CompressorNotFoundException : Exception {

        public static void Throw(Type type) => throw new CompressorNotFoundException($"Could not find compressor for type '{type}'.");

        public CompressorNotFoundException() { }
        public CompressorNotFoundException(string message) : base(message) { }
        public CompressorNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected CompressorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
