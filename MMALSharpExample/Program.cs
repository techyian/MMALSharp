using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharpExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Alter any configuration properties required.         
            MMALCameraConfig.EnableAnnotate = true;
            MMALCameraConfig.Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true };            
            MMALCameraConfig.VideoHeight = 1024;
            MMALCameraConfig.VideoWidth = 768;
            MMALCameraConfig.Debug = true;
            MMALCameraConfig.InlineHeaders = true;

            MMALCamera cam = MMALCamera.Instance;
             
            //Create our component pipeline.                
            cam.AddEncoder(new MMALVideoEncoder(40), cam.Camera.VideoPort)
                .AddEncoder(new MMALImageEncoder(), cam.Camera.StillPort)
                .CreatePreviewComponent(new MMALVideoRenderer())
                .ConfigureCamera();

            AsyncContext.Run(async () =>
            {
                //Record video for 1 minute, using segmented video record to split into multiple files every 30 seconds.
                await cam.TakeVideo(cam.Camera.VideoPort, new StreamCaptureResult(File.Create("/home/pi/testvideo.avi")), DateTime.Now.AddMinutes(1), new Split { Mode = TimelapseMode.Second, Value = 30 });

                //Take multiple pictures every 20 seconds for 1 hour as a timelapse. 
                await cam.TakePictureTimelapse("/home/pi/timelapse", "jpg", new Timelapse { Mode = TimelapseMode.Second, Value = 20, Timeout = DateTime.Now.AddHours(1) }, null);

                //Take a picture on the camera's still port using the encoder connected to the still port
                await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort, new StreamCaptureResult(File.Create("/home/pi/testimage1.jpg")));
                    
                //Example of using the standlone capture method. This will hook up an Image encoder to the Still port of the camera.                    
                await cam.TakeSinglePicture(new StreamCaptureResult(File.Create("/home/pi/singlepicture.jpg")), null);
                    
                /*
                    * Here we are changing the image encoder being used by the camera's still port by replacing it with a Bitmap encoder. It is important
                    * to remove any resources being used by the old encoder before replacing with a new one.
                */
                cam.RemoveEncoder(cam.Camera.StillPort).AddEncoder(new MMALImageEncoder(MMALEncodings.MMAL_ENCODING_BMP, 90), cam.Camera.StillPort);

                await cam.TakePictureIterative("/home/pi/", "bmp", 5, null);

                cam.Dispose();
            });
                        
        }        
    }
}
