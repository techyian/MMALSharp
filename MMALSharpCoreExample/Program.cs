using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;

namespace MMALSharpCore.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Alter any configuration properties required.         
            MMALCameraConfig.EnableAnnotate = true;
            //MMALCameraConfig.Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true };            
            MMALCameraConfig.VideoResolution = new Resolution(1024, 768);
            MMALCameraConfig.PreviewResolution = new Resolution(1024, 768);
            MMALCameraConfig.StillResolution = new Resolution(1024, 768);
            MMALCameraConfig.Debug = true;
                        
            MMALCameraConfig.InlineHeaders = true;

            MMALCamera cam = MMALCamera.Instance;
                                    
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos", ".avi");
                
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, 10, 25))
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    //Create our component pipeline.         
                    cam
                       .AddEncoder(imgEncoder, cam.Camera.StillPort)
                       .CreatePreviewComponent(new MMALVideoRenderer())
                       .ConfigureCamera();

                    //Record video for 1 minute, using segmented video record to split into multiple files every 30 seconds.
                    //await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1), new Split { Mode = TimelapseMode.Second, Value = 30 });

                    //Take multiple pictures every 5 seconds for 1 minute as a timelapse. 
                    //await cam.TakePictureTimelapse(cam.Camera.StillPort, cam.Camera.StillPort, new Timelapse { Mode = TimelapseMode.Second, Value = 5, Timeout = DateTime.Now.AddMinutes(1) });

                    //Take a single picture on the camera's still port using the encoder connected to the still port
                    await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort, useExif: false);

                    //Processes the list of images you've taken with the *ImageStreamCaptureHandler* class into a video
                    //imgCaptureHandler.ImagesToVideo("/home/pi/videos", 2);
                }

                /*
                 * Exiting the using statement will cleanly remove the encoders from the camera, 
                 * allowing us to create a new one for further camera activity. 
                */
                                
                //Here we are changing the image encoder being used by the camera's still port by replacing it with a Bitmap encoder.                 
                /*using (var imgEncoder = new MMALImageEncoder(new ImageStreamCaptureHandler("/home/pi/images/", "bmp"), MMALEncoding.MMAL_ENCODING_BMP, 90))
                {
                    cam.AddEncoder(imgEncoder, cam.Camera.StillPort)
                       .CreatePreviewComponent(new MMALNullSinkComponent())
                       .ConfigureCamera();
                    
                    await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort);
                }*/
                                
                /*var ffmpegCaptureHandler = FFmpegCaptureHandler.RTMPStreamer("mystream", "rtmp://192.168.1.91:6767/live");

                using (var vidEncoder = new MMALVideoEncoder(ffmpegCaptureHandler, 40, 15))
                {
                    cam.AddEncoder(vidEncoder, cam.Camera.VideoPort)                     
                       .CreatePreviewComponent(new MMALVideoRenderer())
                       .ConfigureCamera();

                    /*
                     * Stream video for 1 minute via RTMP using the *FFmpegCaptureHandler* class. 
                     * Note: FFmpeg must be installed for this method to work correctly and an appropriate RTMP server running such as https://github.com/arut/nginx-rtmp-module
                    */
                 //   await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1));
                //}
                                    
                //Once we're finished with the camera and will *not* use it again, cleanup any unmanaged resources.
                cam.Cleanup();                
            });
                        
        }        
    }
}
