// <copyright file="MMALCameraComponent.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    public enum MMALSensorMode
    {
        Mode0,
        Mode1,
        Mode2,
        Mode3,
        Mode4,
        Mode5,
        Mode6,
        Mode7
    }
    
    /// <summary>
    /// Represents a camera component.
    /// </summary>
    public sealed class MMALCameraComponent : MMALComponentBase
    {
        public const int MMALCameraPreviewPort = 0;
        public const int MMALCameraVideoPort = 1;
        public const int MMALCameraStillPort = 2;

        /// <summary>
        /// Managed reference to the Preview port of the camera.
        /// </summary>
        public MMALPortImpl PreviewPort { get; set; }

        /// <summary>
        /// Managed reference to the Video port of the camera.
        /// </summary>
        public MMALPortImpl VideoPort { get; set; }

        /// <summary>
        /// Managed reference to the Still port of the camera.
        /// </summary>
        public MMALPortImpl StillPort { get; set; }

        /// <summary>
        /// Camera Info component. This is used to provide detailed info about the camera itself.
        /// </summary>
        public MMALCameraInfoComponent CameraInfo { get; set; }

        /// <summary>
        /// Initialises a new MMALCameraComponent.
        /// </summary>
        public MMALCameraComponent()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            if (this.CameraInfo == null)
            {
                this.SetSensorDefaults();
            }

            if (this.Outputs.Count == 0)
            {
                throw new PiCameraError("Camera doesn't have any output ports.");
            }

            this.Control.ObjName = "Control port";

            this.PreviewPort = this.Outputs[MMALCameraPreviewPort];
            this.PreviewPort.ObjName = "Preview port";

            this.VideoPort = this.Outputs[MMALCameraVideoPort];
            this.VideoPort.ObjName = "Video port";

            this.StillPort = this.Outputs[MMALCameraStillPort];
            this.StillPort.ObjName = "Still port";

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

        public override void Dispose()
        {
            this.CameraInfo?.DestroyComponent();
            base.Dispose();
        }

        /// <summary>
        /// Prints a summary of the ports and the resolution associated with this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Still Width: {this.StillPort.Resolution.Width}. Video Height: {this.StillPort.Resolution.Height}");
            MMALLog.Logger.Info($"    Video Width: {this.VideoPort.Resolution.Width}. Video Height: {this.VideoPort.Resolution.Height}");
            MMALLog.Logger.Info($"    Max Width: {this.CameraInfo.MaxWidth}. Video Height: {this.CameraInfo.MaxHeight}");
        }

        internal void SetSensorDefaults()
        {
            this.CameraInfo = new MMALCameraInfoComponent();
        }

        /// <summary>
        /// This is the camera's control port callback function. The callback is used if
        /// MMALCameraConfig.SetChangeEventRequest is set to true.
        /// </summary>
        /// <seealso cref="MMALCameraConfig.SetChangeEventRequest" />
        /// <param name="buffer">The buffer header being sent from MMAL.</param>
        /// <param name="port">The managed control port instance.</param>
        internal unsafe void CameraControlCallback(MMALBufferImpl buffer, MMALPortBase port)
        {
            if (buffer.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {
                var data = (MMAL_EVENT_PARAMETER_CHANGED_T*)buffer.Data;

                if (data->Hdr.Id == MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS)
                {
                    var settings = (MMAL_PARAMETER_CAMERA_SETTINGS_T*)data;

                    MMALLog.Logger.Debug($"Analog gain num {settings->AnalogGain.Num}");
                    MMALLog.Logger.Debug($"Analog gain den {settings->AnalogGain.Den}");
                    MMALLog.Logger.Debug($"Exposure {settings->Exposure}");
                    MMALLog.Logger.Debug($"Focus position {settings->FocusPosition}");
                }
            }
            else if (buffer.Cmd == MMALEvents.MMAL_EVENT_ERROR)
            {
                MMALLog.Logger.Info("No data received from sensor. Check all connections, including the Sunny one on the camera board");
            }
            else
            {
                MMALLog.Logger.Info("Received unexpected camera control callback event");
            }
        }

        internal void Initialise()
        {
            this.DisableComponent();
            
            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(
                new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                this.CameraInfo.MaxWidth,
                                                                this.CameraInfo.MaxHeight,
                                                                0,
                                                                1,
                                                                MMALCameraConfig.VideoResolution.Width,
                                                                MMALCameraConfig.VideoResolution.Height,
                                                                3 + Math.Max(0, (MMALCameraConfig.VideoFramerate.Num - 30) / 10),
                                                                0,
                                                                0,
                                                                MMALCameraConfig.ClockMode);
                        
            this.SetCameraConfig(camConfig);

            MMALLog.Logger.Debug("Camera config set");

            this.Control.EnablePort((Action<MMALBufferImpl, MMALPortBase>)this.CameraControlCallback);

            MMALLog.Logger.Debug("Configuring camera parameters.");

            this.SetCameraParameters();

            this.InitialisePreview();
            this.InitialiseVideo();
            this.InitialiseStill();

            this.EnableComponent();

            MMALLog.Logger.Debug("Camera component configured.");
        }

        /// <summary>
        /// Initialises the camera's preview component using the user defined width/height for the video port.
        /// </summary>
        internal void InitialisePreview()
        {
            this.PreviewPort.Resolution = new Resolution(MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height).Pad();
            this.PreviewPort.Crop = new Rectangle(0, 0, MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height);
            this.PreviewPort.FrameRate = new MMAL_RATIONAL_T(0, 1);
            this.PreviewPort.NativeEncodingType = MMALCameraConfig.PreviewEncoding.EncodingVal;
            this.PreviewPort.NativeEncodingSubformat = MMALCameraConfig.PreviewSubformat.EncodingVal;
            
            MMALLog.Logger.Debug("Commit preview");

            this.PreviewPort.Commit();

            this.PreviewPort.BufferNum = Math.Max(
                this.PreviewPort.BufferNumRecommended,
                this.PreviewPort.BufferNumMin);

            this.PreviewPort.BufferSize = Math.Max(
                this.PreviewPort.BufferSizeRecommended,
                this.PreviewPort.BufferSizeMin);
        }

        /// <summary>
        /// Initialises the camera's video port using the width, height and encoding as specified by the user.
        /// </summary>
        internal void InitialiseVideo()
        {
            if (MMALCameraConfig.VideoResolution.Width == 0 || MMALCameraConfig.VideoResolution.Width > this.CameraInfo.MaxWidth)
            {
                MMALCameraConfig.VideoResolution.Width = this.CameraInfo.MaxWidth;
            }

            if (MMALCameraConfig.VideoResolution.Height == 0 || MMALCameraConfig.VideoResolution.Height > this.CameraInfo.MaxHeight)
            {
                MMALCameraConfig.VideoResolution.Height = this.CameraInfo.MaxHeight;
            }

            this.VideoPort.Resolution = new Resolution(MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height).Pad();
            this.VideoPort.Crop = new Rectangle(0, 0, MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height);
            this.VideoPort.FrameRate = MMALCameraConfig.VideoFramerate;
            this.VideoPort.NativeEncodingType = MMALCameraConfig.VideoEncoding.EncodingVal;
            this.VideoPort.NativeEncodingSubformat = MMALCameraConfig.VideoSubformat.EncodingVal;
            
            MMALLog.Logger.Debug("Commit video");
            this.VideoPort.Commit();

            this.VideoPort.BufferNum = Math.Max(
                this.VideoPort.BufferNumRecommended,
                this.VideoPort.BufferNumMin);

            this.VideoPort.BufferSize = Math.Max(
                this.VideoPort.BufferSizeRecommended,
                this.VideoPort.BufferSizeMin);
        }

        /// <summary>
        /// Initialises the camera's still port using the width, height and encoding as specified by the user.
        /// </summary>
        internal void InitialiseStill()
        {
            if (MMALCameraConfig.StillResolution.Width == 0 || MMALCameraConfig.StillResolution.Width > this.CameraInfo.MaxWidth)
            {
                MMALCameraConfig.StillResolution.Width = this.CameraInfo.MaxWidth;
            }

            if (MMALCameraConfig.StillResolution.Height == 0 || MMALCameraConfig.StillResolution.Height > this.CameraInfo.MaxHeight)
            {
                MMALCameraConfig.StillResolution.Height = this.CameraInfo.MaxHeight;
            }
            
            if (MMALCameraConfig.StillEncoding == MMALEncoding.RGB32 ||
                MMALCameraConfig.StillEncoding == MMALEncoding.RGB24 ||
                MMALCameraConfig.StillEncoding == MMALEncoding.RGB16)
            {
                MMALLog.Logger.Warn("Encoding set to RGB. Setting width padding to multiple of 16.");

                try
                {
                    if (!this.StillPort.RgbOrderFixed())
                    {
                        MMALLog.Logger.Warn("Using old firmware. Setting encoding to BGR24");
                        this.StillPort.NativeEncodingType = MMALEncoding.BGR24.EncodingVal;
                    }
                }
                catch
                {
                    MMALLog.Logger.Warn("Using old firmware. Setting encoding to BGR24");
                    this.StillPort.NativeEncodingType = MMALEncoding.BGR24.EncodingVal;
                }

                this.StillPort.NativeEncodingSubformat = 0;

                this.StillPort.Commit();

                this.StillPort.Resolution =
                    new Resolution(MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height).Pad(16, 16);
                
                this.StillPort.Crop = new Rectangle(0, 0, MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height);

                // Indicates variable framerate
                this.StillPort.FrameRate = new MMAL_RATIONAL_T(0, 1);
            }
            else
            {
                this.StillPort.NativeEncodingType = MMALCameraConfig.StillEncoding.EncodingVal;
                this.StillPort.NativeEncodingSubformat = MMALCameraConfig.StillSubFormat.EncodingVal;

                this.StillPort.Commit();

                this.StillPort.Resolution =
                    new Resolution(MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height).Pad();
                
                this.StillPort.Crop = new Rectangle(0, 0, MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height);
                this.StillPort.FrameRate = MMALCameraConfig.StillFramerate;
            }

            MMALLog.Logger.Debug("Commit still");
            this.StillPort.Commit();

            this.StillPort.BufferNum = Math.Max(
                this.StillPort.BufferNumRecommended,
                this.StillPort.BufferNumMin);

            this.StillPort.BufferSize = Math.Max(
                this.StillPort.BufferSizeRecommended,
                this.StillPort.BufferSizeMin);
        }

        internal void SetCameraParameters()
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
        }
    }
}
