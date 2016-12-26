using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALCameraComponent : MMALComponentBase
    {
        public MMALPoolImpl CameraPool { get; set; }

        public static int MMAL_CAMERA_PREVIEW_PORT = 0;
        public static int MMAL_CAMERA_VIDEO_PORT = 1;
        public static int MMAL_CAMERA_CAPTURE_PORT = 2;
        
        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            this.Control.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, 0);

            var previewPort = this.Outputs.ElementAt(MMAL_CAMERA_PREVIEW_PORT);
            var videoPort = this.Outputs.ElementAt(MMAL_CAMERA_VIDEO_PORT);
            var stillPort = this.Outputs.ElementAt(MMAL_CAMERA_CAPTURE_PORT);

            this.Control.EnablePort(CameraControlCallback);

            var camConfig = new MMAL_PARAMETER_CAMERA_CONFIG_T(new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, (uint)Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CONFIG_T>()),
                                                                2592u,
                                                                1944u,
                                                                0u,
                                                                1u,
                                                                300u,
                                                                300u,
                                                                3u,
                                                                0u,
                                                                0u,
                                                                MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T.MMAL_PARAM_TIMESTAMP_MODE_RESET_STC
                                                                );

            Console.WriteLine("Camera config set");
            //this.Control.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG, camConfig);

            Console.WriteLine("Setting encoding.");
                                    
            (*previewPort.Ptr).format->encoding = MMALEncodings.MMAL_ENCODING_OPAQUE;
            (*previewPort.Ptr).format->encodingVariant = MMALEncodings.MMAL_ENCODING_I420;

            Console.WriteLine("Setting ES.");

            (*previewPort.Ptr).format->es->video.width = 2592u;
            (*previewPort.Ptr).format->es->video.height = 1944u;

            Console.WriteLine("Commit preview");

            previewPort.Commit();
            previewPort.FullCopy((*videoPort.Ptr).format);

            Console.WriteLine("Commit video");

            videoPort.Commit();

            (*stillPort.Ptr).format->encoding = MMALEncodings.MMAL_ENCODING_I420;
            (*stillPort.Ptr).format->encodingVariant = MMALEncodings.MMAL_ENCODING_I420;

            (*stillPort.Ptr).format->es->video.width = 2592u;
            (*stillPort.Ptr).format->es->video.height = 1944u;
            (*stillPort.Ptr).format->es->video.crop.x = 0;
            (*stillPort.Ptr).format->es->video.crop.y = 0;
            (*stillPort.Ptr).format->es->video.crop.width = 300;
            (*stillPort.Ptr).format->es->video.crop.height = 300;
            (*stillPort.Ptr).format->es->video.frameRate.num = 0;
            (*stillPort.Ptr).format->es->video.frameRate.den = 1;

            Console.WriteLine("Commit still");

            stillPort.Commit();

            stillPort.BufferSize = Math.Max(stillPort.BufferSize, stillPort.BufferSizeMin);
            stillPort.BufferNum = stillPort.BufferNumRecommended;

            Console.WriteLine("Enable component");

            this.EnableComponent();

            Console.WriteLine("Create pool");

            this.CameraPool = new MMALPoolImpl(stillPort);
        }

        public void CameraControlCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside camera control callback");
        }
        
        public void CameraBufferCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside camera buffer callback");
        }

    }
}
