using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    [Serializable]
    public sealed class EndOfStreamException : Exception {

        public static void ThrowRead() => throw new EndOfStreamException("Tried to read past the end of the stream.");
        public static void ThrowWrite() => throw new EndOfStreamException("Tried to write past the end of the stream.");

        public EndOfStreamException() { }
        public EndOfStreamException(string message) : base(message) { }
        public EndOfStreamException(string message, Exception inner) : base(message, inner) { }
        EndOfStreamException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
