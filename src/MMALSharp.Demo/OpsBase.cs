using System;
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

            var parsedFormat = MMALEncoding.BMP;
            var parsedPixelFormat = MMALEncoding.I420;

            if (parsedFormat == null || parsedPixelFormat == null)
            {
                Console.WriteLine("Could not parse format. Please try again.");
                this.ParsePixelFormat();
            }
            
            return new Tuple<MMALEncoding, MMALEncoding>(parsedFormat, parsedPixelFormat);
        }
    }
}