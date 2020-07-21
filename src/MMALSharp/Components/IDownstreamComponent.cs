// <copyright file="IDownstreamComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component, i.e. a component that can be connected to.
    /// </summary>
    public interface IDownstreamComponent : IComponent
    {
        /// <summary>
        /// The ports which are processing data on this component.
        /// </summary>
        Dictionary<int, IOutputPort> ProcessingPorts { get; }
      
        /// <summary>
        /// Configures the input port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="copyPort">The port to copy from.</param>
        /// <param name="handler">The capture handler to use with this port.</param>
        /// <returns>This component.</returns>
        IDownstreamComponent ConfigureInputPort(IMMALPortConfig config, IPort copyPort, IInputCaptureHandler handler);

        /// <summary>
        /// Call to configure changes on a downstream component input port.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <param name="handler">The input port capture handler. This will be non-null if this port's component is the 1st component in the 
        /// pipeline and you are feeding data to it directly from a <see cref="IInputCaptureHandler"/>. If this port is connected to by another component then leave this parameter null.
        /// </param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        IDownstreamComponent ConfigureInputPort(IMMALPortConfig config, IInputCaptureHandler handler);

        /// <summary>
        /// Configures the input port. In addition, it will create a new instance of the port
        /// in the generic constraint and assign it to the component.
        /// </summary>
        /// <typeparam name="TPort">The input port constraint.</typeparam>
        /// <param name="config">The port configuration object.</param>
        /// <param name="handler">The input port capture handler. This will be non-null if this port's component is the 1st component in the 
        /// pipeline and you are feeding data to it directly from a <see cref="IInputCaptureHandler"/>. If this port is connected to by another component then leave this parameter null.
        /// </param>
        /// <returns>This component.</returns>
        IDownstreamComponent ConfigureInputPort<TPort>(IMMALPortConfig config, IInputCaptureHandler handler) 
            where TPort : IInputPort;

        /// <summary>
        /// Configures the output port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <returns>This component.</returns>
        IDownstreamComponent ConfigureOutputPort(IMMALPortConfig config, IOutputCaptureHandler handler);

        /// <summary>
        /// Configures the output port.
        /// </summary>
        /// <param name="outputPort">The output port number.</param>
        /// <param name="config">The port configuration object.</param>
        /// <param name="handler">The capture handler to use with this port.</param>
        /// <returns>This component.</returns>
        IDownstreamComponent ConfigureOutputPort(int outputPort, IMMALPortConfig config, IOutputCaptureHandler handler);

        /// <summary>
        /// Configures the output port. In addition, it will create a new instance of the port
        /// in the generic constraint and assign it to the component.
        /// </summary>
        /// <typeparam name="TPort">The output port constraint.</typeparam>
        /// <param name="outputPort">The output port number.</param>
        /// <param name="config">The port configuration object.</param>
        /// <param name="handler">The capture handler to use with this port.</param>
        /// <returns>This component.</returns>
        IDownstreamComponent ConfigureOutputPort<TPort>(int outputPort, IMMALPortConfig config, IOutputCaptureHandler handler)
            where TPort : IOutputPort;
    }
}
