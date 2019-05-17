using Apex.TimeStamps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Apex.ValueCompression.Tests {

    [TestClass]
    public class AllocationTests {

        [TestMethod]
        public void BufferAllocation() {
            var pool = System.Buffers.ArrayPool<byte>.Create();

            Time("Using Buffer Pool", () => {
                var bufferList = new List<byte[]>(100);
                for (var i = 0; i < 100000; i++) {
                    bufferList.Add(pool.Rent(4));
                    if (i % 100 == 0) {
                        for (var j = 0; j < bufferList.Count; j++) {
                            pool.Return(bufferList[j]);
                        }
                        bufferList.Clear();
                    }
                }
            });

            Time("Using allocation", () => {
                var bufferList = new List<byte[]>(100);
                for (var i = 0; i < 100000; i++) {
                    bufferList.Add(new byte[4]);
                    if (i % 100 == 0) {
                        bufferList.Clear();
                    }
                }
            });
        }

        void Time(string name, Action action) {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            Debug.WriteLine($"{name}: {(int)sw.ElapsedMilliseconds}ms");
        }
    }
}
