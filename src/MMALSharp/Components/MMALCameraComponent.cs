// <copyright file="MMALCameraComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Defines a set of sensor modes that allow to configure how the raw image data is sent to the GPU before further processing. See wiki on GitHub for more information.
    /// </summary>
    /// <remarks>
    /// https://github.com/techyian/MMALSharp/wiki/OmniVision-OV5647-Camera-Module
    /// https://github.com/techyian/MMALSharp/wiki/Sony-IMX219-Camera-Module
    /// https://www.raspberrypi.org/forums/viewtopic.php?t=85714
    /// </remarks>
    public enum MMALSensorMode
    {
        /// <summary>
        /// Automatic mode (default).
        /// </summary>
        Mode0,

        /// <summary>
        /// 1080p cropped mode.
        /// </summary>
        Mode1,

        /// <summary>
        /// 4:3 ratio.
        /// </summary>
        Mode2,

        /// <summary>
        /// 4:3 ratio (low FPS with OV5647).
        /// </summary>
        Mode3,

        /// <summary>
        /// 2x2 binned 4:3.
        /// </summary>
        Mode4,

        /// <summary>
        /// 2x2 binned 16:9.
        /// </summary>
        Mode5,

        /// <summary>
        /// High FPS. Ratio and resolution depend on camera module.
        /// </summary>
        Mode6,

        /// <summary>
        /// VGA high FPS.
        /// </summary>
        Mode7
    }
    
    /// <summary>
    /// Represents a camera component.
    /// </summary>
    public sealed class MMALCameraComponent : MMALComponentBase, ICameraComponent
    {
        /// <summary>
        /// The output port number of the camera's preview port.
        /// </summary>
        public const int MMALCameraPreviewPort = 0;

        /// <summary>
        /// The output port number of the camera's video port.
        /// </summary>
        public const int MMALCameraVideoPort = 1;

        /// <summary>
        /// The output port number of the camera's still port.
        /// </summary>
        public const int MMALCameraStillPort = 2;

        /// <summary>
        /// Managed reference to the Preview port of the camera.
        /// </summary>
        public IOutputPort PreviewPort { get; set; }

        /// <summary>
        /// Managed reference to the Video port of the camera.
        /// </summary>
        public IOutputPort VideoPort { get; set; }

        /// <summary>
        /// Managed reference to the Still port of the camera.
        /// </summary>
        public IOutputPort StillPort { get; set; }

        /// <summary>
        /// Camera Info component. This is used to provide detailed info about the camera itself.
        /// </summary>
        public ICameraInfoComponent CameraInfo { get; set; }
        
        /// <summary>
        /// Initialises a new MMALCameraComponent.
        /// </summary>
        public unsafe MMALCameraComponent()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            this.Outputs.Add(new OutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, Guid.NewGuid()));
            this.Outputs.Add(new VideoPort((IntPtr)(&(*this.Ptr->Output[1])), this, Guid.NewGuid()));
            this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[2])), this, Guid.NewGuid()));
            
            if (this.CameraInfo == null)
            {
                this.SetSensorDefaults();
            }
            
            this.PreviewPort = this.Outputs[MMALCameraPreviewPort];
            this.VideoPort = this.Outputs[MMALCameraVideoPort];
            this.StillPort = this.Outputs[MMALCameraStillPort];

            /*
             * Stereoscopic mode is only supported with the compute module as it requires two camera modules to work.
             * I have added the code in for consistency with Raspistill, however this project currently only supports one camera module
             * and therefore will not work if enabled.
             * See: https://www.raspberrypi.org/forums/viewtopic.php?p=600720
            */
            this.PreviewPort.SetStereoMode(MMALCameraConfig.StereoMode);
            this.VideoPort.SetStereoMode(MMALCameraConfig.StereoMode);
            this.StillPort.SetStereoMode(MMALCameraConfig.StereoMode);

            this.Control.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, 0);
            
            var eventRequest = new MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(
                new MMAL_PARAMETER_HEADER_T(MMALParametersCommon.MMAL_PARAMETER_CHANGE_EVENT_REQUEST, Marshal.SizeOf<MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T>()),
                MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS, 1);

            if (MMALCameraConfig.SetChangeEventRequest)
            {
                this.Control.SetChangeEventRequest(eventRequest);
            }
        }

        /// <summary>
        /// Disposes of the current component, and frees any native resources still in use by it.
        /// </summary>
        public override void Dispose()
        {
            if (this.CameraInfo != null && this.CameraInfo.CheckState())
            {
                this.CameraInfo.DestroyComponent();
            }
            
            base.Dispose();
        }

        /// <summary>
        /// Prints a summary of the ports and the resolution associated with this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.LogInformation($"    Still Width: {this.StillPort.Resolution.Width}. Video Height: {this.StillPort.Resolution.Height}");
            MMALLog.Logger.LogInformation($"    Video Width: {this.VideoPort.Resolution.Width}. Video Height: {this.VideoPort.Resolution.Height}");
            MMALLog.Logger.LogInformation($"    Max Width: {this.CameraInfo.MaxWidth}. Video Height: {this.CameraInfo.MaxHeight}");
        }

        /// <summary>
        /// Call to initialise the camera component.
        /// </summary>
        /// <param name="stillCaptureHandler">A capture handler when capturing raw image frames from the camera's still port (no encoder attached).</param>
        /// <param name="videoCaptureHandler">A capture handler when capturing raw video from the camera's video port (no encoder attached).</param>
        public void Initialise(IOutputCaptureHandler stillCaptureHandler = null, IOutputCaptureHandler videoCaptureHandler = null)
        {
            this.DisableComponent();
            
            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(
                new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                MMALCameraConfig.Resolution.Width,
                                                                MMALCameraConfig.Resolution.Height,
                                                                0,
                                                                1,
                                                                MMALCameraConfig.Resolution.Width,
                                                                MMALCameraConfig.Resolution.Height,
                                                                3 + Math.Max(0, (new MMAL_RATIONAL_T(MMALCameraConfig.Framerate).Num - 30) / 10),
                                                                0,
                                                                0,
                                                                MMALCameraConfig.ClockMode);
                        
            this.SetCameraConfig(camConfig);

            MMALLog.Logger.LogDebug("Camera config set");

            this.Control.Start();

            MMALLog.Logger.LogDebug("Configuring camera parameters.");

            this.SetCameraParameters();

            this.InitialisePreview();
            this.InitialiseVideo(videoCaptureHandler);
            this.InitialiseStill(stillCaptureHandler);

            this.EnableComponent();

            MMALLog.Logger.LogDebug("Camera component configured.");
        }

        private void SetSensorDefaults()
        {
            this.CameraInfo = new MMALCameraInfoComponent();
        }
                
        /// <summary>
        /// Initialises the camera's preview component using the user defined width/height for the video port.
        /// </summary>
        private void InitialisePreview()
        {
            var portConfig = new MMALPortConfig(
                MMALCameraConfig.Encoding,
                MMALCameraConfig.EncodingSubFormat,
                width: MMALCameraConfig.Resolution.Width,
                height: MMALCameraConfig.Resolution.Height,
                framerate: MMALCameraConfig.Framerate);

            MMALLog.Logger.LogDebug("Commit preview");

            this.PreviewPort.Configure(portConfig, null, null);

            // Use Raspistill values.
            if (MMALCameraConfig.ShutterSpeed > 6000000)
            {
                this.PreviewPort.SetFramerateRange(new MMAL_RATIONAL_T(5, 1000), new MMAL_RATIONAL_T(166, 1000));
            }
            else if (MMALCameraConfig.ShutterSpeed > 1000000)
            {
                this.PreviewPort.SetFramerateRange(new MMAL_RATIONAL_T(166, 1000), new MMAL_RATIONAL_T(999, 1000));
            }
        }

        /// <summary>
        /// Initialises the camera's video port using the width, height and encoding as specified by the user.
        /// </summary>
        /// <param name="handler">The capture handler to associate with this port.</param>
        private void InitialiseVideo(IOutputCaptureHandler handler)
        {
            int currentWidth = MMALCameraConfig.Resolution.Width;
            int currentHeight = MMALCameraConfig.Resolution.Height;

            if (currentWidth == 0 || currentWidth > this.CameraInfo.MaxWidth)
            {
                currentWidth = this.CameraInfo.MaxWidth;
            }

            if (currentHeight == 0 || currentHeight > this.CameraInfo.MaxHeight)
            {
                currentHeight = this.CameraInfo.MaxHeight;
            }

            var portConfig = new MMALPortConfig(
                MMALCameraConfig.Encoding, 
                MMALCameraConfig.EncodingSubFormat,
                width: currentWidth,
                height: currentHeight,
                framerate: MMALCameraConfig.Framerate,
                bufferNum: Math.Max(MMALCameraConfig.UserBufferNum, Math.Max(this.VideoPort.BufferNumRecommended, 3)),
                bufferSize: Math.Max(MMALCameraConfig.UserBufferSize, Math.Max(this.VideoPort.BufferSizeRecommended, this.VideoPort.BufferSizeMin)),
                crop: new Rectangle(0, 0, currentWidth, currentHeight));

            MMALLog.Logger.LogDebug("Commit video");

            this.VideoPort.Configure(portConfig, null, handler);

            // Use Raspistill values.
            if (MMALCameraConfig.ShutterSpeed > 6000000)
            {
                this.VideoPort.SetFramerateRange(new MMAL_RATIONAL_T(5, 1000), new MMAL_RATIONAL_T(166, 1000));
            }
            else if (MMALCameraConfig.ShutterSpeed > 1000000)
            {
                this.VideoPort.SetFramerateRange(new MMAL_RATIONAL_T(167, 1000), new MMAL_RATIONAL_T(999, 1000));
            }
        }

        /// <summary>
        /// Initialises the camera's still port using the width, height and encoding as specified by the user.
        /// </summary>
        /// <param name="handler">The capture handler to associate with the still port.</param>
        private void InitialiseStill(IOutputCaptureHandler handler)
        {
            int currentWidth = MMALCameraConfig.Resolution.Width;
            int currentHeight = MMALCameraConfig.Resolution.Height;

            if (currentWidth == 0 || currentWidth > this.CameraInfo.MaxWidth)
            {
                currentWidth = this.CameraInfo.MaxWidth;
            }

            if (currentHeight == 0 || currentHeight > this.CameraInfo.MaxHeight)
            {
                currentHeight = this.CameraInfo.MaxHeight;
            }

            MMALCameraConfig.Resolution = new Resolution(currentWidth, currentHeight);

            MMALPortConfig portConfig = null;

            if (MMALCameraConfig.Encoding == MMALEncoding.RGB32 ||
                MMALCameraConfig.Encoding == MMALEncoding.RGB24 ||
                MMALCameraConfig.Encoding == MMALEncoding.RGB16)
            {
                MMALLog.Logger.LogWarning("Encoding set to RGB. Setting width padding to multiple of 16.");

                var resolution = MMALCameraConfig.Resolution.Pad(16, 16);
                var encoding = MMALCameraConfig.Encoding;

                try
                {
                    if (!this.StillPort.RgbOrderFixed())
                    {
                        MMALLog.Logger.LogWarning("Using old firmware. Setting encoding to BGR24");
                        encoding = MMALEncoding.BGR24;
                    }
                }
                catch
                {
                    MMALLog.Logger.LogWarning("Using old firmware. Setting encoding to BGR24");
                    encoding = MMALEncoding.BGR24;
                }
                
                portConfig = new MMALPortConfig(
                    encoding,
                    encoding,
                    width: resolution.Width,
                    height: resolution.Height,
                    framerate: MMALCameraConfig.Framerate,
                    bufferNum: Math.Max(MMALCameraConfig.UserBufferNum, Math.Max(this.StillPort.BufferNumRecommended, 3)),
                    bufferSize: Math.Max(MMALCameraConfig.UserBufferSize, Math.Max(this.StillPort.BufferSizeRecommended, this.StillPort.BufferSizeMin)),
                    crop: new Rectangle(0, 0, currentWidth, currentHeight));
            }
            else
            {
                var resolution = MMALCameraConfig.Resolution.Pad();

                portConfig = new MMALPortConfig(
                    MMALCameraConfig.Encoding, 
                    MMALCameraConfig.EncodingSubFormat,
                    width: resolution.Width,
                    height: resolution.Height,
                    framerate: MMALCameraConfig.Framerate,
                    bufferNum: Math.Max(MMALCameraConfig.UserBufferNum, Math.Max(this.StillPort.BufferNumRecommended, 3)),
                    bufferSize: Math.Max(MMALCameraConfig.UserBufferSize, Math.Max(this.StillPort.BufferSizeRecommended, this.StillPort.BufferSizeMin)),
                    crop: new Rectangle(0, 0, currentWidth, currentHeight));
            }
            
            MMALLog.Logger.LogDebug("Commit still");
            this.StillPort.Configure(portConfig, null, handler);

            // Use Raspistill values.
            if (MMALCameraConfig.ShutterSpeed > 6000000)
            {
                this.StillPort.SetFramerateRange(new MMAL_RATIONAL_T(5, 1000), new MMAL_RATIONAL_T(166, 1000));
            }
            else if (MMALCameraConfig.ShutterSpeed > 1000000)
            {
                this.StillPort.SetFramerateRange(new MMAL_RATIONAL_T(167, 1000), new MMAL_RATIONAL_T(999, 1000));
            }
        }

        private void SetCameraParameters()
        {
            this.SetSensorMode(MMALCameraConfig.SensorMode);
            this.SetSaturation(MMALCameraConfig.Saturation);
            this.SetSharpness(MMALCameraConfig.Sharpness);
            this.SetContrast(MMALCameraConfig.Contrast);
            this.SetBrightness(MMALCameraConfig.Brightness);
            this.SetISO(MMALCameraConfig.ISO);
            this.SetVideoStabilisation(MMALCameraConfig.VideoStabilisation);
            this.SetExposureCompensation(MMALCameraConfig.ExposureCompensation);
            this.SetExposureMode(MMALCameraConfig.ExposureMode);
            this.SetExposureMeteringMode(MMALCameraConfig.ExposureMeterMode);
            this.SetAwbMode(MMALCameraConfig.AwbMode);
            this.SetAwbGains(MMALCameraConfig.AwbGainsR, MMALCameraConfig.AwbGainsB);
            this.SetImageFx(MMALCameraConfig.ImageFx);
            this.SetColourFx(MMALCameraConfig.ColourFx);
            this.SetRotation(MMALCameraConfig.Rotation);
            this.SetShutterSpeed(MMALCameraConfig.ShutterSpeed);
            this.SetStatsPass(MMALCameraConfig.StatsPass);
            this.SetDRC(MMALCameraConfig.DrcLevel);
            this.SetFlips(MMALCameraConfig.Flips);
            this.SetZoom(MMALCameraConfig.ROI);
            this.SetBurstMode(MMALCameraConfig.StillBurstMode);
            this.SetAnalogGain(MMALCameraConfig.AnalogGain);
            this.SetDigitalGain(MMALCameraConfig.DigitalGain);
        }
    }
}
