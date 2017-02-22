using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

            var ptr1 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_T>());
            var str1 = (MMAL_PARAMETER_HEADER_T*)ptr1;

            str1->id = MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO;      
            //Deliberately undersize to check if running on older firmware.      
            str1->size = Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_V2_T>() - 4;
            
            try
            {
                //If succeeds, keep OV5647 defaults.
                MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, str1), "");
            }
            catch
            {
                Marshal.FreeHGlobal(ptr1);

                //Running on newer firmware - default to first camera found.                
                //152
                var ptr2 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_V2_T>());
                var str2 = (MMAL_PARAMETER_HEADER_T*)ptr2;

                str2->id = MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO;
                str2->size = Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_V2_T>();

                try
                {
                    MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, str2), "Unable to get camera info for newer firmware.");

                    var p = (IntPtr)(str2);

                    var s = Marshal.PtrToStructure<MMAL_PARAMETER_CAMERA_INFO_V2_T>(p);

                    if (s.cameras != null && s.cameras.Length > 0)
                    {
                        this.SensorName = s.cameras[0].cameraName;
                        this.MaxHeight = s.cameras[0].maxHeight;
                        this.MaxWidth = s.cameras[0].maxWidth;
                    }
                }
                catch (Exception e)
                {
                    //Something went wrong, continue with OV5647 defaults.                    
                    Console.WriteLine("Died" + e.Message);
                }
            }
        }
        
    }
}
