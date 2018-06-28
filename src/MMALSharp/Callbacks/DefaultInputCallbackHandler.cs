// <copyright file="DefaultInputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Input ports.
    /// </summary>
    public class DefaultInputCallbackHandler : InputCallbackHandlerBase
    {
        public DefaultInputCallbackHandler(MMALPortBase port) 
            : base(port)
        {
        }

        public DefaultInputCallbackHandler(MMALPortBase port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }
    }
}
