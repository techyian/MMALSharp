using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

namespace SharPicam
{
    public unsafe class MMALConnectionImpl : MMALObject, IDisposable
    {
        public MMAL_CONNECTION_T* Ptr { get; set; }

        public bool Enabled {
            get
            {
                return (*this.Ptr).isEnabled == 1;
            }
        }

        public uint Flags
        {
            get
            {
                return (*this.Ptr).flags;
            }
        }

        public string Name { get; set; }
        public long TimeSetup
        {
            get
            {
                return (*this.Ptr).timeSetup;
            }
        }
        public long TimeEnable
        {
            get
            {
                return (*this.Ptr).timeEnable;
            }
        }
        public long TimeDisable
        {
            get
            {
                return (*this.Ptr).timeDisable;
            }
        }

        protected MMALConnectionImpl(MMAL_CONNECTION_T* ptr)
        {            
            this.Ptr = ptr;
            this.Name = Marshal.PtrToStringAnsi((IntPtr)(*this.Ptr).name);
            this.Enable();
        }

        public static MMALConnectionImpl CreateConnection(MMALPortImpl output, MMALPortImpl input)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALConnection.mmal_connection_create(&ptr, output.Ptr, input.Ptr, MMALConnection.MMAL_CONNECTION_FLAG_TUNNELLING | MMALConnection.MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT), "Unable to create connection");
            
            return new MMALConnectionImpl((MMAL_CONNECTION_T*)ptr);
        }

        public void Enable()
        {
            if (!Enabled)
                MMALCheck(MMALConnection.mmal_connection_enable(this.Ptr), "Unable to enable connection");
        }

        public void Disable()
        {
            if (Enabled)
                MMALCheck(MMALConnection.mmal_connection_disable(this.Ptr), "Unable to disable connection");
        }

        public void Destroy()
        {
            MMALCheck(MMALConnection.mmal_connection_destroy(this.Ptr), "Unable to destroy connection");
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing connection.");
            this.Destroy();
        }
    }
}
