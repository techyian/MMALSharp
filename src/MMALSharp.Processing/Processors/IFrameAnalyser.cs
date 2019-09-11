// <copyright file="IFrameAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors
{
    /// <summary>
    /// Represents a frame analyser.
    /// </summary>
    public interface IFrameAnalyser
    {
        /// <summary>
        /// The operation to perform analysis.
        /// </summary>
        /// <param name="data">The working data.</param>
        /// <param name="eos">Signals end of stream.</param>
        void Apply(byte[] data, bool eos);
    }
}
