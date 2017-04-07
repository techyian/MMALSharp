using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;

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
                                    
            AsyncContext.Run(async () =>
            {
                using (var vidEncoder = new MMALVideoEncoder(new VideoStreamCaptureHandler("/home/pi/videos", ".avi"), 40))
                using (var imgEncoder = new MMALImageEncoder(new ImageStreamCaptureHandler("/home/pi/images/", "jpg")))
                {
                    //Create our component pipeline.         
                    cam.AddEncoder(vidEncoder, cam.Camera.VideoPort)
                       .AddEncoder(imgEncoder, cam.Camera.StillPort)
                       .CreatePreviewComponent(new MMALVideoRenderer())
                       .ConfigureCamera();

                    //Record video for 1 minute, using segmented video record to split into multiple files every 30 seconds.
                    await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1), new Split { Mode = TimelapseMode.Second, Value = 30 });

                    //Take multiple pictures every 20 seconds for 1 hour as a timelapse. 
                    await cam.TakePictureTimelapse(cam.Camera.StillPort, cam.Camera.StillPort, new Timelapse { Mode = TimelapseMode.Second, Value = 20, Timeout = DateTime.Now.AddHours(1) });

                    //Take a single picture on the camera's still port using the encoder connected to the still port
                    await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort);
                }
                
                /*
                 * Here we are changing the image encoder being used by the camera's still port by replacing it with a Bitmap encoder. 
                */
                using (var imgEncoder = new MMALImageEncoder(new ImageStreamCaptureHandler("/home/pi/images/", "jpg"), MMALEncodings.MMAL_ENCODING_BMP, 90))
                {
                    cam.AddEncoder(imgEncoder, cam.Camera.StillPort);

                    await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort);
                }
                
                //Once we're finished with the camera and will *not* use it again, cleanup any unmanaged resources.
                cam.Cleanup();                
            });
                        
        }        
    }
}
