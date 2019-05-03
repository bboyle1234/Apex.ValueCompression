using Apex.ValueCompression.Compressors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Apex.ValueCompression.Tests {

    [TestClass]
    public class ICompressorTests {

        class Message {
            public int Value;
        }

        class MessageCompressor : ICompressor<Message>, IDecompressor<Message> {
            public void Compress(Stream stream, Message value)
                => stream.WriteCompressedInt(value.Value);

            public void Compress(Stream stream, object value)
                => Compress(stream, (Message)value);

            public Message Decompress(Stream stream)
                => new Message { Value = stream.ReadCompressedInt() };

            object IDecompressor.Decompress(Stream stream)
                => Decompress(stream);
        }


        [TestMethod]
        public void ABC() {
            var compressor = new MessageCompressor();
            var test1 = (ICompressor)compressor;
            var test2 = (IDecompressor)compressor;
        }
    }
}
