// <copyright file="CancellationTokenExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMALSharp.Utility
{
    /// <summary>
    /// This class provides extensions for <see cref="CancellationToken"/>s.
    /// </summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task"/> whose state will be set to <see cref="TaskStatus.Canceled"/> when this <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => { tcs.TrySetCanceled(); }, useSynchronizationContext: false);
            return tcs.Task;
        }
    }
}
