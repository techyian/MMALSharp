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
            MMALCameraConfig config = new MMALCameraConfig
            {
                EnableAnnotate = true,
                Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true }                
            };

            //Assign our config to the global config object
            MMALCameraConfigImpl.Config = config;

            using (MMALCamera cam = MMALCamera.Instance)
            {
                //Create our component pipeline. 
                cam.AddEncoder(new MMALImageEncoder(), cam.Camera.StillPort)
                   .CreatePreviewComponent(new MMALNullSinkComponent())
                   .ConfigureCamera();
                
                AsyncContext.Run(async () =>
                {
                    //Take a picture on output port '0' of the encoder connected to the Camera's still port, sending the output to a filestream.
                    using (var fs = File.Create("/home/pi/test5.jpg"))
                    {
                        await cam.TakePicture(cam.Camera.StillPort, 0, new StreamCaptureResult(fs));
                    }                                        
                });                
            }
        }
    }
}
