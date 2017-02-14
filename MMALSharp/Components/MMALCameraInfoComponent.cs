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
    public class MMALCameraInfoComponent : MMALComponentBase
    {
        public string SensorName { get; set; }
        public uint MaxWidth { get; set; }
        public uint MaxHeight { get; set; }

        public MMALCameraInfoComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA_INFO)
        {
            this.Initialize();
        }

        public unsafe override void Initialize()
        {
            this.SensorName = "OV5647";
            this.MaxWidth = 2592;
            this.MaxHeight = 1944;
          
            MMAL_PARAMETER_CAMERA_INFO_T param = new MMAL_PARAMETER_CAMERA_INFO_T();
            param.hdr = new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO, (uint)Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INFO_T>());
            
            try
            {
                //Keep OV5647 defaults.
                MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, &param.hdr), "");              
            }
            catch
            {
                //Running on newer firmware - default to first camera found.                
                var ptr = Marshal.AllocHGlobal(152);
                var str = (MMAL_PARAMETER_HEADER_T*)ptr;

                str->id = (uint)MMALParametersCamera.MMAL_PARAMETER_CAMERA_INFO;
                //Calculated Marshalled size of MMAL_PARAMETER_CAMERA_INFO_V2_T hence the static value here.
                str->size = 152;
                                
                try
                {
                    MMALCheck(MMALPort.mmal_port_parameter_get(this.Control.Ptr, str), "Unable to get camera info for newer firmware.");
                    
                    var p = (IntPtr)(str);
                    
                    var s = Marshal.PtrToStructure<MMAL_PARAMETER_CAMERA_INFO_V2_T>(p);
                
                    if(s.cameras != null && s.cameras.Length > 0)
                    {                                
                        this.SensorName = s.cameras[0].cameraName;
                        this.MaxHeight = s.cameras[0].maxHeight;
                        this.MaxWidth = s.cameras[0].maxWidth;                        
                    }                        
                }
                catch(Exception e)
                {                    
                    //Something went wrong, continue with OV5647 defaults.                    
                    Console.WriteLine("Died" + e.Message);
                }
            }                        
        }
    }
}
