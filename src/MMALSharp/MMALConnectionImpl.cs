// <copyright file="MMALConnectionImpl.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Components;
using MMALSharp.Native;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a connection between two ports.
    /// </summary>
    public unsafe class MMALConnectionImpl : MMALObject
    {
        public MMALDownstreamComponent DownstreamComponent { get; set; }

        public MMALComponentBase UpstreamComponent { get; set; }

        /// <summary>
        /// The input port of this connection.
        /// </summary>
        public MMALPortBase InputPort { get; set; }

        /// <summary>
        /// The output port of this connection.
        /// </summary>
        public MMALPortBase OutputPort { get; set; }
        
        public MMALPoolImpl ConnectionPool { get; set; }

        /// <summary>
        /// Monitor lock for connection callback method.
        /// </summary>
        protected static object ConnectionLock = new object();
        
        #region Connection struct wrapper properties

        /// <summary>
        /// Name of this connection.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)(*this.Ptr).Name);

        /// <summary>
        /// Indicates whether this connection is enabled.
        /// </summary>
        public bool Enabled => (*this.Ptr).IsEnabled == 1;

        /// <summary>
        /// Flags passed during the create call (Read Only). A bitwise combination of Connection flags values.
        /// </summary>
        public uint Flags => (*this.Ptr).Flags;

        /// <summary>
        /// Time in microseconds taken to setup the connection.
        /// </summary>                          
        public long TimeSetup => (*this.Ptr).TimeSetup;

        /// <summary>
        /// Time in microseconds taken to enable the connection.
        /// </summary>
        public long TimeEnable => (*this.Ptr).TimeEnable;

        /// <summary>
        /// Time in microseconds taken to disable the connection.
        /// </summary>
        public long TimeDisable => (*this.Ptr).TimeDisable;

        #endregion

        internal MMALConnection.MMAL_CONNECTION_CALLBACK_T NativeCallback;

        /// <summary>
        /// Native pointer to the connection that this object represents.
        /// </summary>
        internal MMAL_CONNECTION_T* Ptr { get; set; }

        protected MMALConnectionImpl(MMAL_CONNECTION_T* ptr, MMALPortBase output, MMALPortBase input, MMALDownstreamComponent inputComponent, MMALComponentBase outputComponent, bool useCallback)
        {
            this.Ptr = ptr;
            this.OutputPort = output;
            this.InputPort = input;
            this.DownstreamComponent = inputComponent;
            this.UpstreamComponent = outputComponent;

            if (useCallback)
            {
                this.ConfigureConnectionCallback(output, input);
            }

            this.Enable();

            if (useCallback)
            {
                this.OutputPort.SendAllBuffers();
            }
        }

        public virtual void ManagedConnectionCallback(MMALBufferImpl buffer)
        {
            MMALLog.Logger.Debug("Inside Managed connection callback");
        }

        public override string ToString()
        {
            return $"Component connection - Upstream component: {this.UpstreamComponent.Name} on port {this.OutputPort.Name} Downstream component: {this.DownstreamComponent.Name} on port {this.InputPort.Name}";
        }

        public override void Dispose()
        {
            MMALLog.Logger.Debug("Disposing connection.");
            this.Destroy();
            base.Dispose();
        }

        /// <summary>
        /// Facility to create a connection between two port objects.
        /// </summary>
        /// <param name="output">The output port of the connection.</param>
        /// <param name="input">The input port of the connection.</param>
        /// <param name="inputComponent">The managed instance of the component we are connecting to.</param>
        /// <param name="useCallback">When set to true, enable the connection callback delegate (adversely affects performance).</param>
        /// <returns>A new managed connection object.</returns>
        internal static MMALConnectionImpl CreateConnection(MMALPortBase output, MMALPortBase input, MMALDownstreamComponent inputComponent, bool useCallback)
        {
            IntPtr ptr = IntPtr.Zero;

            if (useCallback)
            {
                MMALCheck(MMALConnection.mmal_connection_create(&ptr, output.Ptr, input.Ptr, MMALConnection.MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT), "Unable to create connection");
            }
            else
            {
                MMALCheck(MMALConnection.mmal_connection_create(&ptr, output.Ptr, input.Ptr, MMALConnection.MMAL_CONNECTION_FLAG_TUNNELLING | MMALConnection.MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT), "Unable to create connection");
            }

            return new MMALConnectionImpl((MMAL_CONNECTION_T*)ptr, output, input, inputComponent, output.ComponentReference, useCallback);
        }

        /// <summary>
        /// Enable a connection. The format of the two ports must have been committed before calling this function, although note that on creation, 
        /// the connection automatically copies and commits the output port's format to the input port.
        /// </summary>
        internal void Enable()
        {
            if (!this.Enabled)
            {
                MMALCheck(MMALConnection.mmal_connection_enable(this.Ptr), "Unable to enable connection");
            }
        }

        /// <summary>
        /// Disable a connection.
        /// </summary>
        internal void Disable()
        {
            if (this.Enabled)
            {
                MMALCheck(MMALConnection.mmal_connection_disable(this.Ptr), "Unable to disable connection");
            }
        }

        /// <summary>
        /// Destroy a connection. Release an acquired reference on a connection. Only actually destroys the connection when the last reference is 
        /// being released. The actual destruction of the connection will start by disabling it, if necessary. Any pool, queue, and so on owned by 
        /// the connection shall then be destroyed.
        /// </summary>
        internal void Destroy()
        {
            // Cleaning port pools for sanity.
            this.UpstreamComponent.CleanPortPools();
            this.DownstreamComponent.CleanPortPools();

            MMALCheck(MMALConnection.mmal_connection_destroy(this.Ptr), "Unable to destroy connection");
        }

        /// <summary>
        /// Represents the native callback method for a connection between two ports.
        /// </summary>
        /// <param name="connection">The native pointer to a MMAL_CONNECTION_T struct.</param>
        internal virtual int NativeConnectionCallback(MMAL_CONNECTION_T* connection)
        {
            lock (MMALConnectionImpl.ConnectionLock)
            {
                MMALLog.Logger.Debug("Inside native connection callback");

                var queue = new MMALQueueImpl(connection->Queue);
                var bufferImpl = queue.GetBuffer();

                if (bufferImpl != null)
                {
                    if (MMALCameraConfig.Debug)
                    {
                        bufferImpl.PrintProperties();
                    }

                    if (bufferImpl.Length > 0)
                    {
                        this.ManagedConnectionCallback(bufferImpl);
                    }

                    this.InputPort.SendBuffer(bufferImpl);
                }
                else
                {
                    queue = new MMALQueueImpl(connection->Pool->Queue);
                    bufferImpl = queue.GetBuffer();

                    if (bufferImpl != null)
                    {
                        if (MMALCameraConfig.Debug)
                        {
                            bufferImpl.PrintProperties();
                        }

                        if (bufferImpl.Length > 0)
                        {
                            this.ManagedConnectionCallback(bufferImpl);
                        }

                        this.OutputPort.SendBuffer(bufferImpl);
                    }
                    else
                    {
                        MMALLog.Logger.Debug("Buffer could not be obtained by connection callback");
                    }
                }
            }

            return (int)connection->Flags;
        }
        
        private void ConfigureConnectionCallback(MMALPortBase output, MMALPortBase input)
        {
            output.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            input.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);

            this.NativeCallback = new MMALConnection.MMAL_CONNECTION_CALLBACK_T(this.NativeConnectionCallback);
            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

            this.Ptr->Callback = ptrCallback;

            if (output.BufferPool == null)
            {
                output.BufferPool = new MMALPoolImpl(output);
            }
        }
    }
}
