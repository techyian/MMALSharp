// <copyright file="FrameAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors
{
    /// <summary>
    /// The FrameAnalyser class is used with the Image Analysis API and
    /// is the base class for frame-differencing motion detection.
    /// </summary>
    public class FrameAnalyser : IFrameAnalyser
    {
        // Some members are fields rather than properties for parallel processing performance reasons.
        // Array-based fields are threadsafe as long as multiple threads access unique array indices.

        /// <summary>
        /// Cells are subsections of an image frame which are processed in parallel. This dictionary contains a list
        /// of recommended cell count values based on image resolution. The dictionary key is a (width, height) tuple
        /// and the value is a (horizontal, vertical) tuple. Multiply the horizontal and vertical values for total cell
        /// count. Approximately 800 to 1000 cells seems ideal for a Raspberry Pi 4B. Note that the buffer (width, height)
        /// can differ from the camera resolution (the hardware requires a horizontal 32-byte boundery and vertical 16-byte
        /// boundary). The padded buffer "resolutions" are also provided by this dictionary.
        /// </summary>
        public static readonly IReadOnlyDictionary<(int width, int height), (int horizontal, int vertical)> RecommendedCellCounts
            = new Dictionary<(int width, int height), (int horizontal, int vertical)>(13)
            {
                // For 1640 x 922, there is no useful divisor for the 922 pixel Y resolution. Dividing by 461 would
                // yield an integer cell height of 2, but convolution requires at least 3 pixels. Instead we use 23
                // which yields a cell height of just over 40, meaning the last row of pixels in each cell are not
                // processed (the indexers are integers). However, the padded-buffer version (second list) has a
                // vertical height of 928 which is divisible by 16.

                                                      // pixels per cell
                { (1920, 1080), (30, 30) }, // 900 cells    64 x 36     Y padded buffer (see list below)
                { (2592, 1944), (36, 36) }, // 1296 cells   72 x 54     Y  padded
                { (1296, 972),  (27, 27) }, // 729 cells    36 x 48     XY padded
                { (1296, 730),  (72, 10) }, // 720 cells    18 x 73     XY padded
                { (640,  480),  (32, 32) }, // 1024 cells   20 x 15     not padded
                { (3280, 2464), (40, 22) }, // 880 cells    82 x 112    X padded
                { (1640, 1232), (40, 22) }, // 880 cells    41 x 56     X padded
                { (1640, 922),  (40, 23) }, // 920 cells    41 x 40.09  XY padded
                { (1280, 720),  (20, 36) }, // 720 cells    64 x 20     not padded
                { (2028, 1080), (26, 36) }, // 936 cells    78 x 30     XY padded
                { (2028, 1520), (26, 38) }, // 988 cells    78 x 40     X padded
                { (4056, 3040), (26, 32) }, // 832 cells   156 x 95     X padded
                { (1012, 760),  (44, 19) }, // 836 cells    23 x 40     XY padded

                // The raw image hardware buffer is padded to align to a 32-byte width and 16-byte height. This
                // padded buffer size is what is stored into ImageContext.Resolution, not the requested camera
                // pixel resolution. The following list represents the padded buffer sizes. The data in the buffer
                // matches the camera resolution, the pixels added for padding are always empty (zero).

                { (1920, 1088), (30, 32) }, // 960 cells     64 x 34    res 1920 x 1080
                { (2592, 1952), (36, 32) }, // 1152 cells    72 x 61    res 2592 x 1944
                { (1312, 976),  (32, 16) }, // 512 cells     41 x 61    res 1296 x 972
                { (1312, 736),  (32, 23) }, // 736 cells     41 x 32    res 1296 x 730
                { (3296, 2464), (32, 22) }, // 704 cells    103 x 112   res 3280 x 2464
                { (1664, 1232), (52, 22) }, // 1144 cells    32 x 56    res 1640 x 1232
                { (1664, 928),  (52, 16) }, // 832 cells     32 x 58    res 1640 x 922
                { (2048, 1088), (32, 32) }, // 1024 cells    64 x 34    res 2028 x 1080
                { (2048, 1520), (32, 38) }, // 1216 cells    64 x 40    res 2028 x 1520
                { (4064, 3040), (32, 32) }, // 1024 cells   127 x 95    res 4056 x 3040
                { (1024, 768),  (32, 24) }, // 768 cells     32 x 32    res 1012 x 760
            };

        /// <summary>
        /// Tracks whether a full frame has been received and processed yet. Fields and properties
        /// like <see cref="CellRect"/> and <see cref="Metadata"/> are not valid until this is true.
        /// </summary>
        internal bool CompletedFirstFrame = false;

        /// <summary>
        /// Represents the coordinates of each test cell for parallel processing. This is
        /// threadsafe if threads do not access overlapping array indices.
        /// </summary>
        internal Rectangle[] CellRect;

        /// <summary>
        /// A byte array representation of the FrameAnalyser's own WorkingData object. Required
        /// to provide fast thread-safe access for parallel analysis.
        /// </summary>
        internal byte[] CurrentFrame;

        /// <summary>
        /// Frame details collected when the first full frame is available. This is a struct and is
        /// a threadsafe copy when passed by value as a method argument. Multiple threads must never
        /// access this instance directly.
        /// </summary>
        internal FrameAnalysisMetadata Metadata;

        /// <summary>
        /// The number of cells an image frame is divided into vertically for parallel processing. This should
        /// be a value that divides evenly into the Y resolution of the image frame. A list of recommended
        /// values is provided in <see cref="RecommendedCellCounts"/>. If this value is not set when the first
        /// full frame is received, the image resolution will use the recommended setting.
        /// </summary>
        public int HorizonalCellCount { get; set; }

        /// <summary>
        /// The number of cells an image frame is divided into vertically for parallel processing. This should
        /// be a value that divides evenly into the Y resolution of the image frame. A list of recommended
        /// values is provided in <see cref="RecommendedCellCounts"/>. If this value is not set when the first
        /// full frame is received, the image resolution will use the recommended setting.
        /// </summary>
        public int VerticalCellCount { get; set; }

        /// <summary>
        /// The frame we are working with.
        /// </summary>
        protected List<byte> WorkingData { get; set; }

        /// <summary>
        /// True if the working data store contains a full frame.
        /// </summary>
        protected bool FullFrame { get; set; }

        /// <summary>
        /// The number of bytes defining a pixel based on the <see cref="PixelFormat"/>.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        /// <returns>The number of bytes</returns>
        public int GetBytesPerPixel(ImageContext context)
        {
            if(context.PixelFormat == null)
                throw new Exception("Pixel format is null");

            // RGB16 doesn't appear to be supported by GDI?
            if (context.PixelFormat == MMALEncoding.RGB24)
            {
                return 24 / 8;
            }

            if (context.PixelFormat == MMALEncoding.RGB32 || context.PixelFormat == MMALEncoding.RGBA)
            {
                return 32 / 8;
            }

            throw new Exception($"Unsupported pixel format: {context.PixelFormat}");
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameAnalyser"/>.
        /// </summary>
        public FrameAnalyser()
        {
            this.WorkingData = new List<byte>();
        }

        /// <summary>
        /// Applies an operation.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        public virtual void Apply(ImageContext context)
        {
            if (this.FullFrame)
            {
                MMALLog.Logger.LogDebug("Clearing frame");
                this.WorkingData.Clear();
                this.FullFrame = false;
            }

            this.WorkingData.AddRange(context.Data);

            if (context.Eos)
            {
                this.FullFrame = true;
                
                this.CurrentFrame = this.WorkingData.ToArray();

                if (!CompletedFirstFrame)
                {
                    ProcessFirstFrame(context);
                    CompletedFirstFrame = true;
                }
            }
        }

        /// <summary>
        /// Executed the first time <see cref="Apply"/> receives an <see cref="ImageContext"/> with EOS set to true.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        protected virtual void ProcessFirstFrame(ImageContext context)
        {
            // Collect basic frame dimensions
            Metadata.Width = context.Resolution.Width;
            Metadata.Height = context.Resolution.Height;
            Metadata.Bpp = this.GetBytesPerPixel(context);
            Metadata.Stride = context.Stride;

            if(HorizonalCellCount == 0 || VerticalCellCount == 0)
            {
                (int h, int v) counts;

                if(!RecommendedCellCounts.TryGetValue((Metadata.Width, Metadata.Height), out counts))
                {
                    throw new Exception($"Resolution {Metadata.Width}x{Metadata.Height} has no recommended cell counts");
                }

                HorizonalCellCount = counts.h;
                VerticalCellCount = counts.v;
            }

            // Prepare the parallel processing cells
            int indices = HorizonalCellCount * VerticalCellCount;
            Metadata.CellWidth = Metadata.Width / HorizonalCellCount;
            Metadata.CellHeight = Metadata.Height / VerticalCellCount;
            int i = 0;

            CellRect = new Rectangle[indices];

            for (int row = 0; row < VerticalCellCount; row++)
            {
                int y = row * Metadata.CellHeight;
                for (int col = 0; col < HorizonalCellCount; col++)
                {
                    int x = col * Metadata.CellWidth;
                    CellRect[i] = new Rectangle(x, y, Metadata.CellWidth, Metadata.CellHeight);
                    i++;
                }
            }
        }
    }
}
