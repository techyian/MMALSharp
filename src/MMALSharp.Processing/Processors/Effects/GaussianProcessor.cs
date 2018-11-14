using System.Numerics;
using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public class GaussianProcessor : IFrameProcessor
    {
        private Vector3[] Kernel = new Vector3[]
        {
            new Vector3(0.0625f, 0.125f, 0.0625f),
            new Vector3(0.125f,  0.25f,  0.125f),
            new Vector3(0.0625f, 0.125f, 0.0625f) 
        };
        
        public void Apply(byte[] store, IImageContext context)
        {
            int currentElement = 0;
            int bytesPerPixel = context.PixelFormat.PixelSize / 8;
            int processed = 0;
            
            for (var i = 1; i <= context.Resolution.Width; i++)
            {
                for (var j = 1; j <= context.Resolution.Height; j++)
                {
                    processed++;
                    
                    currentElement = currentElement + 
                    if (i > 3 && j > 3)
                    {
                        
                    }
                }
                processed++;
            }
        }
    }
}