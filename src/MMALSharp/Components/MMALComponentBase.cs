// <copyright file="MMALComponentBase.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Clocks;
using MMALSharp.Ports.Controls;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp
{
    /// <summary>
    /// Base class for all components.
    /// </summary>
    public abstract unsafe class MMALComponentBase : MMALObject, IComponent
    {
        /// <summary>
        /// Reference to the Control port of this component.
        /// </summary>
        public IControlPort Control { get; }

        /// <summary>
        /// Reference to all input ports associated with this component.
        /// </summary>
        public List<IInputPort> Inputs { get; }

        /// <summary>
        /// Reference to all output ports associated with this component.
        /// </summary>
        public List<IOutputPort> Outputs { get; }

        /// <summary>
        /// Reference to all clock ports associated with this component.
        /// </summary>
        public List<IPort> Clocks { get; }

        /// <summary>
        /// Reference to all ports associated with this component.
        /// </summary>
        public List<IPort> Ports { get; }
        
        /// <summary>
        /// Name of the component
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)this.Ptr->Name);

        /// <summary>
        /// Indicates whether this component is enabled.
        /// </summary>
        public bool Enabled => this.Ptr->IsEnabled == 1;

        /// <summary>
        /// Flag to force processing to stop on this component.
        /// </summary>
        public bool ForceStopProcessing { get; set; }
        
        /// <summary>
        /// Native pointer to the component this object represents.
        /// </summary>
        internal MMAL_COMPONENT_T* Ptr { get; }
        
        /// <summary>
        /// Creates the MMAL Component by the given name.
        /// </summary>
        /// <param name="name">The native MMAL name of the component you want to create.</param>
        protected MMALComponentBase(string name)
        {
            this.Ptr = CreateComponent(name);

            this.Inputs = new List<IInputPort>();
            this.Outputs = new List<IOutputPort>();
            this.Clocks = new List<IPort>();
            this.Ports = new List<IPort>();

            this.Control = new ControlPort((IntPtr)this.Ptr->Control, this, Guid.NewGuid());
            
            for (int i = 0; i < this.Ptr->ClockNum; i++)
            {
                this.Clocks.Add(new ClockPort((IntPtr)(&(*this.Ptr->Clock[i])), this, Guid.NewGuid()));
            }

            for (int i = 0; i < this.Ptr->PortNum; i++)
            {
                this.Ports.Add(new GenericPort<ICallbackHandler>((IntPtr)(&(*this.Ptr->Port[i])), this, PortType.Generic, Guid.NewGuid()));
            }
        }

        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Enables any connections associated with this component, traversing down the pipeline to enable those connections
        /// also.
        /// </summary>
        /// <exception cref="MMALPortConnectedException">Thrown when port enabled prior to enabling connection.</exception>
        public void EnableConnections()
        {
            foreach (IOutputPort port in this.Outputs)
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
        /// <exception cref="MMALPortConnectedException">Thrown when port still enabled prior to disabling connection.</exception>
        public void DisableConnections()
        {
            foreach (IOutputPort port in this.Outputs)
            {
                if (port.ConnectedReference != null)
                {
                    MMALLog.Logger.LogDebug($"Disabling connection between {this.Name} and {port.ConnectedReference.DownstreamComponent.Name}");

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
            MMALLog.Logger.LogInformation($"Component: {this.Name}");

            var sb = new StringBuilder();

            for (var i = 0; i < this.Inputs.Count; i++)
            {
                if (this.Inputs[i].EncodingType != null)
                {
                    sb.Append($"    INPUT port {i} encoding: {this.Inputs[i].NativeEncodingType.ParseEncoding().EncodingName}. \n");
                    sb.Append($"        Width: {this.Inputs[i].Resolution.Width}. Height: {this.Inputs[i].Resolution.Height} \n");
                    sb.Append($"        Num buffers: {this.Inputs[i].BufferNum}. Buffer size: {this.Inputs[i].BufferSize}. \n");
                    sb.Append($"        Rec num buffers: {this.Inputs[i].BufferNumRecommended}. Rec buffer size: {this.Inputs[i].BufferSizeRecommended} \n");
                    sb.Append($"        Resolution: {this.Inputs[i].Resolution.Width} x {this.Inputs[i].Resolution.Height} \n");
                    sb.Append($"        Crop: {this.Inputs[i].Crop.Width} x {this.Inputs[i].Crop.Height} \n \n");
                }
            }

            for (var i = 0; i < this.Outputs.Count; i++)
            {
                if (this.Outputs[i].EncodingType != null)
                {
                    sb.Append($"    OUTPUT port {i} encoding: {this.Outputs[i].NativeEncodingType.ParseEncoding().EncodingName}. \n");
                    sb.Append($"        Width: {this.Outputs[i].Resolution.Width}. Height: {this.Outputs[i].Resolution.Height} \n");
                    sb.Append($"        Num buffers: {this.Outputs[i].BufferNum}. Buffer size: {this.Outputs[i].BufferSize}. \n");
                    sb.Append($"        Rec num buffers: {this.Outputs[i].BufferNumRecommended}. Rec buffer size: {this.Outputs[i].BufferSizeRecommended} \n");
                    sb.Append($"        Resolution: {this.Outputs[i].Resolution.Width} x {this.Outputs[i].Resolution.Height} \n");
                    sb.Append($"        Crop: {this.Outputs[i].Crop.Width} x {this.Outputs[i].Crop.Height} \n \n");
                }
            }

            MMALLog.Logger.LogInformation(sb.ToString());
        }

        /// <summary>
        /// Disposes of the current component, and frees any native resources still in use by it.
        /// </summary>
        public override void Dispose()
        {
            if (this.CheckState())
            {
                MMALLog.Logger.LogDebug($"Disposing component {this.Name}.");

                // See if any pools need disposing before destroying component.
                foreach (var port in this.Inputs)
                {
                    if (port.BufferPool != null)
                    {
                        MMALLog.Logger.LogDebug("Destroying port pool");

                        port.DestroyPortPool();
                    }
                }

                foreach (var port in this.Outputs)
                {
                    if (port.BufferPool != null)
                    {
                        MMALLog.Logger.LogDebug("Destroying port pool");

                        port.DestroyPortPool();
                    }
                }

                this.DisableComponent();
                this.DestroyComponent();

                MMALLog.Logger.LogDebug("Completed disposal...");

                base.Dispose();
            }
        }
        
        /// <summary>
        /// Acquire a reference on a component. Acquiring a reference on a component will prevent a component from being destroyed until the 
        /// acquired reference is released (by a call to mmal_component_destroy). References are internally counted so all acquired references 
        /// need a matching call to release them.
        /// </summary>
        public void AcquireComponent()
        {
            MMALComponent.mmal_component_acquire(this.Ptr);
        }

        /// <summary>
        /// Release a reference on a component Release an acquired reference on a component. Triggers the destruction of the component 
        /// when the last reference is being released.
        /// </summary>
        public void ReleaseComponent()
        {
            MMALCheck(MMALComponent.mmal_component_release(this.Ptr), "Unable to release component");
        }

        /// <summary>
        /// Destroy a previously created component Release an acquired reference on a component. 
        /// Only actually destroys the component when the last reference is being released.
        /// </summary>
        public void DestroyComponent()
        {
            MMALCheck(MMALComponent.mmal_component_destroy(this.Ptr), "Unable to destroy component");
        }

        /// <summary>
        /// Enable processing on a component.
        /// </summary>
        public void EnableComponent()
        {
            if (!this.Enabled)
            {
                MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");
            }
        }

        /// <summary>
        /// Disable processing on a component.
        /// </summary>
        public void DisableComponent()
        {
            if (this.Enabled)
            {
                MMALCheck(MMALComponent.mmal_component_disable(this.Ptr), "Unable to disable component");
            }
        }

        /// <summary>
        /// Helper method to destroy any port pools still in action. Failure to do this will cause MMAL to block indefinitely.
        /// </summary>
        public void CleanPortPools()
        {
            // See if any pools need disposing before destroying component.
            foreach (var port in this.Inputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.LogDebug("Destroying input port pool.");
                    
                    port.DestroyPortPool();
                }
            }

            foreach (var port in this.Outputs)
            {
                if (port.BufferPool != null)
                {
                    MMALLog.Logger.LogDebug("Destroying output port pool.");
                    
                    port.DestroyPortPool();
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
