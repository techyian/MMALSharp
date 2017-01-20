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
            using (MMALCamera cam = new MMALCamera())
            {
                /*Task.Run(async () => {
                    await cam.TakePicture("/home/pi/test1.jpg");
                }).GetAwaiter().GetResult();*/

                cam.TakePicture("/home/pi/test1.jpg");                                
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
