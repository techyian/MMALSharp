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
        public static double[,] LowStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { -1, 0, 1 },
            { 0, 0, 0 },
            { 1, 0, -1 }
        };

        public static double[,] MediumStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { 0, 1, 0 },
            { 1, -4, 1 },
            { 0, 1, 0 }
        };

        public static double[,] HighStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
        };

        public double[,] Kernel { get; }

        public const int KernelWidth = 3;
        public const int KernelHeight = 3;
        
        /// <summary>
        /// Creates a new instance of <see cref="EdgeDetection"/> processor used to apply Edge detection convolution.
        /// </summary>
        /// <param name="strength">The Edge detection strength.</param>
        public EdgeDetection(EDStrength strength)
        {
            switch (strength)
            {
                case EDStrength.Low:
                    Kernel = LowStrengthKernel;
                    break;
                case EDStrength.Medium:
                    Kernel = MediumStrengthKernel;
                    break;
                case EDStrength.High:
                    Kernel = HighStrengthKernel;
                    break;
            }
        }

        /// <inheritdoc />
        public void Apply(IImageContext context)
        {
            this.ApplyConvolution(this.Kernel, KernelWidth, KernelHeight, context);
        }
    }
}