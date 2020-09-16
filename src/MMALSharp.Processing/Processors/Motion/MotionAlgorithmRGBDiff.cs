// <copyright file="MotionAlgorithmRGBDiff.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using MMALSharp.Handlers;
using System;
using System.Threading.Tasks;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// A motion detection algorithm based on per-pixel RGB differencing.
    /// </summary>
    public class MotionAlgorithmRGBDiff : MotionAlgorithmBase, IMotionAlgorithm
    {
        private ThreadSafeParameters _parameters = default;

        private int _cellPixelPercentage;
        private int _cellCountThreshold;

        private ImageContext _fullRawFrameImageContext;
        private IOutputCaptureHandler _outputHandler;
        private byte[] _analysisBuffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rgbThreshold">The minimum RGB difference to indicate the pixel has changed. Maximum value is 765 (full white to full black).</param>
        /// <param name="cellPixelPercentage">Percentage of pixels in each cell to mark the cell as changed.</param>
        /// <param name="cellCountThreshold">Minimum number of cells changed to trigger motion detection.</param>
        public MotionAlgorithmRGBDiff(int rgbThreshold = 200, int cellPixelPercentage = 50, int cellCountThreshold = 20)
        {
            // RGB 255 x 3 = 765
            if(rgbThreshold > 765)
            {
                throw new ArgumentException("Maximum RGB Threshold value is 765");
            }

            // Store this into the thread safe struct to pass to CheckDiff
            _parameters.RGBThreshold = rgbThreshold;

            // Can't calculate actual pixels until we get metadata in FirstFrameCompleted
            _cellPixelPercentage = cellPixelPercentage;

            // Not used in the parallel processing stage, store locally
            _cellCountThreshold = cellCountThreshold;

            _parameters.AnalysisMode = false;
        }

        /// <inheritdoc />
        public void EnableAnalysis(IOutputCaptureHandler handler = null)
        {
            _parameters.AnalysisMode = true;
            _outputHandler = handler;
        }

        /// <inheritdoc />
        public void DisableAnalysis()
        {
            _parameters.AnalysisMode = true;
        }

        /// <inheritdoc />
        public void FirstFrameCompleted(FrameDiffDriver driver, FrameAnalysisMetadata metadata, ImageContext contextTemplate)
        {
            _fullRawFrameImageContext = contextTemplate;

            _parameters.CellPixelThreshold = (int)(metadata.CellWidth * metadata.CellHeight * (_cellPixelPercentage / 100f));

            _analysisBuffer = new byte[driver.TestFrame.Length];
            // Not necessary for this analysis, CheckDiff overwrites the buffer completely
            // Array.Copy(driver.TestFrame, _analysisBuffer, _analysisBuffer.Length);

            _fullRawFrameImageContext.Data = driver.TestFrame;
            _outputHandler?.Process(_fullRawFrameImageContext);
            
            _fullRawFrameImageContext.Data = _analysisBuffer;
        }

        /// <inheritdoc />
        public void ResetAnalyser(FrameDiffDriver driver, FrameAnalysisMetadata metadata)
        { } // not necessary for this algorithm

        /// <inheritdoc />
        public bool DetectMotion(FrameDiffDriver driver, FrameAnalysisMetadata metadata)
        {
            Parallel.ForEach(driver.CellDiff, (cell, loopState, loopIndex)
                => CheckDiff(loopIndex, driver, metadata, _parameters));

            int diff = 0;

            for (int i = 0; i < driver.CellDiff.Length; i++)
            {
                diff += driver.CellDiff[i];

                if (_parameters.AnalysisMode && driver.CellDiff[i] == 1)
                {
                    HighlightCell(255, 0, 255, driver, metadata, i, _analysisBuffer);
                }
            }

            var detected = diff >= _cellCountThreshold;

            // Draw a bar across the frame; red indicates motion, green indicates no motion
            if (_parameters.AnalysisMode && diff > 0)
            {
                int x2 = (int)(((diff * 2f) / (driver.CellDiff.Length / 2f)) * (metadata.Width / 2f));
                (byte r, byte g) = detected ? ((byte)255, (byte)0) : ((byte)0, (byte)255);
                DrawIndicatorBlock(r, g, 0, 0, x2, 0, 10, _analysisBuffer, metadata);
            }

            if(_parameters.AnalysisMode)
            {
                _outputHandler?.Process(_fullRawFrameImageContext);
            }

            return detected;
        }

        private void CheckDiff(long cellIndex, FrameDiffDriver driver, FrameAnalysisMetadata metadata, ThreadSafeParameters parameters)
        {
            // FrameAnalysisMetadata and ThreadSafeParameters are structures; they are by-value copies and all fields are value-types which makes them thread safe

            int diff = 0;
            var rect = driver.CellRect[cellIndex];

            int x2 = rect.X + rect.Width;
            int y2 = rect.Y + rect.Height;

            for (var col = rect.X; col < x2; col++)
            {
                for (var row = rect.Y; row < y2; row++)
                {
                    var index = (col * metadata.Bpp) + (row * metadata.Stride);

                    // Disregard full-black cells in the mask bitmap
                    if (driver.FrameMask != null)
                    {
                        var rgbMask = driver.FrameMask[index] + driver.FrameMask[index + 1] + driver.FrameMask[index + 2];

                        if (rgbMask == 0)
                        {
                            continue;
                        }
                    }

                    byte r = driver.TestFrame[index];
                    byte g = driver.TestFrame[index + 1];
                    byte b = driver.TestFrame[index + 2];
                    int rgb1 = r + g + b;

                    r = driver.CurrentFrame[index];
                    g = driver.CurrentFrame[index + 1];
                    b = driver.CurrentFrame[index + 2];
                    int rgb2 = r + g + b;

                    int rgbDiff = Math.Abs(rgb2 - rgb1);
                    if (rgbDiff > parameters.RGBThreshold)
                    {
                        diff++;
                    }

                    if(!parameters.AnalysisMode)
                    {
                        // Check for early exit opportunity
                        if(diff >= parameters.CellPixelThreshold)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // No early exit for analysis purposes

                        // Output in grayscale based on strength of the diff (765 = 255 x 3)
                        r = Math.Min((byte)255, (byte)((rgbDiff / 765f) * 255.999f));
                        g = r;
                        b = r;

                        // Highlight cell corners
                        if ((col == rect.X || col == x2 - 1) && (row == rect.Y || row == y2 - 1))
                        {
                            r = 128;
                            g = 0;
                            b = 128;
                        }

                        _analysisBuffer[index] = r;
                        _analysisBuffer[index + 1] = g;
                        _analysisBuffer[index + 2] = b;
                    }
                }
            }

            driver.CellDiff[cellIndex] = (diff >= parameters.CellPixelThreshold) ? 1 : 0;
        }

        private struct ThreadSafeParameters
        {
            // Only use value-type fields (no properties, no reference types)
            public int RGBThreshold;
            public int CellPixelThreshold;
            public bool AnalysisMode;
        }
    }
}
