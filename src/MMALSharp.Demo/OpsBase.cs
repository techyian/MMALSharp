using System;
using MMALSharp.Common;
using MMALSharp.Native;

namespace MMALSharp.Demo
{
    public abstract class OpsBase
    {
        protected MMALCamera Cam => MMALCamera.Instance;

        public abstract void Operations();
        
        protected Tuple<MMALEncoding, MMALEncoding> ParsePixelFormat()
        {
            Console.WriteLine("\nPlease select an image format.");
            var format = Console.ReadLine();
            Console.WriteLine("\nPlease select a pixel format.");
            var pixelFormat = Console.ReadLine();
            
            if (string.IsNullOrEmpty(format) || string.IsNullOrEmpty(pixelFormat))
            {
                Console.WriteLine("Please enter valid formats.");
                this.ParsePixelFormat();
            }

            var parsedFormat = format.ParseEncoding();
            var parsedPixelFormat = format.ParseEncoding();

            if (parsedFormat == null || parsedPixelFormat == null)
            {
                Console.WriteLine("Could not parse format. Please try again.");
                this.ParsePixelFormat();
            }
            
            return new Tuple<MMALEncoding, MMALEncoding>(parsedFormat, parsedPixelFormat);
        }
    }
}