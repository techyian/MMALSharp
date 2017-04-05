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
            if (this.Ptr->InputNum > 0)
            {
                for (int i = 0; i < this.Ptr->InputNum; i++)
                {
                    this.Inputs.Add(new MMALPortImpl(&(*this.Ptr->Input[i]), this));
                }
            }

            if (this.Ptr->OutputNum > 0)
            {
                for (int i = 0; i < this.Ptr->OutputNum; i++)
                {
                    this.Outputs.Add(new MMALPortImpl(&(*this.Ptr->Output[i]), this));
                }
            }

            this.Initialize();
        }

        public override void Initialize()
        {
            if (this.CameraInfo == null)
                this.SetSensorDefaults();
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            this.Control.ObjName = "Control port";

            this.PreviewPort = this.Outputs.ElementAt(MMALCameraPreviewPort);
            this.PreviewPort.ObjName = "Preview port";

            this.VideoPort = this.Outputs.ElementAt(MMALCameraVideoPort);
            this.VideoPort.ObjName = "Video port";

            this.StillPort = this.Outputs.ElementAt(MMALCameraStillPort);
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

            this.Control.EnablePort(CameraControlCallback);
            
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
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Camera config set");

            this.SetCameraConfig(camConfig);
                        
            MMAL_VIDEO_FORMAT_T vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfig.StillWidth,
                                                                  MMALCameraConfig.StillHeight,
                                                                  new MMAL_RECT_T(0, 0, MMALCameraConfig.StillWidth, MMALCameraConfig.StillHeight),
                                                                  new MMAL_RATIONAL_T(0, 1),
                                                                  this.PreviewPort.Ptr->Format->Es->Video.Par,
                                                                  this.PreviewPort.Ptr->Format->Es->Video.ColorSpace);

            this.PreviewPort.Ptr->Format->Encoding = MMALCameraConfig.PreviewEncoding.EncodingVal;
            this.PreviewPort.Ptr->Format->Es->Video = vFormat;
            
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Commit preview");

            this.PreviewPort.Commit();
                        
            if (MMALCameraConfig.VideoWidth == 0 || MMALCameraConfig.VideoWidth > this.CameraInfo.MaxWidth)
                MMALCameraConfig.VideoWidth = this.CameraInfo.MaxWidth;
            if (MMALCameraConfig.VideoHeight == 0 || MMALCameraConfig.VideoHeight > this.CameraInfo.MaxHeight)
                MMALCameraConfig.VideoHeight = this.CameraInfo.MaxHeight;

            vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfig.VideoWidth,
                                                                  MMALCameraConfig.VideoHeight,
                                                                  new MMAL_RECT_T(0, 0, MMALCameraConfig.VideoWidth, MMALCameraConfig.VideoHeight),
                                                                  new MMAL_RATIONAL_T(0, 1),
                                                                  this.VideoPort.Ptr->Format->Es->Video.Par,
                                                                  this.VideoPort.Ptr->Format->Es->Video.ColorSpace);

            this.VideoPort.Ptr->Format->Encoding = MMALCameraConfig.VideoEncoding.EncodingVal;
            this.VideoPort.Ptr->Format->EncodingVariant = MMALCameraConfig.VideoSubformat.EncodingVal;
            this.VideoPort.Ptr->Format->Es->Video = vFormat;


            if (MMALCameraConfig.Debug)
                Console.WriteLine("Commit video");

            this.VideoPort.Commit();

            if (this.VideoPort.Ptr->BufferNum < 3)
                this.VideoPort.Ptr->BufferNum = 3;
                        
            //If user hasn't specified Width/Height, or one which is too high, use highest resolution supported by sensor.
            if (MMALCameraConfig.StillWidth == 0 || MMALCameraConfig.StillWidth > this.CameraInfo.MaxWidth)
                MMALCameraConfig.StillWidth = this.CameraInfo.MaxWidth;
            if (MMALCameraConfig.StillHeight == 0 || MMALCameraConfig.StillHeight > this.CameraInfo.MaxHeight)
                MMALCameraConfig.StillHeight = this.CameraInfo.MaxHeight;
            
            vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfig.StillWidth,
                                                MMALCameraConfig.StillHeight,
                                                new MMAL_RECT_T(0, 0, MMALCameraConfig.StillWidth, MMALCameraConfig.StillHeight),
                                                new MMAL_RATIONAL_T(0, 1),
                                                this.StillPort.Ptr->Format->Es->Video.Par,
                                                this.StillPort.Ptr->Format->Es->Video.ColorSpace);

            this.StillPort.Ptr->Format->Encoding = MMALCameraConfig.StillEncoding.EncodingVal;
            this.StillPort.Ptr->Format->EncodingVariant = MMALCameraConfig.StillEncodingSubFormat.EncodingVal;
            this.StillPort.Ptr->Format->Es->Video = vFormat;

            if (MMALCameraConfig.Debug)
                Console.WriteLine("Commit still");

            this.StillPort.Commit();

            if (this.StillPort.Ptr->BufferNum < 3)
                this.StillPort.Ptr->BufferNum = 3;

            if (MMALCameraConfig.Debug)
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
