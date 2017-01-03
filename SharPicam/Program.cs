using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SharPicam.Native.MMALParametersCamera;

namespace SharPicam
{
    public unsafe class Program
    {
        public static void Main(string[] args)
        {
            BcmHost.bcm_host_init();

            var camera = new MMALCameraComponent();
            var nullSink = new MMALNullSinkComponent();

            var previewPort = camera.Outputs.ElementAt(MMALCameraComponent.MMAL_CAMERA_PREVIEW_PORT);
            var videoPort = camera.Outputs.ElementAt(MMALCameraComponent.MMAL_CAMERA_VIDEO_PORT);
            var stillPort = camera.Outputs.ElementAt(MMALCameraComponent.MMAL_CAMERA_CAPTURE_PORT);

            var nullSinkInputPort = nullSink.Inputs.ElementAt(0);
            var nullSinkConnection = MMALConnectionImpl.CreateConnection(previewPort.Ptr, nullSinkInputPort.Ptr);
            
            stillPort.EnablePort(camera.CameraBufferCallback);

            Console.WriteLine("Shutter speed set");
            camera.Control.SetParameter(MMAL_PARAMETER_SHUTTER_SPEED, 0);

            var length = camera.CameraPool.Queue.QueueLength();

            Console.WriteLine("Buffer queue length " + length);

            for (int i = 0; i < length; i++)
            {
                var buffer = camera.CameraPool.Queue.GetBuffer();
                stillPort.SendBuffer(buffer.Ptr);
            }

            Console.WriteLine("Attempt capture");
            stillPort.SetParameter(MMAL_PARAMETER_CAPTURE, 1);          
            Console.WriteLine("Sent port address " + ((IntPtr)stillPort.Ptr).ToString());

            Thread.Sleep(3000);

            stillPort.DisablePort();

            Thread.Sleep(3000);

            BcmHost.bcm_host_deinit();            
        }
    }
}
