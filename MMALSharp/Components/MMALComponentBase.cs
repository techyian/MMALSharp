using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    public abstract unsafe class MMALComponentBase : MMALObject
    {        
        /// <summary>
        /// Reference to the Control port of this component
        /// </summary>
        public MMALControlPortImpl Control { get; set; }

        /// <summary>
        /// Reference to all input ports associated with this component
        /// </summary>
        public List<MMALPortImpl> Inputs { get; set; }

        /// <summary>
        /// Reference to all output ports associated with this component
        /// </summary>
        public List<MMALPortImpl> Outputs { get; set; }

        /// <summary>
        /// Reference to all clock ports associated with this component
        /// </summary>
        public List<MMALPortImpl> Clocks { get; set; }

        /// <summary>
        /// Reference to all ports associated with this component
        /// </summary>
        public List<MMALPortImpl> Ports { get; set; }
        
        /// <summary>
        /// Native pointer to the component this object represents
        /// </summary>
        internal MMAL_COMPONENT_T* Ptr { get; set; }

        /// <summary>
        /// Name of the component
        /// </summary>
        public string Name
        {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)this.Ptr->name);
            }
        }

        /// <summary>
        /// Indicates whether this component is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.Ptr->isEnabled == 1;
            }
        }


        protected MMALComponentBase(string name)
        {
            var ptr = CreateComponent(name);

            this.Ptr = ptr;
                        
            Inputs = new List<MMALPortImpl>();
            Outputs = new List<MMALPortImpl>();
            Clocks = new List<MMALPortImpl>();
            Ports = new List<MMALPortImpl>();

            this.Control = new MMALControlPortImpl(ptr->control, this);

            if (ptr->inputNum > 0)
            {                
                for (int i = 0; i < ptr->inputNum; i++)
                {                    
                    Inputs.Add(new MMALPortImpl(&(*ptr->input[i]), this));
                }
            }
                
            if (ptr->outputNum > 0)
            {                
                for (int i = 0; i < ptr->outputNum; i++)
                {
                    Outputs.Add(new MMALPortImpl(&(*ptr->output[i]), this));
                }
            }        
            
            if (ptr->clockNum > 0)
            {                
                for (int i = 0; i < ptr->clockNum; i++)
                {
                    Clocks.Add(new MMALPortImpl(&(*ptr->clock[i]), this));
                }
            }

            if ((*ptr).portNum > 0)
            {                
                for (int i = 0; i < ptr->portNum; i++)
                {                    
                    Ports.Add(new MMALPortImpl(&(*ptr->port[i]), this));
                }
            }
        }

        /// <summary>
        /// Provides a facility to create a component with a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static MMAL_COMPONENT_T* CreateComponent(string name)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALComponent.mmal_component_create(name, &ptr), "Unable to create component");
                        
            var compPtr = (MMAL_COMPONENT_T*)ptr.ToPointer();
                        
            return compPtr;
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
        /// Enable processing on a component
        /// </summary>
        public void EnableComponent()
        {
            if (!this.Enabled)
                MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");                        
        }

        /// <summary>
        /// Disable processing on a component
        /// </summary>
        public void DisableComponent()
        {
            if (this.Enabled)
                MMALCheck(MMALComponent.mmal_component_disable(this.Ptr), "Unable to disable component");
        }

        /// <summary>
        /// Helper method to initialize this component
        /// </summary>
        public abstract void Initialize();
        
        public override void Dispose()
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Disposing component {0}.", this.Name));
            
            //See if any pools need disposing before destroying component.
            foreach (var port in this.Inputs)
            {
                if (port.BufferPool != null)
                {
                    if (MMALCameraConfigImpl.Config.Debug)
                        Console.WriteLine("Desroying port pool");

                    port.DestroyPortPool();
                }
                    
            }                       
            foreach (var port in this.Outputs)
            {
                if (port.BufferPool != null)
                {
                    if (MMALCameraConfigImpl.Config.Debug)
                        Console.WriteLine("Desroying port pool");

                    port.DestroyPortPool();
                }                    
            }
                                                
            this.DisableComponent();
            this.DestroyComponent();

            base.Dispose();
        }
        
    }
}
