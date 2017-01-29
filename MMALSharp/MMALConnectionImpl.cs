using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    public unsafe class MMALConnectionImpl : MMALObject
    {
        public MMAL_CONNECTION_T* Ptr { get; set; }
        public MMALPortBase InputPort { get; set; }
        public MMALPortBase OutputPort { get; set; }

        #region Connection struct wrapper properties

        public string Name {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)(*this.Ptr).name);
            }
        }
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

        #endregion

        protected MMALConnectionImpl(MMAL_CONNECTION_T* ptr, MMALPortBase output, MMALPortBase input)
        {            
            this.Ptr = ptr;            
            this.OutputPort = output;
            this.InputPort = input;
            this.Enable();
        }

        internal static MMALConnectionImpl CreateConnection(MMALPortBase output, MMALPortBase input)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALConnection.mmal_connection_create(&ptr, output.Ptr, input.Ptr, MMALConnection.MMAL_CONNECTION_FLAG_TUNNELLING | MMALConnection.MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT), "Unable to create connection");
            
            return new MMALConnectionImpl((MMAL_CONNECTION_T*)ptr, output, input);
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

        public override void Dispose()
        {
            Console.WriteLine("Disposing connection.");
            this.Destroy();
            base.Dispose();
        }
    }
}
