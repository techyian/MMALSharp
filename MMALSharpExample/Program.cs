using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
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
                Rotation = 90                
            };

            using (MMALCamera cam = new MMALCamera(config))
            {
                cam.ConfigureCamera().TakePicture(new FileCaptureHandler("/home/pi/test3.jpg"), MMALEncodings.MMAL_ENCODING_JPEG, 90);
            }
        }
    }
}
