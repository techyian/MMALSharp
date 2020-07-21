// <copyright file="MMALDownstreamComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component. A downstream component is a component that can have data passed to it from further up the component
    /// hierarchy.
    /// </summary>
    public abstract class MMALDownstreamComponent : MMALComponentBase, IDownstreamComponent
    {
        /// <summary>
        /// A list of working ports which are processing data in the component pipeline.
        /// </summary>
        public Dictionary<int, IOutputPort> ProcessingPorts { get; set; }

        /// <summary>
        /// Creates a new instance of a Downstream component.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        protected MMALDownstreamComponent(string name)
            : base(name)
        {
            MMALBootstrapper.DownstreamComponents.Add(this);
            this.ProcessingPorts = new Dictionary<int, IOutputPort>();
        }

        /// <summary>
        /// Call to configure changes on a downstream component input port.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The input port capture handler. This will be non-null if this port's component is the 1st component in the 
        /// pipeline and you are feeding data to it directly from a <see cref="IInputCaptureHandler"/>. If this port is connected to by another component then leave this parameter null.
        /// </param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual IDownstreamComponent ConfigureInputPort(IMMALPortConfig config, IInputCaptureHandler handler)
        {
            return this.ConfigureInputPort(config, null, handler);
        }

        /// <summary>
        /// Configures a specific input port on a downstream component. This method will perform a shallow copy of the output
        /// port it is to be connected to.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="copyPort">The output port we are copying format data from.</param>
        /// <param name="handler">The input port capture handler. This will be non-null if this port's component is the 1st component in the 
        /// pipeline and you are feeding data to it directly from a <see cref="IInputCaptureHandler"/>. If this port is connected to by another component then leave this parameter null.
        /// </param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe IDownstreamComponent ConfigureInputPort(IMMALPortConfig config, IPort copyPort, IInputCaptureHandler handler)
        {
            this.Inputs[0].Configure(config, copyPort, handler);

            if (this.Outputs.Count > 0 && this.Outputs[0].Ptr->Format->Type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            return this;
        }

        /// <summary>
        /// Call to configure changes on a downstream component input port.
        /// </summary>
        /// <typeparam name="TPort">Input port type.</typeparam>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The input port capture handler.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe IDownstreamComponent ConfigureInputPort<TPort>(IMMALPortConfig config, IInputCaptureHandler handler)
            where TPort : IInputPort
        {
            this.Inputs[0] = (IInputPort)Activator.CreateInstance(typeof(TPort), (IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid());

            return this.ConfigureInputPort(config, null, handler);
        }

        /// <summary>
        /// Call to configure changes on the first downstream component output port.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual IDownstreamComponent ConfigureOutputPort(IMMALPortConfig config, IOutputCaptureHandler handler)
        {
            return this.ConfigureOutputPort(0, config, handler);
        }

        /// <summary>
        /// Call to configure changes on a downstream component output port.
        /// </summary>
        /// <param name="outputPort">The output port number to configure.</param>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual IDownstreamComponent ConfigureOutputPort(int outputPort, IMMALPortConfig config, IOutputCaptureHandler handler)
        {
            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);
            
            this.Outputs[outputPort].Configure(config, this.Inputs[0], handler);

            return this;
        }

        /// <summary>
        /// Call to configure changes on a downstream component output port.
        /// </summary>
        /// <typeparam name="TPort">Output port type.</typeparam>
        /// <param name="outputPort">The output port number to configure.</param>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe IDownstreamComponent ConfigureOutputPort<TPort>(int outputPort, IMMALPortConfig config, IOutputCaptureHandler handler)
            where TPort : IOutputPort
        {
            this.Outputs[outputPort] = (IOutputPort)Activator.CreateInstance(typeof(TPort), (IntPtr)(&(*this.Ptr->Output[outputPort])), this, Guid.NewGuid());

            return this.ConfigureOutputPort(outputPort, config, handler);
        }

        /// <summary>
        /// Disposes of the current component, closes all connections and frees all associated unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.ClosePipelineConnections();

            MMALBootstrapper.DownstreamComponents.Remove(this);

            MMALLog.Logger.LogDebug($"Remaining components in pipeline: {MMALBootstrapper.DownstreamComponents.Count}");

            base.Dispose();
        }
        
        /// <summary>
        /// Responsible for closing and destroying any connections associated with this component prior to disposing.
        /// </summary>
        private void ClosePipelineConnections()
        {
            // Close any connection held by this component
            foreach (var input in this.Inputs)
            {
                if (input.ConnectedReference != null)
                {
                    MMALLog.Logger.LogDebug($"Removing {input.ConnectedReference}");
                    input.ConnectedReference.Dispose();
                }
            }

            foreach (var output in this.Outputs)
            {
                if (output.ConnectedReference != null)
                {
                    MMALLog.Logger.LogDebug($"Removing {output.ConnectedReference}");
                    output.ConnectedReference.Dispose();
                }
            }
        }
    }
}
