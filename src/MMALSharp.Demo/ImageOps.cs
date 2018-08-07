using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Demo
{
    public class ImageOps : OpsBase
    {
        public override void Operations()
        {
            Console.WriteLine("\nPicture Operations:");
            
            Console.WriteLine("1.    Take Picture");
            Console.WriteLine("2.    Raw Capture");
            Console.WriteLine("3.    Resize Component");

            var key = Console.ReadKey();
            var formats = this.ParsePixelFormat();

            switch (key.KeyChar)
            {
                case '1':
                    this.TakePictureOperations(formats.Item1, formats.Item2);
                    break;
                case '2':
                    this.TakeRawOperations();
                    break;
                case '3':
                    this.TakeResizeOperations(formats.Item1, formats.Item2);
                    break;
            }
            
            Program.OperationsHandler();
        }
        
        private void TakePictureOperations(MMALEncoding encoding, MMALEncoding pixelFormat)
        {
            Console.WriteLine("\nPlease enter a file extension.");
            var extension = Console.ReadLine();
            this.TakePictureManual(extension, encoding, pixelFormat).GetAwaiter().GetResult();
        }

        private void TakeRawOperations()
        {
            Console.WriteLine("\nPlease enter a file extension.");
            var extension = Console.ReadLine();
            this.TakeRawSensor(extension).GetAwaiter().GetResult();
        }
        
        private void TakeResizeOperations(MMALEncoding encoding, MMALEncoding pixelFormat)
        {
            Console.WriteLine("\nPlease enter a file extension.");
            var extension = Console.ReadLine();
            
            Console.WriteLine("\nEnter the width of the resized image.");
            var width = Console.ReadLine();
            
            Console.WriteLine("\nEnter the height of the resized image.");
            var height = Console.ReadLine();

            int intWidth = 0, intHeight = 0;

            if (!int.TryParse(width, out intWidth) || !int.TryParse(height, out intHeight))
            {
                Console.WriteLine("Invalid values entered, please try again.");
                this.TakeResizeOperations(encoding, pixelFormat);
            }
            
            this.ResizePicture(extension, encoding, pixelFormat, intWidth, intHeight).GetAwaiter().GetResult();
        }
        
        private async Task TakePictureManual(string extension, MMALEncoding encoding, MMALEncoding pixelFormat)
        {
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", extension))
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
            using (var nullSink = new MMALNullSinkComponent())
            {
                this.Cam.ConfigureCameraSettings();
                await Task.Delay(2000);        
                
                // Create our component pipeline.         
                imgEncoder.ConfigureOutputPort(0, encoding, pixelFormat, 90);
                
                this.Cam.Camera.StillPort.ConnectTo(imgEncoder);                    
                this.Cam.Camera.PreviewPort.ConnectTo(nullSink);
        
                await this.Cam.ProcessAsync(this.Cam.Camera.StillPort);
            }
        }

        private async Task TakeRawSensor(string extension)
        {
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            {
                await this.Cam.TakeRawPicture(imgCaptureHandler);
            }
        }
        
        private async Task ResizePicture(string extension, MMALEncoding encoding, MMALEncoding pixelFormat, int width, int height)
        {
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", extension))
            using (var resizer = new MMALResizerComponent(width, height, null))
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
            using (var nullSink = new MMALNullSinkComponent())
            {
                this.Cam.ConfigureCameraSettings();

                await Task.Delay(2000);
                
                // Create our component pipeline.         
                resizer.ConfigureInputPort(MMALCameraConfig.StillEncoding, MMALCameraConfig.StillSubFormat, this.Cam.Camera.StillPort);
                resizer.ConfigureOutputPort(0, pixelFormat, pixelFormat, 0);
            
                imgEncoder.ConfigureOutputPort(0, encoding, pixelFormat, 90);
                    
                this.Cam.Camera.StillPort.ConnectTo(resizer);                    
                resizer.Outputs[0].ConnectTo(imgEncoder);
                this.Cam.Camera.PreviewPort.ConnectTo(nullSink);
                
                await this.Cam.ProcessAsync(this.Cam.Camera.StillPort);
            }
        }
        
        
    }
}