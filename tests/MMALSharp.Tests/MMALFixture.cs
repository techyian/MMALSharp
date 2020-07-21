// <copyright file="MMALFixture.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using NLog.Extensions.Logging;
using Xunit;

namespace MMALSharp.Tests
{
    public class MMALFixture : IDisposable
    {
        public MMALStandalone MMALStandalone = MMALStandalone.Instance;
        public MMALCamera MMALCamera = MMALCamera.Instance;

        public MMALFixture()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddNLog("NLog.config");
            });

            MMALLog.LoggerFactory = loggerFactory;
        }

        public void CheckAndAssertFilepath(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                var length = new System.IO.FileInfo(filepath).Length;
                Assert.True(length > 0, $"File {filepath} has 0 bytes.");
            }
            else
            {
                Assert.True(false, $"File {filepath} was not created");
            }
        }

        public void CheckAndAssertDirectory(string directory)
        {
            DirectoryInfo info = new DirectoryInfo(directory);

            if (info.Exists)
            {
                var files = info.EnumerateFiles();

                Assert.True(files != null && files.Any());
            }
            else
            {
                Assert.True(false, $"Directory {directory} was not created");
            }
        }

        public void Dispose()
        {
            this.MMALCamera.Cleanup();
            this.MMALStandalone.Cleanup();
        }
    }

    [CollectionDefinition("MMALCollection")]
    public class MMALTestCollection : ICollectionFixture<MMALFixture>
    {
    }
}
