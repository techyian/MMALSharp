using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public class MMALCameraComponent : MMALComponentBase
    {
        public const int MMAL_CAMERA_PREVIEW_PORT = 0;
        public const int MMAL_CAMERA_VIDEO_PORT = 1;
        public const int MMAL_CAMERA_CAPTURE_PORT = 2;
        
        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA)
        {
            if (this.Outputs.Count == 0)
                throw new PiCameraError("Camera doesn't have any output ports.");

            var previewPort = this.Outputs.ElementAt(MMAL_CAMERA_PREVIEW_PORT);
            var videoPort = this.Outputs.ElementAt(MMAL_CAMERA_VIDEO_PORT);
            var stillPort = this.Outputs.ElementAt(MMAL_CAMERA_CAPTURE_PORT);

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

        }

        public void CameraControlCallback(MMALBufferImpl buffer)
        {

        }
        
    }
}
