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
    public abstract class MMALDownstreamComponent : MMALComponentBase
    {
        public MMALPortImpl InputPort { get; set; }
        public MMALPortImpl OutputPort { get; set; }

        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        protected MMALDownstreamComponent(string name, ICaptureHandler handler) : base(name)
        {
            this.Handler = handler;
            MMALCamera.Instance.DownstreamComponents.Add(this);
        }
        
        private void ClosePipelineConnections()
        {
            //Find any components this component is connected to, recursively removing these components.

            foreach (MMALPortImpl port in this.Outputs)
            {
                var connection = MMALCamera.Instance.Connections.Where(c => c.InputPort == port).FirstOrDefault();
                if (connection != null)
                {
                    //This component has an output port connected to another component.
                    connection.DownstreamComponent.ClosePipelineConnections();

                    //Destroy the connection
                    connection.Dispose();
                }
            }

            //Close any connection held by this component
            var finalConnection = MMALCamera.Instance.Connections.Where(c => c.DownstreamComponent == this).FirstOrDefault();

            if (finalConnection != null)
            {
                finalConnection.Dispose();
                
                MMALCamera.Instance.Connections.Remove(finalConnection);
            }
        }
        
        public override void Dispose()
        {
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Removing downstream component");
            }

            this.ClosePipelineConnections();

            //Remove any unmanaged resources held by the capture handler.
            this.Handler?.Dispose();

            MMALCamera.Instance.DownstreamComponents.Remove(this);

            base.Dispose();
        }

    }
}
