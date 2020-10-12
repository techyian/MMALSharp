// <copyright file="MotionAlgorithmBase.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// Utilities for derived motion algorithm classes.
    /// </summary>
    public abstract class MotionAlgorithmBase
    {
        /// <summary>
        /// Highlights a motion detection cell, typically to indicate a threshold was tripped.
        /// </summary>
        /// <param name="r">Red channel of the highlight RGB color</param>
        /// <param name="g">Green channel of the highlight RGB color</param>
        /// <param name="b">Blue channel of the highlight RGB color</param>
        /// <param name="driver">The <see cref="FrameDiffDriver"/> containing the buffer</param>
        /// <param name="metadata">The <see cref="FrameAnalysisMetadata"/> structure with frame properties</param>
        /// <param name="index">The array index of the cell to highlight</param>
        /// <param name="buffer">The frame buffer to draw into</param>
        protected void HighlightCell(byte r, byte g, byte b, FrameDiffDriver driver, FrameAnalysisMetadata metadata, int index, byte[] buffer)
        {
            for (int x = driver.CellRect[index].X; x < driver.CellRect[index].X + driver.CellRect[index].Width; x++)
            {
                var y = driver.CellRect[index].Y;
                var i = (x * metadata.Bpp) + (y * metadata.Stride);
                buffer[i] = r;
                buffer[i + 1] = g;
                buffer[i + 2] = b;
                y += driver.CellRect[index].Height - 1;
                i = (x * metadata.Bpp) + (y * metadata.Stride);
                buffer[i] = r;
                buffer[i + 1] = g;
                buffer[i + 2] = b;
            }

            for (int y = driver.CellRect[index].Y; y < driver.CellRect[index].Y + driver.CellRect[index].Height; y++)
            {
                var x = driver.CellRect[index].X;
                var i = (x * metadata.Bpp) + (y * metadata.Stride);
                buffer[i] = r;
                buffer[i + 1] = g;
                buffer[i + 2] = b;
                x += driver.CellRect[index].Width - 1;
                i = (x * metadata.Bpp) + (y * metadata.Stride);
                buffer[i] = r;
                buffer[i + 1] = g;
                buffer[i + 2] = b;
            }
        }

        /// <summary>
        /// Draws a filled block into the frame buffer. Can be used as a visual indicator of internal app state.
        /// </summary>
        /// <param name="r">Red channel of the highlight RGB color</param>
        /// <param name="g">Green channel of the highlight RGB color</param>
        /// <param name="b">Blue channel of the highlight RGB color</param>
        /// <param name="x1">Left column of the block</param>
        /// <param name="x2">Right column of the block</param>
        /// <param name="y1">Top row of the block</param>
        /// <param name="y2">Bottom row of the block</param>
        /// <param name="buffer">The frame buffer to draw into</param>
        /// <param name="metrics">The <see cref="FrameAnalysisMetadata"/> structure with frame properties</param>
        protected void DrawIndicatorBlock(byte r, byte g, byte b, int x1, int x2, int y1, int y2, byte[] buffer, FrameAnalysisMetadata metrics)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    var i = (x * metrics.Bpp) + (y * metrics.Stride);
                    buffer[i] = r;
                    buffer[i + 1] = g;
                    buffer[i + 2] = b;
                }
            }
        }
    }
}
