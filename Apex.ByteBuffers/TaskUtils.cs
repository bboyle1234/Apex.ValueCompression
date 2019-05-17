using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.ByteBuffers {
#if NET45
    internal static class TaskUtils {

        public static readonly Task CompletedTask;

        static TaskUtils() {
            var tcs = new TaskCompletionSource<object>();
            CompletedTask = tcs.Task;
            tcs.SetResult(null);
        }
    }
#endif
}
