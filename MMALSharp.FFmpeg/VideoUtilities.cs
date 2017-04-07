using MMALSharp.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.FFmpeg
{
    public static class VideoUtilities
    {
        /// <summary>
        /// Useful for Timelapse captures. Enables you to convert a list of images associated with an ImageStreamCaptureHandler to a video
        /// </summary>
        /// <param name="result"></param>
        /// <param name="targetDirectory"></param>
        public static void ImagesToVideo(this ImageStreamCaptureHandler result, string targetDirectory, int fps)
        {            
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "ffmpeg";

            StringBuilder sb = new StringBuilder();
            result.ProcessedStreams.ForEach(c => sb.Append(string.Format(" -i {0}", c)));
            targetDirectory.TrimEnd('/');
            
            if(fps == 0)
            {
                //Default to 25fps - FFmpeg defaults to this value if nothing is specified
                fps = 25;
            }

            process.StartInfo.Arguments = string.Format("-framerate {0} {1} {2}/out.avi", fps, sb.ToString(), targetDirectory);
            process.Start();
            process.WaitForExit();
        }
    }
}
