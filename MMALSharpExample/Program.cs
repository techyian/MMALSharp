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
            MMALCameraConfig.Debug = true;
            
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

                    //Example of using the standlone capture method. This will hook up an Image encoder to the Still port of the camera.
                    using (var fs = File.Create("/home/pi/test2.jpg"))
                    {
                        await cam.TakeSinglePicture(new StreamCaptureResult(fs));
                    }

                    /*
                     * By using the TakeSinglePicture method, we disposed of our previously connected encoder connected to the camera's Still port.
                     * Therefore, we must create a new one in order to use the manually constructed methods.
                    */
                    cam.AddEncoder(new MMALImageEncoder(), cam.Camera.StillPort);

                    using (var fs = File.Create("/home/pi/test3.jpg"))
                    {
                        await cam.TakePicture(cam.Camera.StillPort, 0, new StreamCaptureResult(fs));
                    }

                    /*
                     * Here we are changing the image encoder being used by the camera's still port by replacing it with a Bitmap encoder. It is important
                     * to remove any resources being used by the old encoder before replacing with a new one.
                    */
                    cam.RemoveEncoder(cam.Camera.StillPort).AddEncoder(new MMALImageEncoder(MMALEncodings.MMAL_ENCODING_BMP, 90), cam.Camera.StillPort);

                    await cam.TakePictureIterative(cam.Camera.StillPort, 0, "/home/pi/", "bmp", 5);

                });          
                
                
                      
            }
        }
    }
}
