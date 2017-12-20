using MMALSharp.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using MMALSharp.Handlers;


namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a camera component.
    /// </summary>
    public unsafe class MMALCameraComponent : MMALComponentBase
    {        
        public const int MMALCameraPreviewPort = 0;
        public const int MMALCameraVideoPort = 1;
        public const int MMALCameraStillPort = 2;
        
        /// <summary>
        /// Managed reference to the Preview port of the camera
        /// </summary>
        public MMALPortImpl PreviewPort { get; set; }

        /// <summary>
        /// Managed reference to the Video port of the camera
        /// </summary>
        public MMALPortImpl VideoPort { get; set; }

        /// <summary>
        /// Managed reference to the Still port of the camera
        /// </summary>
        public MMALPortImpl StillPort { get; set; }

        /// <summary>
        /// Camera Info component. This is used to provide detailed info about the camera itself
        /// </summary>
        public MMALCameraInfoComponent CameraInfo { get; set; }

        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            if (this.CameraInfo == null)
                this.SetSensorDefaults();
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

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

            var eventRequest = new MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCommon.MMAL_PARAMETER_CHANGE_EVENT_REQUEST, Marshal.SizeOf<MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T>()),
                MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS, 1);

            if (MMALCameraConfig.SetChangeEventRequest)
                this.Control.SetChangeEventRequest(eventRequest);
            
            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                this.CameraInfo.MaxWidth,
                                                                this.CameraInfo.MaxHeight,
                                                                0,
                                                                1,
                                                                MMALCameraConfig.VideoResolution.Width,
                                                                MMALCameraConfig.VideoResolution.Height,
                                                                3 + Math.Max(0, (MMALCameraConfig.VideoFramerate.Num - 30) / 10),
                                                                0,
                                                                0,
                                                                MMALCameraConfig.ClockMode
                                                              );
            
            MMALLog.Logger.Debug("Camera config set");

            this.SetCameraConfig(camConfig);

            this.Control.EnablePort((Action<MMALBufferImpl, MMALPortBase>) CameraControlCallback);

            this.Initialise();

            MMALLog.Logger.Debug("Camera component configured.");
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
        /// <param name="buffer"></param>
        /// <param name="port"></param>
        internal void CameraControlCallback(MMALBufferImpl buffer, MMALPortBase port)
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
            this.InitialisePreview();
            this.InitialiseVideo();
            this.InitialiseStill();
        }

        /// <summary>
        /// Initialises the camera's preview component using the user defined width/height for the video port
        /// </summary>
        internal void InitialisePreview()
        {
            var vFormat = new MMAL_VIDEO_FORMAT_T(
                MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoResolution.Width, 32),
                MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoResolution.Height, 16),
                new MMAL_RECT_T(0, 0, MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height),
                new MMAL_RATIONAL_T(0, 1),
                this.PreviewPort.Ptr->Format->es->video.par,
                this.PreviewPort.Ptr->Format->es->video.colorSpace
            );
            
            this.PreviewPort.Ptr->Format->encoding = MMALCameraConfig.PreviewEncoding.EncodingVal;
            this.PreviewPort.Ptr->Format->encodingVariant = MMALCameraConfig.PreviewSubformat.EncodingVal;
            this.PreviewPort.Ptr->Format->es->video = vFormat;

            MMALLog.Logger.Debug("Commit preview");

            this.PreviewPort.Commit();
        }

        /// <summary>
        /// Initialises the camera's video port using the width, height and encoding as specified by the user
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
            
            var vFormat = new MMAL_VIDEO_FORMAT_T(
                MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoResolution.Width, 32),
                MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoResolution.Height, 16),
                new MMAL_RECT_T(0, 0, MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height),
                MMALCameraConfig.VideoFramerate,
                this.VideoPort.Ptr->Format->es->video.par,
                this.VideoPort.Ptr->Format->es->video.colorSpace
            );

            this.VideoPort.Ptr->Format->encoding = MMALCameraConfig.VideoEncoding.EncodingVal;
            this.VideoPort.Ptr->Format->encodingVariant = MMALCameraConfig.VideoSubformat.EncodingVal;
            this.VideoPort.Ptr->Format->es->video = vFormat;

            MMALLog.Logger.Debug("Commit video");

            this.VideoPort.Commit();

            this.VideoPort.Ptr->BufferNum = Math.Max(this.VideoPort.BufferNumRecommended,
                this.VideoPort.BufferNumMin);

            this.VideoPort.Ptr->BufferSize = Math.Max(this.VideoPort.BufferSizeRecommended,
                this.VideoPort.BufferSizeMin);
        }

        /// <summary>
        /// Initialises the camera's still port using the width, height and encoding as specified by the user
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
            
            var vFormat = new MMAL_VIDEO_FORMAT_T();
            
            if (MMALCameraConfig.StillEncoding == MMALEncoding.RGB32 ||
                MMALCameraConfig.StillEncoding == MMALEncoding.RGB24 ||
                MMALCameraConfig.StillEncoding == MMALEncoding.RGB16)
            {
                MMALLog.Logger.Warn("Encoding set to RGB. Setting width padding to multiple of 16.");

                vFormat = new MMAL_VIDEO_FORMAT_T(
                    MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.StillResolution.Width, 16),
                    MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.StillResolution.Height, 16),
                    new MMAL_RECT_T(0, 0, MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height),
                    new MMAL_RATIONAL_T(0, 1),
                    this.StillPort.Ptr->Format->es->video.par,
                    this.StillPort.Ptr->Format->es->video.colorSpace
                );

                try
                {
                    if (!this.StillPort.RgbOrderFixed())
                    {
                        MMALLog.Logger.Warn("Using old firmware. Setting encoding to BGR24");
                        this.StillPort.Ptr->Format->encoding = MMALEncoding.BGR24.EncodingVal;
                    }
                }
                catch
                {
                    MMALLog.Logger.Warn("Using old firmware. Setting encoding to BGR24");
                    this.StillPort.Ptr->Format->encoding = MMALEncoding.BGR24.EncodingVal;
                }
                this.StillPort.Ptr->Format->encodingVariant = 0;
            }
            else
            {
                vFormat = new MMAL_VIDEO_FORMAT_T(
                    MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.StillResolution.Width, 32),
                    MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.StillResolution.Height, 16),
                    new MMAL_RECT_T(0, 0, MMALCameraConfig.StillResolution.Width, MMALCameraConfig.StillResolution.Height),
                    MMALCameraConfig.StillFramerate,
                    this.StillPort.Ptr->Format->es->video.par,
                    this.StillPort.Ptr->Format->es->video.colorSpace
                );

                this.StillPort.Ptr->Format->encoding = MMALCameraConfig.StillEncoding.EncodingVal;
                this.StillPort.Ptr->Format->encodingVariant = MMALCameraConfig.StillSubFormat.EncodingVal;
            }

            this.StillPort.Ptr->Format->es->video = vFormat;

            MMALLog.Logger.Debug("Commit still");
            
            this.StillPort.Commit();
            
            this.StillPort.Ptr->BufferNum = Math.Max(this.StillPort.BufferNumRecommended,
                this.StillPort.BufferNumMin);

            this.StillPort.Ptr->BufferSize = Math.Max(this.StillPort.BufferSizeRecommended,
                this.StillPort.BufferSizeMin);
        }

        internal void SetCameraParameters()
        {
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
        
        public override void Dispose()
        {
            this.CameraInfo?.DestroyComponent();
            base.Dispose();
        }

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.CameraInfo.MaxWidth}. Height: {this.CameraInfo.MaxHeight}");
        }
    }
}
