using Apex.TimeStamps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Apex.ValueCompression.Tests {

    [TestClass]
    public class CompressorTests {

        [TestMethod]
        public void BoolCompressor() {
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                writer.WriteCompressedBool(true);
                writer.WriteCompressedBool(false);
                writer.WriteCompressedNullableBool(true);
                writer.WriteCompressedNullableBool(false);
                writer.WriteCompressedNullableBool(null);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                Assert.IsTrue(reader.ReadCompressedBool());
                Assert.IsFalse(reader.ReadCompressedBool());
                Assert.IsTrue(reader.ReadCompressedNullableBool().Value);
                Assert.IsFalse(reader.ReadCompressedNullableBool().Value);
                Assert.IsNull(reader.ReadCompressedNullableBool());
            }
        }

        [TestMethod]
        public void IntCompressor() {
            var values = new List<int>();
            var value = int.MinValue;
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
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedInt(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedInt());
                }
            }
        }

        [TestMethod]
        public void LongCompressor() {
            var values = new List<long>();
            var value = long.MinValue;
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
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedLong(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedLong());
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
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedUInt(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedUInt());
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
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedULong(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedULong());
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
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteFullDouble(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadFullDouble());
                }
            }
        }

        [TestMethod]
        public void DoubleCompressorOffset() {
            var values = new List<double>();
            var tickSize = 0.01;
            var seed = (double)(5683 * (decimal)tickSize);
            for (var i = -1000000; i < 1000000; i++) {
                values.Add((double)((decimal)seed + (decimal)tickSize * i));
            }

            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var value in values)
                    writer.WriteDoubleOffset(seed, tickSize, value);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadDoubleOffset(seed, tickSize));
                }
            }
        }

        [TestMethod]
        public void StringCompressor() {
            var values = new List<string>();
            values.Add("");
            values.Add("abc");
            values.Add("123");
            values.Add("The quick brown fox jumped over the lazy dog.");
            values.Add(@"!@#$%^&*()_+~?><)""");
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var value in values)
                    writer.WriteCompressedString(value);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedString());
                }
            }
        }


        [TestMethod]
        public void DecimalCompressor() {
            var values = new List<decimal>();
            var value = decimal.MinValue;
            var increment = (decimal.MaxValue / 10000) - (decimal.MinValue / 10000);
            while (true) {
                values.Add(value);
                if (value == decimal.MaxValue) break;
                if (value < decimal.MaxValue - increment) {
                    value += increment;
                } else {
                    value = decimal.MaxValue;
                }
            }

            for (var i = -1001; i < 1001; i++) {
                values.Add(i);
            }

            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedDecimal(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedDecimal());
                }
            }
        }

        [TestMethod]
        public void EnumCompressor() {
            var values = ((ConsoleColor[])Enum.GetValues(typeof(ConsoleColor))).ToList();
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var x in values)
                    writer.WriteCompressedEnum(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                for (var i = 0; i < values.Count; i++) {
                    Assert.AreEqual(values[i], reader.ReadCompressedEnum<ConsoleColor>());
                }
            }
        }

        [TestMethod]
        public void TimeStampCompressor() {
            var t = TimeStamp.Now;
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                writer.WriteCompressedTimeStamp(t);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                var t2 = reader.ReadCompressedTimeStamp();
                Assert.AreEqual(t, t2);
            }
        }

        [TestMethod]
        public void MonthStampCompressor() {
            var now = DateTime.UtcNow;
            var month = new MonthStamp(now.Year, now.Month);
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                writer.WriteCompressedMonthStamp(month);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                var month2 = reader.ReadCompressedMonthStamp();
                Assert.AreEqual(month, month2);
            }
        }

        [TestMethod]
        public void DateStampCompressor() {
            var now = DateTime.UtcNow;
            var date = new DateStamp(now.Year, now.Month, now.Day);
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                writer.WriteCompressedDateStamp(date);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                var date2 = reader.ReadCompressedDateStamp();
                Assert.AreEqual(date, date2);
            }
        }
    }
}
