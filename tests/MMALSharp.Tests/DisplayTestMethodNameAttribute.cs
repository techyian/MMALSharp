// <copyright file="DisplayTestMethodNameAttribute.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Reflection;
using Xunit.Sdk;

namespace MMALSharp.Tests
{
    public class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {        
        public override void Before(MethodInfo methodUnderTest)
        {            
            Console.WriteLine("Running test '{0}.'", methodUnderTest.Name);
        }                
    }
}
