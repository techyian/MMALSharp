// <copyright file="MMALFixture.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using Xunit;

namespace MMALSharp.Tests
{
    public class MMALFixture : IDisposable
    {
        public MMALCamera MMALCamera = MMALCamera.Instance;

        public MMALFixture()
        {
        }

        public void CheckAndAssertFilepath(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                var length = new System.IO.FileInfo(filepath).Length;
                Assert.True(length > 0);
            }
            else
            {
                Assert.True(false, $"File {filepath} was not created");
            }
        }

        public void Dispose() => this.MMALCamera.Cleanup();
    }

    [CollectionDefinition("MMALCollection")]
    public class MmalCollection : ICollectionFixture<MMALFixture>
    {
    }
}
