using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a camera component
    /// </summary>
    public unsafe class MMALCameraComponent : MMALComponentBase
    {        
        private const int MMALCameraPreviewPort = 0;
        private const int MMALCameraVideoPort = 1;
        private const int MMALCameraStillPort = 2;
        
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
            this.Initialize();
        }

        public override void Initialize()
        {
            if (this.CameraInfo == null)
                this.SetSensorDefaults();
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            this.PreviewPort.SetStereoMode(MMALCameraConfigImpl.Config.StereoMode);
            this.VideoPort.SetStereoMode(MMALCameraConfigImpl.Config.StereoMode);
            this.StillPort.SetStereoMode(MMALCameraConfigImpl.Config.StereoMode);

            MMALParameterHelpers.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, 0, this.Control.Ptr);

            this.Control.ObjName = "Control port";

            this.PreviewPort = this.Outputs.ElementAt(MMALCameraPreviewPort);
            this.PreviewPort.ObjName = "Preview port";

            this.VideoPort = this.Outputs.ElementAt(MMALCameraVideoPort);
            this.VideoPort.ObjName = "Video port";

            this.StillPort = this.Outputs.ElementAt(MMALCameraStillPort);
            this.StillPort.ObjName = "Still port";

            var eventRequest = new MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCommon.MMAL_PARAMETER_CHANGE_EVENT_REQUEST, Marshal.SizeOf<MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T>()),
                                                                         MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS, 1);

            if (MMALCameraConfigImpl.Config.SetChangeEventRequest)
                this.Control.SetChangeEventRequest(eventRequest);

            this.Control.EnablePort(CameraControlCallback, null);
            
            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                this.CameraInfo.MaxWidth,
                                                                this.CameraInfo.MaxHeight,
                                                                0,
                                                                1,
                                                                this.CameraInfo.MaxWidth,
                                                                this.CameraInfo.MaxHeight,
                                                                3,
                                                                0,
                                                                0,
                                                                MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T.MMAL_PARAM_TIMESTAMP_MODE_RESET_STC
                                                                );
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Camera config set");

            this.SetCameraConfig(camConfig);

            MMAL_VIDEO_FORMAT_T vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfigImpl.Config.StillWidth,
                                                                  MMALCameraConfigImpl.Config.StillHeight,
                                                                  new MMAL_RECT_T(0, 0, MMALCameraConfigImpl.Config.StillHeight, MMALCameraConfigImpl.Config.StillWidth),
                                                                  new MMAL_RATIONAL_T(0, 1),
                                                                  this.PreviewPort.Ptr->Format->Es->Video.Par,
                                                                  this.PreviewPort.Ptr->Format->Es->Video.ColorSpace);

            this.PreviewPort.Ptr->Format->Encoding = MMALCameraConfigImpl.Config.PreviewEncoding;
            this.PreviewPort.Ptr->Format->Es->Video = vFormat;
            
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Commit preview");

            this.PreviewPort.Commit();
            this.PreviewPort.FullCopy(this.VideoPort);

            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Commit video");

            this.VideoPort.Commit();

            if (this.VideoPort.Ptr->BufferNum < 3)
                this.VideoPort.Ptr->BufferNum = 3;

            this.StillPort.Ptr->Format->Encoding = MMALCameraConfigImpl.Config.StillEncoding;
            this.StillPort.Ptr->Format->EncodingVariant = MMALCameraConfigImpl.Config.StillEncodingSubFormat;

            //If user hasn't specified Width/Height, use highest resolution supported by sensor.
            if (MMALCameraConfigImpl.Config.StillWidth == 0)
                MMALCameraConfigImpl.Config.StillWidth = this.CameraInfo.MaxWidth;
            if (MMALCameraConfigImpl.Config.StillHeight == 0)
                MMALCameraConfigImpl.Config.StillHeight = this.CameraInfo.MaxHeight;
            
            if (MMALCameraConfigImpl.Config.StillWidth > this.CameraInfo.MaxWidth)
                MMALCameraConfigImpl.Config.StillWidth = MMALUtil.VCOS_ALIGN_UP(this.CameraInfo.MaxWidth, 16);
            if (MMALCameraConfigImpl.Config.StillHeight > this.CameraInfo.MaxHeight)
                MMALCameraConfigImpl.Config.StillHeight = MMALUtil.VCOS_ALIGN_UP(this.CameraInfo.MaxHeight, 16);

            vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfigImpl.Config.StillWidth,
                                                MMALCameraConfigImpl.Config.StillHeight,
                                                new MMAL_RECT_T(0, 0, MMALCameraConfigImpl.Config.StillHeight, MMALCameraConfigImpl.Config.StillWidth),
                                                new MMAL_RATIONAL_T(0, 1),
                                                this.StillPort.Ptr->Format->Es->Video.Par,
                                                this.StillPort.Ptr->Format->Es->Video.ColorSpace);

            this.StillPort.Ptr->Format->Es->Video = vFormat;

            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Commit still");

            this.StillPort.Commit();

            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Camera component configured.");            
        }
        
        public void SetSensorDefaults()
        {
            this.CameraInfo = new MMALCameraInfoComponent();                        
        }
        
        public void CameraControlCallback(MMALBufferImpl buffer, MMALPortBase port)
        {            
            if (buffer.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {                
                var data = (MMAL_EVENT_PARAMETER_CHANGED_T*)buffer.Data;
                
                if (data->Hdr.Id == MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS)
                {
                    var settings = (MMAL_PARAMETER_CAMERA_SETTINGS_T*)data;

                    Console.WriteLine("Analog gain num " + settings->AnalogGain.Num);
                    Console.WriteLine("Analog gain den " + settings->AnalogGain.Den);
                    Console.WriteLine("Exposure " + settings->Exposure);
                    Console.WriteLine("Focus position " + settings->FocusPosition);                    
                }

            }
            else if (buffer.Cmd == MMALEvents.MMAL_EVENT_ERROR)
            {
                Console.WriteLine("No data received from sensor. Check all connections, including the Sunny one on the camera board");
            }
            else
            {
                Console.WriteLine("Received unexpected camera control callback event");
            }            
        }

        public override void Dispose()
        {
            if (this.CameraInfo != null)
                this.CameraInfo.DestroyComponent();
            base.Dispose();
        }

    }
}
