using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
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
                EnableAnnotate = true,
                Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true },
                StillWidth = 1024,
                StillHeight = 768
            };

            using (MMALCamera cam = new MMALCamera(config))
            {
                AsyncContext.Run(async () =>
                {
                    await cam.ConfigureCamera().TakePicture(new FileCaptureHandler("/home/pi/test3.jpg"), MMALEncodings.MMAL_ENCODING_JPEG, 90);
                });                
            }
        }
    }
}
