using Apex.ValueCompression;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Math;

namespace Apex.ByteBuffers {

    /// <summary>
    /// Use this class for either of two purposes:
    /// 1. Your byte array needs to be greater than the usual maximum (int) size of an <see cref="Array"/>.
    /// 2. You need a stream-like object that you can write to from a single process 
    /// but can spawn many independent readers for concurrent read access.
    /// </summary>
    public sealed class ByteBuffer : IWriteBytes {

        readonly List<byte[]> _buffers;
        public readonly int BufferSize;
        public readonly long BufferSizeAsLong;

        int _writeIndex = 0;
        byte[] _writeBuffer = null;

        public ByteBuffer(int bufferSize) {
            _buffers = new List<byte[]>();
            BufferSize = bufferSize;
            BufferSizeAsLong = bufferSize;
            StartNewBuffer();
        }

        void StartNewBuffer() {
            _writeBuffer = new byte[BufferSize];
            _buffers.Add(_writeBuffer);
            _writeIndex = 0;
        }

        public long Position => Length;

        // Used only as a public property - not used in the processing of reading/writing.
        // Is explicitly maintained in code so that the frequent calls to this property are fast.
        // (Old version of code calculated the result of this property for every call to it)
        public long Length { get; private set; }

        public void WriteByte(byte b) {
            if (_writeIndex == BufferSize)
                StartNewBuffer();
            _writeBuffer[_writeIndex] = b;
            _writeIndex++;
            Length++;
        }

        public void WriteBytes(byte[] bytes) {
            WriteBytes(bytes, 0, bytes.Length);
        }

        public Task WriteBytesAsync(byte[] bytes) {
            WriteBytes(bytes, 0, bytes.Length);
#if NET45
            return TaskUtils.CompletedTask;
#else
            return Task.CompletedTask;
#endif
        }

        public void WriteBytes(byte[] bytes, int start, int count) {
            var readIndex = start;
            var bytesRemainingToWrite = count;
            while (true) {
                var spaceLeftInCurrentBuffer = BufferSize - _writeIndex;
                if (spaceLeftInCurrentBuffer >= bytesRemainingToWrite) {
                    Buffer.BlockCopy(bytes, readIndex, _writeBuffer, _writeIndex, bytesRemainingToWrite);
                    _writeIndex += bytesRemainingToWrite;
                    Length += count;
                    return;
                } else {
                    Buffer.BlockCopy(bytes, readIndex, _writeBuffer, _writeIndex, spaceLeftInCurrentBuffer);
                    readIndex += spaceLeftInCurrentBuffer;
                    bytesRemainingToWrite -= spaceLeftInCurrentBuffer;
                    StartNewBuffer();
                }
            }
        }

        public Task WriteBytesAsync(byte[] bytes, int start, int count) {
            WriteBytes(bytes, start, count);
#if NET45
            return TaskUtils.CompletedTask;
#else
            return Task.CompletedTask;
#endif
        }

        public byte ReadByte(long index) {
            var bufferIndex = (int)(index / BufferSizeAsLong);
            var byteIndex = (int)(index % BufferSizeAsLong);
            return _buffers[bufferIndex][byteIndex];
        }

        public byte[] ReadBytes(long index, int count) {
            var bytes = new byte[count];
            ReadBytes(index, count, bytes, 0);
            return bytes;
        }

        public void ReadBytes(long index, int count, byte[] destination, int destinationOffset) {
            while (count > 0) {
                var readBuffer = _buffers[(int)(index / BufferSizeAsLong)];
                var readIndex = (int)(index % BufferSizeAsLong);
                var numBytes = Min(count, BufferSize - readIndex);
                Buffer.BlockCopy(readBuffer, readIndex, destination, destinationOffset, numBytes);
                index += numBytes;
                destinationOffset += numBytes;
                count -= numBytes;
            }
        }

        public void ReadBytes(long index, int count, IWriteBytes destination) {
            while (count > 0) {
                var readBuffer = _buffers[(int)(index / BufferSizeAsLong)];
                var readIndex = (int)(index % BufferSizeAsLong);
                var numBytes = Min(count, BufferSize - readIndex);
                destination.WriteBytes(readBuffer, readIndex, numBytes);
                index += numBytes;
                count -= numBytes;
            }
        }

        public IReadBytes CreateReader() {
            return new Reader(this);
        }

        public IReadBytes CreateReader(long startIndex) {
            var reader = new Reader(this);
            reader.GoToIndex(startIndex);
            return reader;
        }

        class Reader : IReadBytes {

            readonly ByteBuffer _parent;

            public long Position { get; private set; }

            public long BytesRemaining
                => _parent.Length - Position;

            public long Length
                => _parent.Length;

            internal Reader(ByteBuffer parent) {
                _parent = parent;
            }

            internal void GoToIndex(long index) {
                Position = index;
            }

            public byte ReadByte()
                => _parent.ReadByte(Position++);

            public void ReadBytes(byte[] buffer, int offset, int count) {
                _parent.ReadBytes(Position, count, buffer, offset);
                Position += count;
            }

            public Task ReadBytesAsync(byte[] buffer, int offset, int count) {
                ReadBytes(buffer, offset, count);
#if NET45
                return TaskUtils.CompletedTask;
#else
                return Task.CompletedTask;
#endif
            }

            public void CopyTo(IWriteBytes destination, int bufferSize = 81920) {
                var numBytes = (int)Min((long)int.MaxValue, BytesRemaining);
                while (numBytes > 0) {
                    _parent.ReadBytes(Position, numBytes, destination);
                    Position += numBytes;
                    numBytes = (int)Min((long)int.MaxValue, BytesRemaining);
                }
            }

            public Task CopyToAsync(IWriteBytes destination, int bufferSize = 81920) {
                CopyTo(destination, bufferSize);
#if NET45
                return TaskUtils.CompletedTask;
#else
                return Task.CompletedTask;
#endif
            }
        }
    }
}
