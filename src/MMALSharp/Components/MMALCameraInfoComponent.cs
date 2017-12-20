using MMALSharp.Native;
using System;
using System.Runtime.InteropServices;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a Camera Info component
    /// </summary>
    public unsafe class MMALCameraInfoComponent : MMALComponentBase
    {
        /// <summary>
        /// The sensor name of the camera
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Maximum width supported by the sensor
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Maximum height supported by the sensor
        /// </summary>
        public int MaxHeight { get; set; }

        public MMALCameraInfoComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA_INFO)
        {
            this.SensorName = "OV5647";
            this.MaxWidth = 2592;
            this.MaxHeight = 1944;

            IntPtr ptr1 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_T>());
            var str1 = (MMAL_PARAMETER_HEADER_T*)ptr1;

            str1->Id = MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO;      
            //Deliberately undersize to check if running on older firmware.      
            str1->Size = Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_T>();
                                    
            try
            {
                //If succeeds, keep OV5647 defaults.
                MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, str1), "");
            }
            catch
            {
                Marshal.FreeHGlobal(ptr1);

                //Running on newer firmware - default to first camera found.
                IntPtr ptr2 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_V2_T>());
                var str2 = (MMAL_PARAMETER_HEADER_T*)ptr2;

                str2->Id = MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO;
                str2->Size = Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_V2_T>();
                                
                try
                {
                    MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, str2), "Unable to get camera info for newer firmware.");

                    var p = (IntPtr)str2;

                    var s = Marshal.PtrToStructure<MMAL_PARAMETER_CAMERA_INFO_V2_T>(p);

                    if (s.Cameras != null && s.Cameras.Length > 0)
                    {
                        this.SensorName = s.Cameras[0].CameraName;
                        this.MaxHeight = s.Cameras[0].MaxHeight;
                        this.MaxWidth = s.Cameras[0].MaxWidth;
                    }
                }
                catch
                {
                    //Something went wrong, continue with OV5647 defaults.                    
                    MMALLog.Logger.Warn("Could not determine firmware version. Continuing with OV5647 defaults");
                }
            }
        }

        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Camera info");
        }
    }
}
