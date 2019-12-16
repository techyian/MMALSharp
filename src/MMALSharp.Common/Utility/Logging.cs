// <copyright file="Logging.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Provides static access to the global logger.
    /// </summary>
    public static class MMALLog
    {
        private static ILoggerFactory _Factory = null;

        /// <summary>
        /// Gets the global logger component.
        /// </summary>
        public static Microsoft.Extensions.Logging.ILogger Logger => MMALLog.LoggerFactory.CreateLogger("MMALSharp");

        /// <summary>
        /// Responsible for instantiating a new Logger Factory.
        /// </summary>
        private static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        /// <summary>
        /// Configures the logger and applies the configuration.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        private static void ConfigureLogger(ILoggerFactory factory)
        {
            factory
                .AddDebug()
                .AddConsole()
                .AddNLog()
                .AddSerilog();
        }        
    }
}
