// <copyright file="CustomConnectionCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;

namespace MMALSharp.Tests.Data
{
    public class CustomConnectionCallbackHandler : ConnectionCallbackHandler
    {
        public CustomConnectionCallbackHandler(IConnection connection) 
            : base(connection)
        {
        }

        public override void InputCallback(IBuffer buffer)
        {
            base.InputCallback(buffer);

            MMALLog.Logger.LogInformation("In the custom input callback handler.");
        }
    }
}
