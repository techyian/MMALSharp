using MMALSharp.Handlers;
using System;
using System.Linq;
using System.Reflection;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component. A downstream component is a component that can have data passed to it from further up the component
    /// heirarchy.
    /// </summary>
    public abstract unsafe class MMALDownstreamComponent : MMALComponentBase
    {               
        /// <summary>
        /// Represents the connection between the upstream/downstream component
        /// </summary>
        public MMALConnectionImpl Connection { get; set; }
        
        protected MMALDownstreamComponent(string name, ICaptureHandler handler) : base(name)
        {            
            this.Handler = handler;
        }

        /// <summary>
        /// Provides a facility to create a connection between this component and an upstream component's output port.
        /// </summary>
        /// <param name="output">The output port we're connecting this downstream component to</param>
        internal void CreateConnection(MMALPortBase output)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Creating downstream connection");
            this.Connection = MMALConnectionImpl.CreateConnection(output, this.Inputs.ElementAt(0));
        }
                        
    }
}
