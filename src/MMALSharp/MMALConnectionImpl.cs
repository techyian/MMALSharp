// <copyright file="MMALConnectionImpl.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a connection between two ports.
    /// </summary>
    public unsafe class MMALConnectionImpl : MMALObject, IConnection
    {
        /// <summary>
        /// The connection callback handler.
        /// </summary>
        public IConnectionCallbackHandler CallbackHandler { get; internal set; }

        /// <summary>
        /// The pool of buffer headers in this connection.
        /// </summary>
        public IBufferPool ConnectionPool { get; set; }

        /// <summary>
        /// The downstream component associated with the connection.
        /// </summary>
        public IDownstreamComponent DownstreamComponent { get; }

        /// <summary>
        /// The upstream component associated with the connection.
        /// </summary>
        public IComponent UpstreamComponent { get; }

        /// <summary>
        /// The input port of this connection.
        /// </summary>
        public IInputPort InputPort { get; }

        /// <summary>
        /// The output port of this connection.
        /// </summary>
        public IOutputPort OutputPort { get; }
        
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

        private MMALConnection.MMAL_CONNECTION_CALLBACK_T NativeCallback;

        /// <summary>
        /// Native pointer to the connection that this object represents.
        /// </summary>
        public MMAL_CONNECTION_T* Ptr { get; }

        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Creates a new instance of <see cref="MMALConnectionImpl"/>.
        /// </summary>
        /// <param name="ptr">The native connection pointer.</param>
        /// <param name="output">The upstream component's output port.</param>
        /// <param name="input">The downstream component's input port.</param>
        /// <param name="inputComponent">The upstream component.</param>
        /// <param name="outputComponent">The downstream component.</param>
        /// <param name="useCallback">
        /// Configure the connection to intercept native callbacks. Note: will adversely impact performance. In addition, this will implicitly enable
        /// zero copy functionality on both the source and sink ports.
        /// </param>
        protected MMALConnectionImpl(MMAL_CONNECTION_T* ptr, IOutputPort output, IInputPort input, IDownstreamComponent inputComponent, IComponent outputComponent, bool useCallback)
        {
            this.Ptr = ptr;
            this.OutputPort = output;
            this.InputPort = input;
            this.DownstreamComponent = inputComponent;
            this.UpstreamComponent = outputComponent;

            if (useCallback)
            {
                this.CallbackHandler = new DefaultConnectionCallbackHandler(this);
                this.ConfigureConnectionCallback(output, input);
            }

            this.Enable();

            if (useCallback)
            {
                this.OutputPort.SendAllBuffers(this.ConnectionPool);
            }
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"Component connection - Upstream component: {this.UpstreamComponent.Name} on port {this.OutputPort.Name} Downstream component: {this.DownstreamComponent.Name} on port {this.InputPort.Name}";
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            MMALLog.Logger.LogDebug("Disposing connection.");
            this.OutputPort?.CloseConnection();
            this.InputPort?.CloseConnection();
            this.Destroy();
            base.Dispose();
        }
        
        /// <summary>
        /// Enable a connection. The format of the two ports must have been committed before calling this function, although note that on creation, 
        /// the connection automatically copies and commits the output port's format to the input port.
        /// </summary>
        public void Enable()
        {
            if (!this.Enabled)
            {
                MMALLog.Logger.LogDebug($"Enabling connection between {this.OutputPort.Name} and {this.InputPort.Name}");
                MMALCheck(MMALConnection.mmal_connection_enable(this.Ptr), "Unable to enable connection");
            }
        }

        /// <summary>
        /// Disable a connection.
        /// </summary>
        public void Disable()
        {
            if (this.Enabled)
            {
                MMALLog.Logger.LogDebug($"Disabling connection between {this.OutputPort.Name} and {this.InputPort.Name}");
                MMALCheck(MMALConnection.mmal_connection_disable(this.Ptr), "Unable to disable connection");
            }
        }

        /// <summary>
        /// Destroy a connection. Release an acquired reference on a connection. Only actually destroys the connection when the last reference is 
        /// being released. The actual destruction of the connection will start by disabling it, if necessary. Any pool, queue, and so on owned by 
        /// the connection shall then be destroyed.
        /// </summary>
        public void Destroy()
        {
            // Cleaning port pools for sanity.
            this.UpstreamComponent.CleanPortPools();
            this.DownstreamComponent.CleanPortPools();
            
            MMALCheck(MMALConnection.mmal_connection_destroy(this.Ptr), "Unable to destroy connection");
        }

        /// <summary>
        /// Associates a <see cref="IConnectionCallbackHandler"/> for use with this connection instance. This will only be used
        /// if callbacks have been enabled against this connection.
        /// </summary>
        /// <param name="handler">The callback handler to use.</param>
        public void RegisterCallbackHandler(IConnectionCallbackHandler handler)
        {
            this.CallbackHandler = handler;
        }

        /// <summary>
        /// Facility to create a connection between two port objects.
        /// </summary>
        /// <param name="output">The output port of the connection.</param>
        /// <param name="input">The input port of the connection.</param>
        /// <param name="inputComponent">The managed instance of the component we are connecting to.</param>
        /// <param name="useCallback">When set to true, enable the connection callback delegate (adversely affects performance).</param>
        /// <returns>A new managed connection object.</returns>
        internal static MMALConnectionImpl CreateConnection(IOutputPort output, IInputPort input, IDownstreamComponent inputComponent, bool useCallback)
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
        /// Represents the native callback method for a connection between two ports.
        /// </summary>
        /// <param name="connection">The native pointer to a MMAL_CONNECTION_T struct.</param>
        /// <returns>The value of all flags set against this connection.</returns>
        protected virtual int NativeConnectionCallback(MMAL_CONNECTION_T* connection)
        {            
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug("Inside native connection callback");
            }
            
            var queue = new MMALQueueImpl(connection->Queue);
            var bufferImpl = queue.GetBuffer();

            if (bufferImpl.CheckState())
            {
                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }

                if (bufferImpl.Length > 0)
                {
                    this.CallbackHandler.InputCallback(bufferImpl);
                }

                this.InputPort.SendBuffer(bufferImpl);
            }
            else
            {
                queue = new MMALQueueImpl(connection->Pool->Queue);
                bufferImpl = queue.GetBuffer();

                if (bufferImpl.CheckState())
                {
                    if (MMALCameraConfig.Debug)
                    {
                        bufferImpl.PrintProperties();
                    }

                    if (bufferImpl.Length > 0)
                    {
                        this.CallbackHandler.OutputCallback(bufferImpl);
                    }

                    this.OutputPort.SendBuffer(bufferImpl);
                }
                else
                {
                    MMALLog.Logger.LogInformation("Buffer could not be obtained by connection callback");
                }
            }
            
            return (int)connection->Flags;
        }
        
        private void ConfigureConnectionCallback(IOutputPort output, IInputPort input)
        {
            output.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            input.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);

            this.NativeCallback = new MMALConnection.MMAL_CONNECTION_CALLBACK_T(this.NativeConnectionCallback);
            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

            this.Ptr->Callback = ptrCallback;

            this.ConnectionPool = new MMALPoolImpl(this.Ptr->Pool);
        }
    }
}
