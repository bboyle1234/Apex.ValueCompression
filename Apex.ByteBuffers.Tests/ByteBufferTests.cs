using Apex.ValueCompression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using static System.Math;

namespace Apex.ByteBuffers.Tests {

    [TestClass]
    public class ByteBufferTests {

        static byte[] InputBytes;

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            var rand = new Random();
            InputBytes = new byte[1000000];
            for (var i = 0; i < 1000000; i++) {
                InputBytes[i] = (byte)rand.Next(255);
            }
        }

        [TestMethod]
        public void WriteByteReadByte() {
            var buffer = new ByteBuffer(16);
            for (var i = 0; i < InputBytes.Length; i++) {
                buffer.WriteByte(InputBytes[i]);
            }
            for (var i = 0; i < InputBytes.Length; i++) {
                Assert.AreEqual(buffer.ReadByte(i), InputBytes[i]);
            }
        }

        [TestMethod]
        public void WriteBytesReadBytes() {
            var buffer = new ByteBuffer(16);
            buffer.WriteBytes(InputBytes);
            for (var i = 0; i < InputBytes.Length; i += 4) {
                var bytes = buffer.ReadBytes(i, 4);
                Assert.IsTrue(Enumerable.SequenceEqual(bytes, InputBytes.Skip(i).Take(4)));
            }
            for (var i = 0; i < InputBytes.Length; i += 5) {
                var bytes = buffer.ReadBytes(i, 5);
                Assert.IsTrue(Enumerable.SequenceEqual(bytes, InputBytes.Skip(i).Take(5)));
            }
            /// 23 doesn't divide into 1000000, and it's bigger than the buffer size of 16,
            /// so it's a good read size to use for testing "hairy" reads.
            for (var i = 0; i < InputBytes.Length; i += 23) {
                var numBytesToRead = Min(23, InputBytes.Length - i);
                var bytes = buffer.ReadBytes(i, numBytesToRead);
                Assert.IsTrue(Enumerable.SequenceEqual(bytes, InputBytes.Skip(i).Take(numBytesToRead)));
            }
        }

        [TestMethod]
        public void Readers() {
            var rand = new Random();
            var buffer = new ByteBuffer(16);
            buffer.WriteBytes(InputBytes);

            var count = rand.Next(21000);
            var offset = rand.Next(InputBytes.Length - count);
            var destination = new byte[count];
            buffer.CreateReader(offset).ReadBytes(destination, 0, count);
            Assert.IsTrue(Enumerable.SequenceEqual(InputBytes.Skip(offset).Take(count), destination));

            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                buffer.CreateReader().CopyTo(writer);
                Assert.IsTrue(Enumerable.SequenceEqual(InputBytes, ms.ToArray()));
            }
        }
    }
}
