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
            var months = new List<MonthStamp>();
            var month = new MonthStamp(1, 1);
            do {
                months.Add(month);
                month = month.AddMonths(1);
            } while (month.Year <= 5000);
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var x in months)
                    writer.WriteCompressedMonthStamp(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                var months2 = new List<MonthStamp>();
                for (var i = 0; i < months.Count; i++)
                    months2.Add(reader.ReadCompressedMonthStamp());
                Assert.IsTrue(months.SequenceEqual(months2));
            }
        }

        [TestMethod]
        public void DateStampCompressor() {
            var days = new List<DateStamp>();
            var day = new DateStamp(1, 1, 1);
            do {
                days.Add(day);
                day = day.AddMonths(1);
            } while (day.Year <= 5000);
            using (var ms = new MemoryStream()) {
                var writer = ms.AsIWriteBytes();
                foreach (var x in days)
                    writer.WriteCompressedDateStamp(x);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = ms.AsIReadBytes();
                var days2 = new List<DateStamp>();
                for (var i = 0; i < days.Count; i++)
                    days2.Add(reader.ReadCompressedDateStamp());
                Assert.IsTrue(days.SequenceEqual(days2));
            }
        }
    }
}
