// <copyright file="EdgeDetection.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// Represents the strength of the Edge Detection algorithm.
    /// </summary>
    public enum EDStrength
    {
        /// <summary>
        /// Low strength.
        /// </summary>
        Low,

        /// <summary>
        /// Medium strength.
        /// </summary>
        Medium,

        /// <summary>
        /// High strength.
        /// </summary>
        High
    }
    
    /// <summary>
    /// A kernel based image processor used to apply Edge detection convolution.
    /// </summary>
    public class EdgeDetection : ConvolutionBase, IFrameProcessor
    {
        /// <summary>
        /// The kernel's width.
        /// </summary>
        public const int KernelWidth = 3;

        /// <summary>
        /// The kernel's height.
        /// </summary>
        public const int KernelHeight = 3;

        /// <summary>
        /// A kernel used to apply a low strength edge detection convolution to an image.
        /// </summary>
        public static double[,] LowStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { -1, 0, 1 },
            { 0, 0, 0 },
            { 1, 0, -1 }
        };

        /// <summary>
        /// A kernel used to apply a medium strength edge detection convolution to an image.
        /// </summary>
        public static double[,] MediumStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { 0, 1, 0 },
            { 1, -4, 1 },
            { 0, 1, 0 }
        };

        /// <summary>
        /// A kernel used to apply a high strength edge detection convolution to an image.
        /// </summary>
        public static double[,] HighStrengthKernel = new double[KernelWidth, KernelHeight]
        {
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
        };

        /// <summary>
        /// The working kernel.
        /// </summary>
        public double[,] Kernel { get; }
        
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
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(this.Kernel, KernelWidth, KernelHeight, context);
        }
    }
}