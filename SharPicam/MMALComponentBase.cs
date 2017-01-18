using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

namespace SharPicam
{
    public abstract unsafe class MMALComponentBase : MMALObject
    {
        public MMAL_COMPONENT_T* Ptr { get; set; }
        public string Name { get; set; }
        public bool Enabled {
            get {
                return this.Ptr->isEnabled == 1;
            }
        }
        public MMALControlPortImpl Control { get; set; }
        public List<MMALPortImpl> Inputs { get; set; }
        public List<MMALPortImpl> Outputs { get; set; }
        public List<MMALPortImpl> Clocks { get; set; }
        public List<MMALPortImpl> Ports { get; set; }
        public MMALPoolImpl BufferPool { get; set; }

        protected MMALComponentBase(string name)
        {
            var ptr = CreateComponent(name);

            this.Ptr = ptr;

            this.Name = Marshal.PtrToStringAnsi((IntPtr)ptr->name);

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
                
            if(ptr->outputNum > 0)
            {                
                for (int i = 0; i < ptr->outputNum; i++)
                {
                    Outputs.Add(new MMALPortImpl(&(*ptr->output[i]), this));
                }
            }        
            
            if(ptr->clockNum > 0)
            {                
                for (int i = 0; i < ptr->clockNum; i++)
                {
                    Clocks.Add(new MMALPortImpl(&(*ptr->clock[i]), this));
                }
            }

            if((*ptr).portNum > 0)
            {                
                for (int i = 0; i < ptr->portNum; i++)
                {                    
                    Ports.Add(new MMALPortImpl(&(*ptr->port[i]), this));
                }
            }
        }

        private static MMAL_COMPONENT_T* CreateComponent(string name)
        {
            IntPtr ptr = IntPtr.Zero;
            MMALCheck(MMALComponent.mmal_component_create(name, &ptr), "Unable to create component");
                        
            var compPtr = (MMAL_COMPONENT_T*)ptr.ToPointer();
                        
            return compPtr;
        }

        protected void AcquireComponent()
        {
            MMALComponent.mmal_component_acquire(this.Ptr);
        }

        protected void ReleaseComponent()
        {
            MMALCheck(MMALComponent.mmal_component_release(this.Ptr), "Unable to release component");
        }

        protected void DestroyComponent()
        {
            MMALCheck(MMALComponent.mmal_component_destroy(this.Ptr), "Unable to destroy component");
        }

        protected void EnableComponent()
        {
            MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");                        
        }

        protected void DisableComponent()
        {
            MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to disable component");
        }

        public virtual void Dispose()
        {
            Console.WriteLine("Disposing component.");
            
            foreach(var port in Inputs)
            {
                port.DisablePort();
            }
            foreach (var port in Outputs)
            {
                port.DisablePort();
            }
            
            this.DisableComponent();
            this.DestroyComponent();
        }
        
    }
}
