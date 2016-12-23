using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public static class MMALPortHelper
    {
        public static Dictionary<int, Type> ParameterHelper = new Dictionary<int, Type> {
            
        };
            

    }

    public unsafe class MMALPortImpl : MMALObject
    {
        public MMAL_PORT_T* Ptr { get; set; }
        public MMAL_COMPONENT_T* Comp { get; set; }
        public string Name { get; set; }
        public bool Enabled {
            get {
                return (*Ptr).isEnabled == 1;
            }
        }
        public uint BufferNumMin {
            get {
                return (*Ptr).bufferNumMin;
            }
        }
        public uint BufferSizeMin {
            get {
                return (*Ptr).bufferSizeMin;
            }
        }
        public uint BufferAlignmentMin {
            get {
                return (*Ptr).bufferAlignmentMin;
            }
        }
        public uint BufferNumRecommended {
            get {
                return (*Ptr).bufferNumRecommended;
            }
        }
        public uint BufferSizeRecommended {
            get {
                return (*Ptr).bufferSizeRecommended;
            }
        }
        public uint BufferNum {
            get {
                return (*Ptr).bufferNum;
            }
            set {
                Ptr->bufferNum = value;
            }
        }
        public uint BufferSize {
            get {
                return (*Ptr).bufferSize;
            }
            set {
                Ptr->bufferSize = value;
            }
        }
        
        public MMALPortImpl(MMAL_PORT_T* ptr)
        {
            this.Ptr = ptr;
            this.Comp = (*ptr).component;
            this.Name = Marshal.PtrToStringAnsi((IntPtr)((*ptr).name));          
        }

        public void EnablePort()
        {            
            //MMALCheck(MMALPort.mmal_port_enable(this.Ptr,))
        }

        public void DisablePort()
        {

        }

        public T GetParameter<T>(int key) where T : struct
        {
            
        }

        public void SetParameter<T>(int id, T value) where T : struct
        {

        }

    }
}
