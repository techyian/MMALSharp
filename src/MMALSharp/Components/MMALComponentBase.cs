// <copyright file="MMALComponentBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Base class for all components.
    /// </summary>
    public abstract unsafe class MMALComponentBase : MMALObject
    {
        /// <summary>
        /// Reference to the Control port of this component.
        /// </summary>
        public MMALControlPort Control { get; internal set; }

        /// <summary>
        /// Reference to all input ports associated with this component.
        /// </summary>
        public List<MMALPortImpl> Inputs { get; internal set; }

        /// <summary>
        /// Reference to all output ports associated with this component.
        /// </summary>
        public List<MMALPortImpl> Outputs { get; internal set; }

        /// <summary>
        /// Reference to all clock ports associated with this component.
        /// </summary>
        public List<MMALPortImpl> Clocks { get; internal set; }

        /// <summary>
        /// Reference to all ports associated with this component.
        /// </summary>
        public List<MMALPortImpl> Ports { get; internal set; }
        
        /// <summary>
        /// Name of the component
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)this.Ptr->Name);

        /// <summary>
        /// Indicates whether this component is enabled.
        /// </summary>
        public bool Enabled => this.Ptr->IsEnabled == 1;

        /// <summary>
        /// The handler to process the final data.
        /// </summary>
        public ICaptureHandler Handler { get; set; }

        /// <summary>
        /// Native pointer to the component this object represents.
        /// </summary>
        internal MMAL_COMPONENT_T* Ptr { get; set; }

        /// <summary>
        /// Creates the MMAL Component by the given name.
        /// </summary>
        /// <param name="name">The native MMAL name of the component you want to create.</param>
        protected MMALComponentBase(string name)
        {
            this.Ptr = CreateComponent(name);

            this.Inputs = new List<MMALPortImpl>();
            this.Outputs = new List<MMALPortImpl>();
            this.Clocks = new List<MMALPortImpl>();
            this.Ports = new List<MMALPortImpl>();

            this.Control = new MMALControlPort(this.Ptr->Control, this, PortType.Control, Guid.NewGuid());

            for (int i = 0; i < this.Ptr->InputNum; i++)
            {
                this.Inputs.Add(new MMALPortImpl(&(*this.Ptr->Input[i]), this, PortType.Input, Guid.NewGuid()));
            }

            for (int i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new MMALPortImpl(&(*this.Ptr->Output[i]), this, PortType.Output, Guid.NewGuid()));
            }

            for (int i = 0; i < this.Ptr->ClockNum; i++)
            {
                this.Clocks.Add(new MMALPortImpl(&(*this.Ptr->Clock[i]), this, PortType.Clock, Guid.NewGuid()));
            }

            for (int i = 0; i < this.Ptr->PortNum; i++)
            {
                this.Ports.Add(new MMALPortImpl(&(*this.Ptr->Port[i]), this, PortType.Unknown, Guid.NewGuid()));
            }
        }
        
        /// <summary>
        /// Enables any connections associated with this component, traversing down the pipeline to enable those connections
        /// also.
        /// </summary>
        public void EnableConnections()
        {
            foreach (MMALPortImpl port in this.Outputs)
            {
                if (port.ConnectedReference != null)
                {
                    // This component has an output port connected to another component.
                    port.ConnectedReference.DownstreamComponent.EnableConnections();

                    // Enable the connection
                    port.ConnectedReference.Enable();
                }
            }
        }

        /// <summary>
        /// Disables any connections associated with this component, traversing down the pipeline to disable those connections
        /// also.
        /// </summary>
        public void DisableConnections()
        {
            foreach (MMALPortImpl port in this.Outputs)
            {
                if (port.ConnectedReference != null)
                {
                    // This component has an output port connected to another component.
                    port.ConnectedReference.DownstreamComponent.DisableConnections();

                    // Disable the connection
                    port.ConnectedReference.Disable();
                }
            }
        }

        /// <summary>
        /// Prints a summary of the ports associated with this component to the console.
        /// </summary>
        public virtual void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: {this.Name}");

            for (var i = 0; i < this.Inputs.Count; i++)
            {
                if (this.Inputs[i].EncodingType != null)
                {
                    MMALLog.Logger.Info($"    Port {i} Input encoding: {this.Inputs[i].EncodingType.EncodingName}.");
                }
            }

            for (var i = 0; i < this.Outputs.Count; i++)
            {
                if (this.Outputs[i].EncodingType != null)
                {
                    MMALLog.Logger.Info($"    Port {i} Output encoding: {this.Outputs[i].EncodingType.EncodingName}");
                }
            }
        }

        /// <summary>
        /// Disposes of the current component, and frees any native resources still in use by it.
        /// </summary>
        public override void Dispose()
        {
            MMALLog.Logger.Debug($"Disposing component {this.Name}.");

            // See if any pools need disposing before destroying component.
            foreach (var port in this.Inputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.Debug("Destroying port pool");

                    port.DestroyPortPool();
                }
            }

            foreach (var port in this.Outputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.Debug("Destroying port pool");

                    port.DestroyPortPool();
                }
            }

            this.DisableComponent();
            this.DestroyComponent();

            MMALLog.Logger.Debug("Completed disposal...");

            base.Dispose();
        }
        
        /// <summary>
        /// Acquire a reference on a component. Acquiring a reference on a component will prevent a component from being destroyed until the 
        /// acquired reference is released (by a call to mmal_component_destroy). References are internally counted so all acquired references 
        /// need a matching call to release them.
        /// </summary>
        internal void AcquireComponent()
        {
            MMALComponent.mmal_component_acquire(this.Ptr);
        }

        /// <summary>
        /// Release a reference on a component Release an acquired reference on a component. Triggers the destruction of the component 
        /// when the last reference is being released.
        /// </summary>
        internal void ReleaseComponent()
        {
            MMALCheck(MMALComponent.mmal_component_release(this.Ptr), "Unable to release component");
        }

        /// <summary>
        /// Destroy a previously created component Release an acquired reference on a component. 
        /// Only actually destroys the component when the last reference is being released.
        /// </summary>
        internal void DestroyComponent()
        {
            MMALCheck(MMALComponent.mmal_component_destroy(this.Ptr), "Unable to destroy component");
        }

        /// <summary>
        /// Enable processing on a component.
        /// </summary>
        internal void EnableComponent()
        {
            if (!this.Enabled)
            {
                MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");
            }
        }

        /// <summary>
        /// Disable processing on a component.
        /// </summary>
        internal void DisableComponent()
        {
            if (this.Enabled)
            {
                MMALCheck(MMALComponent.mmal_component_disable(this.Ptr), "Unable to disable component");
            }
        }

        /// <summary>
        /// Enable the port with the specified port number.
        /// </summary>
        /// <param name="outputPortNumber">The output port number.</param>
        internal void Start(int outputPortNumber)
        {
            this.Start(this.Outputs[outputPortNumber]);
        }
        
        /// <summary>
        /// Enable the port specified.
        /// </summary>
        /// <param name="port">The output port.</param>
        internal void Start(MMALPortBase port)
        {
            switch (port.PortType)
            {
                case PortType.Input:
                    port.EnableInputPort();
                    break;
                case PortType.Output:
                    if (this.Handler != null && this.Handler.GetType().GetTypeInfo().IsSubclassOf(typeof(FileStreamCaptureHandler)))
                    {
                        ((FileStreamCaptureHandler)this.Handler).NewFile();
                    }
                    port.EnableOutputPort();
                    break;
                case PortType.Control:
                    port.EnableControlPort();
                    break;
                default:
                    port.EnableOutputPort();
                    break;
            }
        }
        
        /// <summary>
        /// Disable the port with the specified port number.
        /// </summary>
        /// <param name="outputPortNumber">The output port number.</param>
        internal void Stop(int outputPortNumber)
        {
            this.Outputs[outputPortNumber].DisablePort();
        }

        /// <summary>
        /// Disable the specified port.
        /// </summary>
        /// <param name="port">The output port.</param>
        internal void Stop(MMALPortBase port)
        {
            port.DisablePort();
        }

        /// <summary>
        /// Helper method to destroy any port pools still in action. Failure to do this will cause MMAL to block indefinitely.
        /// </summary>
        internal void CleanPortPools()
        {
            // See if any pools need disposing before destroying component.
            foreach (var port in this.Inputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.Debug("Destroying port pool");

                    if (port.Enabled)
                    {
                        port.DisablePort();
                    }

                    port.DestroyPortPool();
                    port.BufferPool = null;
                }
            }

            foreach (var port in this.Outputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.Debug("Destroying port pool");

                    if (port.Enabled)
                    {
                        port.DisablePort();
                    }

                    port.DestroyPortPool();
                    port.BufferPool = null;
                }
            }
        }

        /// <summary>
        /// Provides a facility to create a component with a given name.
        /// </summary>
        /// <param name="name">The name of the component to create.</param>
        /// <returns>A pointer to the new component struct.</returns>
        private static MMAL_COMPONENT_T* CreateComponent(string name)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALComponent.mmal_component_create(name, &ptr), "Unable to create component");

            var compPtr = (MMAL_COMPONENT_T*)ptr.ToPointer();

            return compPtr;
        }
    }
}
