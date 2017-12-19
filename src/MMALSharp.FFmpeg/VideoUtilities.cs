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
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = "ffmpeg"
                }
            };
            
            if (result.ProcessedFiles.Count == 0)
                return;

            //Create temporary directory and copy all files in the capture handler to it.
            var tempDirectory = result.ProcessedFiles.FirstOrDefault().Directory.TrimEnd('/') + "/mmalsharptemp/";
            var extension = result.ProcessedFiles.FirstOrDefault().Extension;
            
            try
            {
                System.IO.Directory.CreateDirectory(tempDirectory);

                foreach (var tuple in result.ProcessedFiles)
                {
                    System.IO.File.Copy(tuple.Directory.TrimEnd('/') + "/" + tuple.Filename.TrimEnd('.') + tuple.Extension, tempDirectory + tuple.Filename.TrimEnd('.') + tuple.Extension);
                }

                targetDirectory.TrimEnd('/');

                if (fps == 0)
                {
                    //Default to 25fps - FFmpeg defaults to this value if nothing is specified
                    fps = 25;
                }

                process.StartInfo.Arguments = $"-framerate {fps} -f image2 -pattern_type glob -i {tempDirectory + "'*" + extension + "'"} {targetDirectory}/out.avi";
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
