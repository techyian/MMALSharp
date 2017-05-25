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
            this.Inputs.ElementAt(0).ShallowCopy(this.Outputs.ElementAt(0));
            this.Handler = handler;
        }

        /// <summary>
        /// Removes an encoder component from the pipeline. Recursively removes all connected components associated with this component.
        /// </summary>
        public void Remove()
        {
            var enc = MMALCamera.Instance.DownstreamComponents.Where(c => c == this).FirstOrDefault();

            if (enc != null)
            {
                if (MMALCameraConfig.Debug)
                {
                    Console.WriteLine("Removing encoder");
                }

                //Find any components this component is connected to, recursively removing these components.

                foreach (MMALPortImpl port in this.Outputs)
                {
                    var connection = MMALCamera.Instance.Connections.Where(c => c.OutputPort == port).FirstOrDefault();
                    if (connection != null)
                    {
                        //This component has an output port connected to another component.
                        connection.DownstreamComponent.Remove();

                        //Destroy the connection
                        connection.Destroy();
                    }
                }
                
                MMALCamera.Instance.DownstreamComponents.Remove(enc);
                enc.Dispose();
            }
        }
        
        public override void Dispose()
        {
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Removing downstream component");
            }

            this.Remove();

            //Remove any unmanaged resources held by the capture handler.
            this.Handler?.Dispose();

            base.Dispose();
        }

    }
}
