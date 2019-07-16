using System;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents an output port callback handler.
    /// </summary>
    public interface ICallbackHandler
    {
        /// <summary>
        /// Flag to state whether completed status has been sent to observers.
        /// </summary>
        bool Triggered { get; set; }

        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        MMALEncoding EncodingType { get; }
        
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        PortBase WorkingPort { get; }

        /// <summary>
        /// The input callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        ProcessResult InputCallback(MMALBufferImpl buffer);

        /// <summary>
        /// The callback function to carry out. Applies to output, control and connection ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void Callback(MMALBufferImpl buffer);
                
        /// <summary>
        /// Send completed status to observers.
        /// </summary>
        void SendCompleted();

        /// <summary>
        /// Send exception to observers.
        /// </summary>
        void SendError(Exception e);
    }
}