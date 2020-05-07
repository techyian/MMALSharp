using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Demo
{
    public class VideoOps : OpsBase
    {
        public override void Operations()
        {
            Console.WriteLine("\nVideo Operations:");
            
            Console.WriteLine("1.    Take Video");
            
            var key = Console.ReadKey();
            var formats = this.ParsePixelFormat();

            switch (key.KeyChar)
            {
                case '1':
                    this.TakeVideoOperations(formats.Item1, formats.Item2);
                    break;
            }
            
            Program.OperationsHandler();
        }
        
        private void TakeVideoOperations(MMALEncoding encoding, MMALEncoding pixelFormat)
        {
            Console.WriteLine("\nPlease enter a file extension.");
            var extension = Console.ReadLine();
            Console.WriteLine("\nPlease enter the bitrate value.");
            var bitrate = Console.ReadLine();
            Console.WriteLine("\nPlease enter the number of seconds to record for.");
            var seconds = Console.ReadLine();
            
            int intBitrate = 0, intSeconds = 0;

            if (!int.TryParse(bitrate, out intBitrate) || !int.TryParse(seconds, out intSeconds))
            {
                Console.WriteLine("Invalid values entered, please try again.");
                this.TakeVideoOperations(encoding, pixelFormat);
            }
            
            this.TakeVideoManual(extension, encoding, pixelFormat, intBitrate, intSeconds).GetAwaiter().GetResult();
        }
        
        private async Task TakeVideoManual(string extension, MMALEncoding encoding, MMALEncoding pixelFormat, int bitrate, int seconds)
        {            
            using (var vidCaptureHandler = new VideoStreamCaptureHandler($"/home/pi/videos/", extension))
            using (var vidEncoder = new MMALVideoEncoder())
            using (var renderer = new MMALVideoRenderer())
            {
                this.Cam.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encoding, pixelFormat, bitrate: bitrate);
                
                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                this.Cam.Camera.VideoPort.ConnectTo(vidEncoder);
                this.Cam.Camera.PreviewPort.ConnectTo(renderer);
                                                  
                // Camera warm up time
                await Task.Delay(2000);
            
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
                                
                await this.Cam.ProcessAsync(this.Cam.Camera.VideoPort, cts.Token);
            }
        }
    }
}