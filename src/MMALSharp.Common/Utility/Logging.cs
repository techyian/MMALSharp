// <copyright file="Logging.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using NLog;
using NLog.Config;
using NLog.Targets;

namespace MMALSharp
{
    public static class MMALLog
    {
        public static string LogLocation { get; set; }

        public static Logger Logger { get; set; }

        public static void ConfigureLogConfig()
        {
            var debugLayout = @"${longdate}|${event-properties:item=EventId.Id}|${uppercase:${level}}|${logger}|${message} ${exception}";
            var consoleLayout = @"${message} ${exception}";

            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = consoleLayout;
                        
            if (!string.IsNullOrEmpty(MMALLog.LogLocation))
            {
                fileTarget.FileName = MMALLog.LogLocation;
            }
            else
            {
                fileTarget.FileName = "${basedir}/mmal-log-${shortdate}.log";
            }
            
            fileTarget.Layout = debugLayout;

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Info, LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
