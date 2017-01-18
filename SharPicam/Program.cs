using SharPicam.Components;
using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SharPicam.Native.MMALParametersCamera;
using static SharPicam.MMALParameterHelpers;
using System.IO;
using System.Diagnostics;

namespace SharPicam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (MMALCamera cam = new MMALCamera())
            {
                cam.TakePicture("/home/pi/test1.jpg").Wait();                
            }

            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

            if(currentThreads != null)
            {
                foreach (ProcessThread thread in currentThreads)
                {
                    Console.WriteLine("Thread");
                }
            }

            

            
        }
    }
}
