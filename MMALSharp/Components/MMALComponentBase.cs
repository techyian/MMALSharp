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
        public MMALControlPortImpl Control { get; set; }
        public List<MMALPortImpl> Inputs { get; set; }
        public List<MMALPortImpl> Outputs { get; set; }
        public List<MMALPortImpl> Clocks { get; set; }
        public List<MMALPortImpl> Ports { get; set; }
        

        public MMAL_COMPONENT_T* Ptr { get; set; }
        public string Name
        {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)this.Ptr->name);
            }
        }
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

        public void AcquireComponent()
        {
            MMALComponent.mmal_component_acquire(this.Ptr);
        }

        public void ReleaseComponent()
        {
            MMALCheck(MMALComponent.mmal_component_release(this.Ptr), "Unable to release component");
        }

        public void DestroyComponent()
        {
            MMALCheck(MMALComponent.mmal_component_destroy(this.Ptr), "Unable to destroy component");
        }

        public void EnableComponent()
        {
            if(!this.Enabled)
                MMALCheck(MMALComponent.mmal_component_enable(this.Ptr), "Unable to enable component");                        
        }

        public void DisableComponent()
        {
            if(this.Enabled)
                MMALCheck(MMALComponent.mmal_component_disable(this.Ptr), "Unable to disable component");
        }

        public abstract void Initialize();
        
        public override void Dispose()
        {
            Console.WriteLine("Disposing component.");

            this.Control.DisablePort();

            foreach (var port in this.Outputs)
            {
                port.DisablePort();
            }

            foreach (var port in this.Inputs)
            {                
                port.DisablePort();
            }
                        
            this.DisableComponent();
            this.DestroyComponent();

            base.Dispose();
        }
        
    }
}
