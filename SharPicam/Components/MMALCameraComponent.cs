using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALPortExtensions;
using static SharPicam.MMALParameterHelpers;
using System.IO;

namespace SharPicam.Components
{
    public unsafe class MMALCameraComponent : MMALComponentBase
    {        
        public static int MMAL_CAMERA_PREVIEW_PORT = 0;
        public static int MMAL_CAMERA_VIDEO_PORT = 1;
        public static int MMAL_CAMERA_CAPTURE_PORT = 2;
        
        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, 0, this.Control.Ptr);

            this.Control.ObjName = "Control port";

            var previewPort = this.Outputs.ElementAt(MMAL_CAMERA_PREVIEW_PORT);
            previewPort.ObjName = "Preview port";
            
            var videoPort = this.Outputs.ElementAt(MMAL_CAMERA_VIDEO_PORT);
            videoPort.ObjName = "Video port";

            var stillPort = this.Outputs.ElementAt(MMAL_CAMERA_CAPTURE_PORT);
            stillPort.ObjName = "Still port";

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
        
            this.Control.SetCameraConfig(camConfig);
            this.Control.SetControlParameters(new MMALCameraParameters());
                     
            previewPort.Ptr->format->encoding = MMALEncodings.MMAL_ENCODING_OPAQUE;
            previewPort.Ptr->format->encodingVariant = MMALEncodings.MMAL_ENCODING_I420;
            previewPort.Ptr->format->es->video.width = 2592u;
            previewPort.Ptr->format->es->video.height = 1944u;

            Console.WriteLine("Commit preview");

            previewPort.Commit();
            previewPort.FullCopy(videoPort.Ptr->format);
                        
            Console.WriteLine("Commit video");

            videoPort.Commit();

            if (videoPort.Ptr->bufferNum < 3)
                videoPort.Ptr->bufferNum = 3;

            stillPort.Ptr->format->encoding = MMALEncodings.MMAL_ENCODING_I420;
            stillPort.Ptr->format->encodingVariant = MMALEncodings.MMAL_ENCODING_I420;
                        
            stillPort.Ptr->format->es->video.width = MMALUtil.VCOS_ALIGN_UP(640u, 32);
            stillPort.Ptr->format->es->video.height = MMALUtil.VCOS_ALIGN_UP(480u, 32);
            stillPort.Ptr->format->es->video.crop.x = 0;
            stillPort.Ptr->format->es->video.crop.y = 0;
            stillPort.Ptr->format->es->video.crop.width = 2592;
            stillPort.Ptr->format->es->video.crop.height = 1944;
            stillPort.Ptr->format->es->video.frameRate.num = 0;
            stillPort.Ptr->format->es->video.frameRate.den = 1;
            
            Console.WriteLine("Commit still");

            stillPort.Commit();

            stillPort.BufferSize = Math.Max(stillPort.BufferSize, stillPort.BufferSizeMin);
            stillPort.BufferNum = stillPort.BufferNumRecommended;
 
            Console.WriteLine("Enable component");

            this.EnableComponent();

            Console.WriteLine("Create pool");

            this.BufferPool = new MMALPoolImpl(stillPort);
        }

        public void CameraControlCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside camera control callback");

            if (buffer.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {
                Console.WriteLine("Buffer cmd == MMAL_EVENT_PARAMETER_CHANGED");

                var data = (MMAL_EVENT_PARAMETER_CHANGED_T*)buffer.Data;
                Console.WriteLine("Header id = " + data->hdr.id);

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
        
        public void CameraBufferCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside camera buffer callback");

            Console.WriteLine("Buffer alloc size " + buffer.AllocSize);
            Console.WriteLine("Buffer length " + buffer.Length);
            Console.WriteLine("Buffer offset " + buffer.Offset);
            buffer.Properties();

            var bufferStream = buffer.DataStream();

            if(bufferStream.Item1 != MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED &&
               bufferStream.Item1 != MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                try
                {
                    using (var fileStream = File.OpenWrite("/home/pi/test.jpg"))
                    {
                        fileStream.Seek(fileStream.Length, SeekOrigin.Begin);
                        bufferStream.Item2.CopyTo(fileStream);                        
                    }                                            
                }
                catch
                {
                    Console.WriteLine("Could not open file for writing");
                }                
            }
            else
            {
                using (var fileStream = File.Create("/home/pi/test.jpg", 4000))
                {
                    
                    if (bufferStream.Item2 != null)
                    {
                        bufferStream.Item2.Seek(0, SeekOrigin.Begin);
                        bufferStream.Item2.CopyTo(fileStream);
                        bufferStream.Item2.Dispose();
                    }
                    else
                    {
                        Console.WriteLine("Data stream null.");
                    }
                }
            }

                      
            
        }

    }
}
