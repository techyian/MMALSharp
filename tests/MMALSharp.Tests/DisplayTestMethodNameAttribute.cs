// <copyright file="DisplayTestMethodNameAttribute.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Reflection;
using Xunit.Sdk;

namespace MMALSharp.Tests
{
    public class MMALTestsAttribute : BeforeAfterTestAttribute
    {        
        public override void Before(MethodInfo methodUnderTest)
        {
            TestHelper.SetConfigurationDefaults();
            Console.WriteLine("Running test '{0}.'", methodUnderTest.Name);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            TestHelper.SetConfigurationDefaults();
            base.After(methodUnderTest);
        }
    }
}
