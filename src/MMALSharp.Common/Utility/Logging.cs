using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public static class MMALLog
    {
        public static Logger Logger { get; set; }

        public static void ConfigureLogConfig()
        {
            var layout = @"${longdate}|${event-properties:item=EventId.Id}|${uppercase:${level}}|${logger}|${message} ${exception}";

            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = layout;
            fileTarget.FileName = "mmal-log-${shortdate}.log";
            fileTarget.Layout = layout;

            // Step 4. Define rules
            var rule1 = new LoggingRule("DEBUG", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("CONSOLE", LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
