using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALParameterHelpers;
using System.IO;

namespace MMALSharp.Components
{
    public unsafe class MMALCameraComponent : MMALComponentBase
    {        
        private const int MMAL_CAMERA_PREVIEW_PORT = 0;
        private const int MMAL_CAMERA_VIDEO_PORT = 1;
        private const int MMAL_CAMERA_CAPTURE_PORT = 2;
        
        public MMALPortImpl PreviewPort { get; set; }
        public MMALPortImpl VideoPort { get; set; }
        public MMALPortImpl StillPort { get; set; }
        
        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {        
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, 0, this.Control.Ptr);

            this.Control.ObjName = "Control port";
            
            this.PreviewPort = this.Outputs.ElementAt(MMAL_CAMERA_PREVIEW_PORT);                        
            this.PreviewPort.ObjName = "Preview port";
            
            this.VideoPort = this.Outputs.ElementAt(MMAL_CAMERA_VIDEO_PORT);
            this.VideoPort.ObjName = "Video port";

            this.StillPort = this.Outputs.ElementAt(MMAL_CAMERA_CAPTURE_PORT);
            this.StillPort.ObjName = "Still port";

            var eventRequest = new MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCommon.MMAL_PARAMETER_CHANGE_EVENT_REQUEST, (uint)Marshal.SizeOf<MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T>()),
                                                                         (uint)MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS, 1);

            //this.Control.SetChangeEventRequest(eventRequest);
                                                                                     
            this.Control.EnablePort(CameraControlCallback);

            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, (uint)Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                2592u,
                                                                1944u,
                                                                0u,
                                                                1u,
                                                                2592u,
                                                                1944u,
                                                                3u,
                                                                0u,
                                                                0u,
                                                                MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T.MMAL_PARAM_TIMESTAMP_MODE_RESET_STC
                                                                );
            
            Console.WriteLine("Camera config set");
        
            this.SetCameraConfig(camConfig);
                        
        }

        public override void Initialize()
        {
            this.PreviewPort.Ptr->format->encoding = MMALEncodings.MMAL_ENCODING_OPAQUE;
            this.PreviewPort.Ptr->format->es->video.width = 2592u;
            this.PreviewPort.Ptr->format->es->video.height = 1944u;

            Console.WriteLine("Commit preview");

            this.PreviewPort.Commit();
            this.PreviewPort.FullCopy(this.VideoPort);

            Console.WriteLine("Commit video");

            this.VideoPort.Commit();

            if (this.VideoPort.Ptr->bufferNum < 3)
                this.VideoPort.Ptr->bufferNum = 3;

            this.StillPort.Ptr->format->encoding = MMALEncodings.MMAL_ENCODING_OPAQUE;
            this.StillPort.Ptr->format->encodingVariant = MMALEncodings.MMAL_ENCODING_I420;

            this.StillPort.Ptr->format->es->video.width = MMALUtil.VCOS_ALIGN_UP(640u, 32);
            this.StillPort.Ptr->format->es->video.height = MMALUtil.VCOS_ALIGN_UP(480u, 32);
            this.StillPort.Ptr->format->es->video.crop.x = 0;
            this.StillPort.Ptr->format->es->video.crop.y = 0;
            this.StillPort.Ptr->format->es->video.crop.width = 2592;
            this.StillPort.Ptr->format->es->video.crop.height = 1944;
            this.StillPort.Ptr->format->es->video.frameRate.num = 0;
            this.StillPort.Ptr->format->es->video.frameRate.den = 1;

            Console.WriteLine("Commit still");
            this.StillPort.Commit();
       
        }
        
        public void CameraControlCallback(MMALBufferImpl buffer)
        {            
            if (buffer.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {                
                var data = (MMAL_EVENT_PARAMETER_CHANGED_T*)buffer.Data;
                
                if(data->hdr.id == MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS)
                {
                    var settings = (MMAL_PARAMETER_CAMERA_SETTINGS_T*)data;

                    Console.WriteLine("Analog gain num " + settings->analogGain.num);
                    Console.WriteLine("Analog gain den " + settings->analogGain.den);
                    Console.WriteLine("Exposure " + settings->exposure);
                    Console.WriteLine("Focus position " + settings->focusPosition);                    
                }

            }
            else if(buffer.Cmd == MMALEvents.MMAL_EVENT_ERROR)
            {
                Console.WriteLine("No data received from sensor. Check all connections, including the Sunny one on the camera board");
            }
            else
            {
                Console.WriteLine("Received unexpected camera control callback event");
            }            
        }
        
        public void StopCapture()
        {
            if(StillPort.Enabled)
                StillPort.SetImageCapture(false);
        }

        public byte[] CameraBufferCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside camera buffer callback");

            Console.WriteLine("Buffer alloc size " + buffer.AllocSize);
            Console.WriteLine("Buffer length " + buffer.Length);
            Console.WriteLine("Buffer offset " + buffer.Offset);
            
            return buffer.DataStream();                        
        }
        
    }
}
