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
            
            using (MMALCamera cam = MMALCamera.Instance)
            {
                //Create our component pipeline. 
                cam.AddEncoder(new MMALImageEncoder(), cam.Camera.StillPort)
                   .CreatePreviewComponent(new MMALNullSinkComponent())
                   .ConfigureCamera();
                
                AsyncContext.Run(async () =>
                {
                    //Take a picture on output port '0' of the encoder connected to the Camera's still port, sending the output to a filestream.
                    using (var fs = File.Create("/home/pi/test.jpg"))
                    {
                        await cam.TakePicture(cam.Camera.StillPort, 0, new StreamCaptureResult(fs));
                    }                                        
                });          
                
                
                      
            }
        }
    }
}
