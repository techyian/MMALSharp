
using System;
using MMALSharp.Config;

namespace MMALSharp.Callbacks
{
    public interface IVideoOutputCallbackHandler : ICallbackHandler
    {
        Split Split { get; }
        DateTime? LastSplit { get; }
    }
}
