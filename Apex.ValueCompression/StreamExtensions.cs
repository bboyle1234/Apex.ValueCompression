using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {

    public static class StreamExtensions {

        public static IReadBytes AsIReadBytes(this Stream stream)
            => new StreamReadWrapper(stream);

        public static IWriteBytes AsIWriteBytes(this Stream stream)
            => new StreamWriteWrapper(stream);
    }
}
