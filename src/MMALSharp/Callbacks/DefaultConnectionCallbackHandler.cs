// <copyright file="DefaultConnectionCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Callbacks
{
    public class DefaultConnectionCallbackHandler : ConnectionCallbackHandlerBase
    {
        public DefaultConnectionCallbackHandler(MMALConnectionImpl connection) 
            : base(connection)
        {
        }
    }
}
