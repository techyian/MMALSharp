// <copyright file="Logging.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using System;

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Provides static access to the global logger.
    /// Credit to @snakefoot - https://github.com/NLog/NLog.Extensions.Logging/issues/379#issuecomment-569544196
    /// </summary>
    public static class MMALLog
    {
        /// <summary>
        /// Gets the global logger component.
        /// </summary>
        public static ILogger Logger => _logger;
        private static readonly MMALLogger _logger = new MMALLogger();

        /// <summary>
        /// Responsible for getting/setting the working LoggerFactory.
        /// </summary>
        public static ILoggerFactory LoggerFactory
        {
            get => _logger.LoggerFactory;
            set => _logger.LoggerFactory = value;
        }

        private class MMALLogger : ILogger
        {
            public ILoggerFactory LoggerFactory
            {
                get { return _loggerFactory; }
                set
                {
                    _loggerFactory = value;
                    _logger = null;
                }
            }

            private ILoggerFactory _loggerFactory;
            private ILogger _logger;
            private ILogger Logger => _logger ?? (_logger = _loggerFactory?.CreateLogger("MMALSharp"));

            IDisposable ILogger.BeginScope<TState>(TState state)
            {
                return Logger?.BeginScope(state);
            }

            bool ILogger.IsEnabled(LogLevel logLevel)
            {
                return Logger?.IsEnabled(logLevel) ?? false;
            }

            void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Logger?.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}