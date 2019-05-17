using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression.Compressors {

    [Serializable]
    public sealed class DecompressorNotFoundException : Exception {

        public static void Throw(Type type) => throw new DecompressorNotFoundException($"Could not find decompressor for type '{type}'.");

        public DecompressorNotFoundException() { }
        public DecompressorNotFoundException(string message) : base(message) { }
        public DecompressorNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DecompressorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
