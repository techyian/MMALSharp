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

            
            if (result.ProcessedStreams.Count == 0)
                return;

            //Create temporary directory and copy all files in the capture handler to it.
            var tempDirectory = result.ProcessedStreams.FirstOrDefault().Item1.TrimEnd('/') + "/mmalsharptemp/";
            var extension = result.ProcessedStreams.FirstOrDefault().Item3;
            try
            {
                System.IO.Directory.CreateDirectory(tempDirectory);

                foreach (var tuple in result.ProcessedStreams)
                {
                    System.IO.File.Copy(tuple.Item1.TrimEnd('/') + "/" + tuple.Item2.TrimEnd('.') + tuple.Item3, tempDirectory + tuple.Item2.TrimEnd('.') + tuple.Item3);
                }

                targetDirectory.TrimEnd('/');

                if (fps == 0)
                {
                    //Default to 25fps - FFmpeg defaults to this value if nothing is specified
                    fps = 25;
                }

                process.StartInfo.Arguments = string.Format("-framerate {0} -f image2 -pattern_type glob -i {1} {2}/out.avi", fps, tempDirectory + "'*" + extension + "'", targetDirectory);
                process.Start();
                process.WaitForExit();
            }
            finally
            {
                //Make sure we try to cleanup even if error occurs.
                if(System.IO.Directory.Exists(tempDirectory))
                {
                    System.IO.Directory.Delete(tempDirectory, true);
                }                
            }
            


        }
    }
}
