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

        public void Dispose()
        {
            this.MMALCamera.Cleanup();
        }
    }

    [CollectionDefinition("MMALCollection")]
    public class MMALCollection : ICollectionFixture<MMALFixture>
    {

    }
}
