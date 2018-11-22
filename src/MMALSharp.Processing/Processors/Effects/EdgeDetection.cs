using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public enum EDStrength
    {
        Low,
        Medium,
        High
    }
    
    public class EdgeDetection : ConvolutionBase, IFrameProcessor
    {
        private const int KernelWidth = 3;
        private const int KernelHeight = 3;
        
        private double[,] Kernel { get; }
        
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
    
        public void Apply(byte[] store, IImageContext context)
        {
            this.Convolute(store, this.Kernel, KernelWidth, KernelHeight, context);
        }
    }
}