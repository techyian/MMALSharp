using System.Numerics;
using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public class GaussianProcessor : ConvolutionBase, IFrameProcessor
    {
        private const int KernelWidth = 3;
        private const int KernelHeight = 3;
        
        private double[,] kernel = new double[KernelWidth, KernelHeight]
        {
            { 0.0625, 0.125, 0.0625 },
            { 0.125,  0.25,  0.125  },
            { 0.0625, 0.125, 0.0625 } 
        };
        
        public void Apply(byte[] store, IImageContext context)
        {
            this.Convolute(store, kernel, KernelWidth, KernelHeight, context);
        }
    }
}