using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Creating downstream connection");
            this.Connection = MMALConnectionImpl.CreateConnection(output, this.Inputs.ElementAt(0));
        }

        public void CloseConnection()
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Closing downstream connection");
            this.Connection.Disable();
        }

        /// <summary>
        /// Delegate to process the buffer header containing image data
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="port"></param>
        public virtual void ManagedCallback(MMALBufferImpl buffer, MMALPortBase port)
        {
            var data = buffer.GetBufferData();
            port.ProcessCallback(data);            
        }

        public void Start(int outputPortNumber, Action<MMALBufferImpl, MMALPortBase> managedCallback, Action<byte[]> processCallback)
        {            
            this.Outputs.ElementAt(outputPortNumber).EnablePort(managedCallback, processCallback);
        }

        public void Start(MMALPortBase port, Action<MMALBufferImpl, MMALPortBase> managedCallback, Action<byte[]> processCallback)
        {            
            port.EnablePort(managedCallback, processCallback);
        }

        public void Stop(int outputPortNumber)
        {
            this.Outputs.ElementAt(outputPortNumber).DisablePort();
        }

        public void Stop(MMALPortBase port)
        {
            port.DisablePort();
        }

        
    }
}
