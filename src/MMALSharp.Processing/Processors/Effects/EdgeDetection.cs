// <copyright file="EdgeDetection.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public enum EDStrength
    {
        Low,
        Medium,
        High
    }
    
    /// <summary>
    /// A kernel based image processor used to apply Edge detection convolution.
    /// </summary>
    public class EdgeDetection : ConvolutionBase, IFrameProcessor
    {
        private const int KernelWidth = 3;
        private const int KernelHeight = 3;
        
        private double[,] Kernel { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="EdgeDetection"/>.
        /// </summary>
        /// <param name="strength">The Edge detection strength.</param>
        public EdgeDetection(EDStrength strength)
        {
            switch (strength)
            {
                case EDStrength.Low:
                    Kernel = new double[KernelWidth, KernelHeight]
                    {
                        { -1, 0, 1 },
                        { 0,  0, 0 },
                        { 1, 0, -1 }
                    };
                    break;
                case EDStrength.Medium:
                    Kernel = new double[KernelWidth, KernelHeight]
                    {
                        { 0, 1, 0 },
                        { 1, -4, 1 },
                        { 0, 1, 0 }
                    };
                    break;
                case EDStrength.High:
                    Kernel = new double[KernelWidth, KernelHeight]
                    {
                        { -1, -1, -1 },
                        { -1, 8, -1 },
                        { -1, -1, -1 }
                    };
                    break;
            }
        }

        /// <inheritdoc />
        public void Apply(IImageContext context)
        {
            this.Convolute(context.Data, this.Kernel, KernelWidth, KernelHeight, context);
        }
    }
}