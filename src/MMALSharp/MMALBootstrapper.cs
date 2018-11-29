using System.Collections.Generic;
using MMALSharp.Components;

namespace MMALSharp
{
    public static class MMALBootstrapper
    {
        /// <summary>
        /// List of all encoders currently in the pipeline.
        /// </summary>
        public static List<MMALDownstreamComponent> DownstreamComponents { get; } = new List<MMALDownstreamComponent>();
    }
}