using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Apex.ValueCompression.Tests {

    [TestClass]
    public class CompressorTests {

        [TestMethod]
        public void IntCompressor() {
            var values = new List<int>();
            var value = int.MinValue + 1;
            var increment = (int.MaxValue / 10000) - (int.MinValue / 10000);
            while (true) {
                values.Add(value);
                if (value == int.MaxValue) break;
                if (value < int.MaxValue - increment) {
                    value += increment;
                } else {
                    value = int.MaxValue;
                }
            }

            for (var i = -1001; i < 1001; i++) {
                values.Add(i);
            }

            using (var ms = new MemoryStream()) {
                foreach (var x in values)
                    ms.WriteCompressedInt(x);
                ms.Seek(0, SeekOrigin.Begin);
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadCompressedInt());
                }
            }
        }

        [TestMethod]
        public void LongCompressor() {
            var values = new List<long>();
            var value = long.MinValue + 1;
            var increment = (long.MaxValue / 10000) - (long.MinValue / 10000);
            while (true) {
                values.Add(value);
                if (value == long.MaxValue) break;
                if (value < long.MaxValue - increment) {
                    value += increment;
                } else {
                    value = long.MaxValue;
                }
            }

            for (var i = -1001L; i < 1001L; i++) {
                values.Add(i);
            }

            using (var ms = new MemoryStream()) {
                foreach (var x in values)
                    ms.WriteCompressedLong(x);
                ms.Seek(0, SeekOrigin.Begin);
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadCompressedLong());
                }
            }
        }

        [TestMethod]
        public void UIntCompressor() {
            var values = new List<uint>();
            var value = uint.MinValue;
            var increment = (uint.MaxValue - uint.MinValue) / 10000;
            while (true) {
                values.Add(value);
                if (value == uint.MaxValue) break;
                if (value < uint.MaxValue - increment) {
                    value += increment;
                } else {
                    value = uint.MaxValue;
                }
            }

            for (var i = 1U; i < 1001U; i++) {
                values.Add(i);
            }

            using (var ms = new MemoryStream()) {
                foreach (var x in values)
                    ms.WriteCompressedUInt(x);
                ms.Seek(0, SeekOrigin.Begin);
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadCompressedUInt());
                }
            }
        }

        [TestMethod]
        public void ULongCompressor() {
            var values = new List<ulong>();
            var value = ulong.MinValue;
            var increment = (ulong.MaxValue - ulong.MinValue) / 10000;
            while (true) {
                values.Add(value);
                if (value == ulong.MaxValue) break;
                if (value < ulong.MaxValue - increment) {
                    value += increment;
                } else {
                    value = ulong.MaxValue;
                }
            }

            for (var i = 1UL; i < 1001UL; i++) {
                values.Add(i);
            }

            using (var ms = new MemoryStream()) {
                foreach (var x in values)
                    ms.WriteCompressedULong(x);
                ms.Seek(0, SeekOrigin.Begin);
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadCompressedULong());
                }
            }
        }

        [TestMethod]
        public void DoubleCompressorFull() {
            var values = new List<double>();
            var value = double.MinValue;
            var increment = (double.MaxValue / 10000) - (double.MinValue / 10000);
            while (true) {
                values.Add(value);
                if (value == double.MaxValue) break;
                if (value < double.MaxValue - increment) {
                    value += increment;
                } else {
                    value = double.MaxValue;
                }
            }
            using (var ms = new MemoryStream()) {
                foreach (var x in values)
                    ms.WriteFullDouble(x);
                ms.Seek(0, SeekOrigin.Begin);
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadFullDouble());
                }
            }
        }

        [TestMethod]
        public void DoubleCompressorOffset() {
            var values = new List<double>();
            var tickSize = 0.1234;
            var seed = (double)(5683 * (decimal)tickSize);
            for (var i = -100; i < 100; i++) {
                values.Add((double)((decimal)seed + (decimal)tickSize * i));
            }

            using (var ms = new MemoryStream()) {
                foreach (var value in values)
                    ms.WriteDoubleOffset(seed, tickSize, value);
                ms.Seek(0, SeekOrigin.Begin);
                for(var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], ms.ReadDoubleOffset(seed, tickSize));
                }
            }
        }
    }
}
