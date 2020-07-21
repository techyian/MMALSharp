// <copyright file="VideoUtilities.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using System.Diagnostics;
using System.Linq;

namespace MMALSharp.FFmpeg
{
    /// <summary>
    /// This class provides utility methods for video capturing and converting based on FFmpeg.
    /// </summary>
    public static class VideoUtilities
    {
        /// <summary>
        /// Useful for Timelapse captures. Enables you to convert a list of images associated with an ImageStreamCaptureHandler to a video.
        /// </summary>
        /// <param name="result">The list of images we wish to process.</param>
        /// <param name="targetDirectory">The target directory we want to save the video to.</param>
        /// <param name="fps">The framerate in fps to set as -framerate parameter for FFmpeg.</param>
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

            // Create temporary directory and copy all files in the capture handler to it.
            var tempDirectory = result.ProcessedFiles.FirstOrDefault().Directory.TrimEnd('/') + "/mmalsharptemp/";
            var extension = result.ProcessedFiles.FirstOrDefault().Extension;

            try
            {
                System.IO.Directory.CreateDirectory(tempDirectory);

                foreach (var tuple in result.ProcessedFiles)
                {
                    System.IO.File.Copy($"{tuple.Directory.TrimEnd('/')}/{tuple.Filename.TrimEnd('.')}.{tuple.Extension}", $"{tempDirectory}{tuple.Filename.TrimEnd('.')}.{tuple.Extension}");
                }

                targetDirectory.TrimEnd('/');

                if (fps == 0)
                {
                    // Default to 25fps - FFmpeg defaults to this value if nothing is specified
                    fps = 25;
                }

                process.StartInfo.Arguments = $"-framerate {fps} -f image2 -pattern_type glob -y -i {tempDirectory + "'*." + extension + "'"} {targetDirectory}/out.avi";
                process.Start();
                process.WaitForExit();
            }
            finally
            {
                // Make sure we try to cleanup even if error occurs.
                if (System.IO.Directory.Exists(tempDirectory))
                {
                    System.IO.Directory.Delete(tempDirectory, true);
                }
            }
        }
    }
}
