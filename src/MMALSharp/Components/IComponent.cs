// <copyright file="IComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Ports;
using MMALSharp.Ports.Controls;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a component.
    /// </summary>
    public interface IComponent : IMMALObject
    {
        /// <summary>
        /// The component's control port.
        /// </summary>
        IControlPort Control { get; }

        /// <summary>
        /// The list of input ports.
        /// </summary>
        List<IInputPort> Inputs { get; }

        /// <summary>
        /// The list of output ports.
        /// </summary>
        List<IOutputPort> Outputs { get; }

        /// <summary>
        /// The list of clock ports.
        /// </summary>
        List<IPort> Clocks { get; }

        /// <summary>
        /// The list of all ports.
        /// </summary>
        List<IPort> Ports { get; }

        /// <summary>
        /// The name of the component.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag whether this component is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Flag to force the component to stop processing.
        /// </summary>
        bool ForceStopProcessing { get; set; }

        /// <summary>
        /// Call to enable connections.
        /// </summary>
        void EnableConnections();

        /// <summary>
        /// Call to disable connections.
        /// </summary>
        void DisableConnections();

        /// <summary>
        /// Call to print metadata for this component.
        /// </summary>
        void PrintComponent();

        /// <summary>
        /// Acquire a reference on a component. Acquiring a reference on a component will prevent a component from being destroyed until the 
        /// acquired reference is released (by a call to mmal_component_destroy). References are internally counted so all acquired references 
        /// need a matching call to release them.
        /// </summary>
        void AcquireComponent();

        /// <summary>
        /// Release a reference on a component Release an acquired reference on a component. Triggers the destruction of the component 
        /// when the last reference is being released.
        /// </summary>
        void ReleaseComponent();

        /// <summary>
        /// Destroy a previously created component Release an acquired reference on a component. 
        /// Only actually destroys the component when the last reference is being released.
        /// </summary>
        void DestroyComponent();

        /// <summary>
        /// Enable processing on a component.
        /// </summary>
        void EnableComponent();

        /// <summary>
        /// Disable processing on a component.
        /// </summary>
        void DisableComponent();

        /// <summary>
        /// Helper method to destroy any port pools still in action. Failure to do this will cause MMAL to block indefinitely.
        /// </summary>
        void CleanPortPools();
    }
}
