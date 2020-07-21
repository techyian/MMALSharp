// <copyright file="Parameter.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp
{
    internal class Parameter
    {
        public Parameter(int paramVal, Type paramType, string paramName)
        {
            this.ParamValue = paramVal;
            this.ParamType = paramType;
            this.ParamName = paramName;
        }

        public int ParamValue { get; set; }

        public Type ParamType { get; set; }

        public string ParamName { get; set; }
    }
}
