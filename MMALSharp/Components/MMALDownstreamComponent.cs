using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component. A downstream component is a component that can have data passed to it from further up the component
    /// heirarchy.
    /// </summary>
    public abstract unsafe class MMALDownstreamComponent : MMALComponentBase
    {
        //Storage for downstream components. Stored here until we've finished processing. 
        public byte[] Storage { get; set; }

        /// <summary>
        /// Represents the connection between the upstream/downstream component
        /// </summary>
        public MMALConnectionImpl Connection { get; set; }

        public MMALDownstreamComponent(string name) : base(name)
        {
        }

        /// <summary>
        /// Provides a facility to create a connection between this component and an upstream component's output port.
        /// </summary>
        /// <param name="output"></param>
        public void CreateConnection(MMALPortBase output)
        {
            this.Connection = MMALConnectionImpl.CreateConnection(output, this.Inputs.ElementAt(0));
        }

        /// <summary>
        /// Closes a connection between an upstream/downstream component
        /// </summary>
        public void CloseConnection()
        {
            if (this.Connection != null)
                this.Connection.Disable();
        }
        
    }
}
