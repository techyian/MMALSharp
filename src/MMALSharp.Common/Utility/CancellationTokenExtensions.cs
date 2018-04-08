using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMALSharp.Utility
{
    public static class CancellationTokenExtensions
    {
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => { tcs.TrySetCanceled(); }, useSynchronizationContext: false);
            return tcs.Task;
        }
    }
}
