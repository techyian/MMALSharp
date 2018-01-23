// <copyright file="IMMALConvert.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public interface IMMALConvert
    {
        Task Convert(int outputPort);
    }
}
