using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ValueCompression {
    public interface IWriteBytes {
        long Position { get; }
        void WriteByte(byte value);
        void WriteBytes(byte[] bytes);
        Task WriteBytesAsync(byte[] bytes);
        void WriteBytes(byte[] bytes, int offset, int count);
        Task WriteBytesAsync(byte[] bytes, int offset, int count);
    }
}
