using MMALSharp.Components;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MMALSharp.Native.MMALParametersCamera;
using static MMALSharp.MMALParameterHelpers;
using System.IO;
using System.Diagnostics;

namespace MMALSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MMALCameraConfig config = new MMALCameraConfig
            {
                Sharpness = 100,
                Rotation = 50,
                Contrast = 10,
                ImageEffect = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NEGATIVE 
            };

            using (MMALCamera cam = new MMALCamera(config))
            {
                cam.ConfigureCamera().TakePicture("/home/pi/test2.jpg");

                Console.WriteLine("Brightness " + cam.Brightness);
                Console.WriteLine("Contrast " + cam.Contrast);
                Console.WriteLine("Sharpness " + cam.Sharpness);

                //cam.Brightness = 10;
                //cam.TakePicture("/home/pi/test3.jpg");
                
                /*Task.Run(async () => {
                    await cam.TakePictureAsync("/home/pi/test1.jpg");
                }).GetAwaiter().GetResult();*/
            }

            Console.WriteLine("Exit");
                  
        }
    }
}
