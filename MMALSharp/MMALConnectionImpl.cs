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
    /// <summary>
    /// Represents a connection between two ports
    /// </summary>
    public unsafe class MMALConnectionImpl : MMALObject
    {
        /// <summary>
        /// Native pointer to the connection that this object represents
        /// </summary>
        internal MMAL_CONNECTION_T* Ptr { get; set; }

        /// <summary>
        /// The input port of this connection
        /// </summary>
        public MMALPortBase InputPort { get; set; }

        /// <summary>
        /// The output port of this connection
        /// </summary>
        public MMALPortBase OutputPort { get; set; }

        #region Connection struct wrapper properties

        /// <summary>
        /// Name of this connection
        /// </summary>
        public string Name {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)(*this.Ptr).Name);
            }
        }

        /// <summary>
        /// Indicates whether this connection is enabled
        /// </summary>
        public bool Enabled {
            get
            {
                return (*this.Ptr).IsEnabled == 1;
            }
        }

        /// <summary>
        /// Flags passed during the create call (Read Only). A bitwise combination of Connection flags values.
        /// </summary>
        public uint Flags
        {
            get
            {
                return (*this.Ptr).Flags;
            }
        }

        /// <summary>
        /// Time in microseconds taken to setup the connection.
        /// </summary>                          
        public long TimeSetup
        {
            get
            {
                return (*this.Ptr).TimeSetup;
            }
        }

        /// <summary>
        /// Time in microseconds taken to enable the connection.
        /// </summary>
        public long TimeEnable
        {
            get
            {
                return (*this.Ptr).TimeEnable;
            }
        }

        /// <summary>
        /// Time in microseconds taken to disable the connection.
        /// </summary>
        public long TimeDisable
        {
            get
            {
                return (*this.Ptr).TimeDisable;
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

        /// <summary>
        /// Facility to create a connection between two port objects
        /// </summary>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static MMALConnectionImpl CreateConnection(MMALPortBase output, MMALPortBase input)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALConnection.mmal_connection_create(&ptr, output.Ptr, input.Ptr, MMALConnection.MMAL_CONNECTION_FLAG_TUNNELLING | MMALConnection.MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT), "Unable to create connection");
            
            return new MMALConnectionImpl((MMAL_CONNECTION_T*)ptr, output, input);
        }

        /// <summary>
        /// Enable a connection. The format of the two ports must have been committed before calling this function, although note that on creation, 
        /// the connection automatically copies and commits the output port's format to the input port.
        /// </summary>
        internal void Enable()
        {
            if (!Enabled)
                MMALCheck(MMALConnection.mmal_connection_enable(this.Ptr), "Unable to enable connection");
        }

        /// <summary>
        /// Disable a connection.
        /// </summary>
        internal void Disable()
        {
            if (Enabled)
                MMALCheck(MMALConnection.mmal_connection_disable(this.Ptr), "Unable to disable connection");
        }

        /// <summary>
        /// Destroy a connection. Release an acquired reference on a connection. Only actually destroys the connection when the last reference is 
        /// being released. The actual destruction of the connection will start by disabling it, if necessary. Any pool, queue, and so on owned by 
        /// the connection shall then be destroyed.
        /// </summary>
        internal void Destroy()
        {
            MMALCheck(MMALConnection.mmal_connection_destroy(this.Ptr), "Unable to destroy connection");
        }

        public override void Dispose()
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Disposing connection.");
            this.Destroy();
            base.Dispose();
        }
    }
}
