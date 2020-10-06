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
        /// count. Approximately 800 to 1000 cells seems ideal for a Raspberry Pi 4B.
        /// </summary>
        public static IReadOnlyDictionary<(int width, int height), (int horizontal, int vertical)> RecommendedCellCounts
            = new Dictionary<(int width, int height), (int horizontal, int vertical)>(13)
            {
                { (1920, 1080), (30, 30) }, // 900 cells    64 x 36
                { (2592, 1944), (36, 36) }, // 1296 cells   72 x 54
                { (1296, 972),  (27, 27) }, // 729 cells    36 x 27
                { (1296, 730),  (48, 10) }, // 480 cells    27 x 73
                { (640, 480),   (32, 32) }, // 1024 cells   20 x 15
                { (3280, 2464), (40, 22) }, // 880 cells    82 x 112
                { (1640, 1232), (40, 22) }, // 880 cells    41 x 56
                { (1640, 922),  (40, 23) }, // 920 cells    41 x 40.09 (922 has no useful divisor)
                { (1280, 720),  (20, 36) }, // 720 cells    64 x 20
                { (2028, 1080), (26, 36) }, // 936 cells    78 x 30
                { (2028, 1520), (26, 38) }, // 988 cells    78 x 40
                { (4056, 3040), (00, 32) }, // 988 cells   156 x 80
                { (1012, 760),  (00, 00) }, // 874 cells    44 x 20
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
